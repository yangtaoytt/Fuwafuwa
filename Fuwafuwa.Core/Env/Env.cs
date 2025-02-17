using Fuwafuwa.Core.Container.Level3;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level2;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Distributor.Implement;
using Fuwafuwa.Core.Distributor.Interface;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Env;

// this class is not concurrent safe
public class Env {
    private readonly CancellationTokenSource _cancelSource;

    private readonly Dictionary<Type, (IRegistrableContainer container, Task task, bool isRegisted)> _customContainers;

    private readonly List<Task> _defaultServiceTask;

    private readonly ServiceRegisterGroup _group;

    private readonly Logger2Event? _logger;


    public Env(int concurrencyLevel, Logger2Event? logger = null) {
        ConcurrencyLevel = concurrencyLevel;
        _logger = logger;
        _group = new ServiceRegisterGroup(_logger);
        _cancelSource = new CancellationTokenSource();

        var subjectBufferContainer = new SubjectBufferContainer(concurrencyLevel,
            () => new HashDistributor<NullServiceData, SubjectData, (SimpleSharedDataWrapper<Register>,
                NullSharedDataWrapper<object>)>(),
            (new SimpleSharedDataWrapper<Register>(new Register(_group)), new object())
            , _logger);

        var taskAgentContainer = new TaskAgentContainer(ConcurrencyLevel,
            () => new PollingDistributor<TaskAgentData, NullSubjectData, (SimpleSharedDataWrapper<Register>,
                NullSharedDataWrapper<object>)>(),
            (new SimpleSharedDataWrapper<Register>(new Register(_group)), new object())
            , _logger);

        _defaultServiceTask = [
            RunTask(subjectBufferContainer),
            RunTask(taskAgentContainer)
        ];


        _group.RegisterAndBroadcast(subjectBufferContainer).Wait();
        _group.RegisterAndBroadcast(taskAgentContainer).Wait();


        _customContainers = [];
    }

    public int ConcurrencyLevel { get; init; }

    public List<(Type serviceType, bool isRegister)> GetServiceStatus() {
        return _customContainers.Select(kvp => (kvp.Key, kvp.Value.isRegisted)).ToList();
    }

    private Task RunTask(IRegistrableContainer container) {
        return container.Run(_cancelSource.Token)
            .ContinueWith(
                task => {
                    if (task.IsFaulted) {
                        _cancelSource.Cancel();
                    }
                });
    }

    private Type Run(IRegistrableContainer container) {
        var serviceType = container.ServiceAttributeType.serviceType;
        var task = RunTask(container);
        _customContainers.Add(serviceType, (container, task, false));
        return serviceType;
    }

    public async Task Register(Type serviceType) {
        var (container, task, isRegistered) = _customContainers[serviceType];
        if (isRegistered) {
            return;
        }

        _customContainers[serviceType] = (container, task, true);
        await _group.RegisterAndBroadcast(container);
    }

    // be careful, sometimes if you stop a service and then write to input, but immediately restart it,
    // the stop function may not work,
    // this is because the input is async, so before the service get input message, the restart is already executed.
    public async Task UnRegister(Type serviceType) {
        var (container, task, isRegistered) = _customContainers[serviceType];
        if (!isRegistered) {
            return;
        }

        _customContainers[serviceType] = (container, task, false);
        await _group.UnregisterAndBroadcast(container);
    }

    public void Close() {
        _cancelSource.Cancel();
        Task.WaitAll(_customContainers.Values.Select(kvp => kvp.task).ToArray());
        Task.WaitAll(_defaultServiceTask);
    }

    public async Task UnRegisterAll() {
        foreach (var (container, _) in _customContainers) {
            await UnRegister(container);
        }
    }

    public async Task RegisterAll() {
        foreach (var (container, _) in _customContainers) {
            await Register(container);
        }
    }


    public (Type, InputHandler<TInputData>) CreateRunPollingInput<TInputCore, TInputData,
        TSharedData, TInitData>(TInitData initData)
        where TInputCore : IInputCore<TSharedData, TInitData>, new()
        where TInitData : new()
        where TSharedData : ISharedDataWrapper {
        var inputHandler = new InputHandler<TInputData>();

        var inputContainer = new InputContainer<TInputCore, TInputData, TSharedData, TInitData>(
            ConcurrencyLevel,
            () => new PollingDistributor<InputPackagedData, NullSubjectData, (SimpleSharedDataWrapper<Register>,
                TSharedData)>(),
            inputHandler, (new SimpleSharedDataWrapper<Register>(new Register(_group)), new TInitData()), _logger);
        var serviceType = Run(inputContainer);

        return (serviceType, inputHandler);
    }


    public async Task<(Type, InputHandler<TInputData>)> CreateRunRegisterPollingInput<TInputCore, TInputData,
        TSharedData, TInitData>(TInitData initData)
        where TInputCore : IInputCore<TSharedData, TInitData>, new()
        where TInitData : new()
        where TSharedData : ISharedDataWrapper {
        var (serviceType, inputHandler) =
            CreateRunPollingInput<TInputCore, TInputData, TSharedData, TInitData>(initData);
        await Register(serviceType);
        return (serviceType, inputHandler);
    }

    public Type CreateRunProcessor<TProcessorCore, TServiceData, TDistributor, TSharedData, TInitData>(
        TInitData initData)
        where TServiceData : IProcessorData
        where TProcessorCore : IProcessorCore<TServiceData, TSharedData, TInitData>, new()
        where TSharedData : ISharedDataWrapper
        where TDistributor : IDistributor<TServiceData, SubjectDataWithCommand, (SimpleSharedDataWrapper<Register>,
            TSharedData)>, new() {
        var processorContainer = new ProcessorContainer<TProcessorCore, TServiceData, TSharedData, TInitData>(
            ConcurrencyLevel,
            () => new TDistributor(),
            (new SimpleSharedDataWrapper<Register>(new Register(_group)), initData), _logger);
        var serviceType = Run(processorContainer);
        return serviceType;
    }
    
    public Type CreateRunPollingProcessor<TProcessorCore, TServiceData, TSharedData, TInitData>(
        TInitData initData)
        where TServiceData : IProcessorData
        where TProcessorCore : IProcessorCore<TServiceData, TSharedData, TInitData>, new()
        where TSharedData : ISharedDataWrapper {
        return CreateRunProcessor<TProcessorCore, TServiceData, PollingDistributor<TServiceData, SubjectDataWithCommand,
            (SimpleSharedDataWrapper<Register>,
            TSharedData)>, TSharedData, TInitData>(initData);
    }

    public async Task<Type> CreateRunRegisterPollingProcessor<TProcessorCore, TServiceData, TSharedData, TInitData>(
        TInitData initData)
        where TServiceData : IProcessorData
        where TProcessorCore : IProcessorCore<TServiceData, TSharedData, TInitData>, new()
        where TSharedData : ISharedDataWrapper {
        var serviceType = CreateRunPollingProcessor<TProcessorCore, TServiceData, TSharedData, TInitData>(initData);
        await Register(serviceType);
        return serviceType;
    }

    public Type CreateRunPollingExecutor<TExecutorCore, TServiceData, TSharedData, TInitData>(
        TInitData initData)
        where TServiceData : AExecutorData
        where TExecutorCore : IExecutorCore<TServiceData, TSharedData, TInitData>, new()
        where TSharedData : ISharedDataWrapper {
        var executorContainer = new ExecutorContainer<TExecutorCore, TServiceData, TSharedData, TInitData>(
            ConcurrencyLevel,
            () => new PollingDistributor<TServiceData, NullSubjectData, ValueTuple<TSharedData>>(), initData,
            _logger);
        var serviceType = Run(executorContainer);
        return serviceType;
    }

    public async Task<Type> CreateRunRegisterPollingExecutor<TExecutorCore, TServiceData, TSharedData, TInitData>(
        TInitData initData)
        where TServiceData : AExecutorData
        where TExecutorCore : IExecutorCore<TServiceData, TSharedData, TInitData>, new()
        where TSharedData : ISharedDataWrapper {
        var serviceType = CreateRunPollingExecutor<TExecutorCore, TServiceData, TSharedData, TInitData>(initData);
        await Register(serviceType);
        return serviceType;
    }
}