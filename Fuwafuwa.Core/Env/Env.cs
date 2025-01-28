using Fuwafuwa.Core.Container.Level3;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Distributor.Implement;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
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

        _cancelSource = new CancellationTokenSource();

        var subjectBufferContainer = new SubjectBufferContainer(concurrencyLevel,
            () => new HashDistributor<NullServiceData, SubjectData, InitTuple<Register, object>>(), _logger);

        var taskAgentContainer = new TaskAgentContainer(ConcurrencyLevel,
            () => new PollingDistributor<TaskAgentData, NullSubjectData, InitTuple<Register, object>>(), _logger);

        _defaultServiceTask = [
            subjectBufferContainer.Run(_cancelSource.Token),
            taskAgentContainer.Run(_cancelSource.Token)
        ];

        _group = new ServiceRegisterGroup(_logger);

        _group.RegisterAndBroadcast(subjectBufferContainer).Wait();
        _group.RegisterAndBroadcast(taskAgentContainer).Wait();


        _customContainers = [];
        
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Console.WriteLine($"[Exception] unobservedException: {e.Exception}");
            e.SetObserved();
        };
    }

    public int ConcurrencyLevel { get; init; }

    public Type Run(IRegistrableContainer container) {
        var serviceType = container.ServiceAttributeType.serviceType;
        var task = container.Run(_cancelSource.Token);
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

    public async Task<Type> RunRegister(IRegistrableContainer container) {
        var res = Run(container);

        await Register(container.ServiceAttributeType.serviceType);

        return res;
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

    public void UnRegisterAll() {
        _customContainers.ToList().ForEach(kvp => UnRegister(kvp.Key).Wait());
    }

    public void RegisterAll() {
        _customContainers.ToList().ForEach(kvp => Register(kvp.Key).Wait());
    }


    public Task<(Type, InputHandler<TInputData>)> CreateRunRegisterPollingInputWithoutSharedData<TService, TInputData>()
        where TService : BaseInputService<object>, new() {
        return CreateRunRegisterPollingInput<TService, TInputData, object>();
    }

    public async Task<(Type, InputHandler<TInputData>)> CreateRunRegisterPollingInput<TService, TInputData, TSharedData>()
        where TService : BaseInputService<TSharedData>, new() where TSharedData : new() {
        var inputHandler = new InputHandler<TInputData>();

        var inputContainer = new InputContainer<TService, TInputData, TSharedData>(
            ConcurrencyLevel,
            () => new PollingDistributor<InputPackagedData, NullSubjectData, InitTuple<Register, TSharedData>>(),
            inputHandler, _logger);
        var serviceType = await RunRegister(inputContainer);

        return (serviceType, inputHandler);
    }


    public Task<Type> CreateRunRegisterPollingProcessorWithoutSharedData<TService, TServiceData>()
        where TService : BaseProcessService<TServiceData, object>, new()
        where TServiceData : IProcessorData {
        return CreateRunRegisterPollingProcessor<TService, TServiceData, object>();
    }

    public async Task<Type> CreateRunRegisterPollingProcessor<TService, TServiceData, TSharedData>()
        where TService : BaseProcessService<TServiceData, TSharedData>, new()
        where TServiceData : IProcessorData
        where TSharedData : new() {
        var processorContainer = new ProcessorContainer<TService, TServiceData, TSharedData>(
            ConcurrencyLevel,
            () => new PollingDistributor<TServiceData, SubjectDataWithCommand, InitTuple<Register, TSharedData>>(), _logger);
        var serviceType = await RunRegister(processorContainer);

        return serviceType;
    }

    public Task<Type> CreateRunRegisterPollingExecutorWithoutSharedData<TService, TServiceData>()
        where TService : BaseExecutorService<TServiceData, object>, new()
        where TServiceData : AExecutorData {
        return CreateRunRegisterPollingExecutor<TService, TServiceData, object>();
    }

    public async Task<Type> CreateRunRegisterPollingExecutor<TService, TServiceData, TSharedData>()
        where TService : BaseExecutorService<TServiceData, TSharedData>, new()
        where TServiceData : AExecutorData
        where TSharedData : new() {
        var executorContainer = new ExecutorContainer<TService, TServiceData, TSharedData>(
            ConcurrencyLevel,
            () => new PollingDistributor<TServiceData, NullSubjectData, InitTuple<TSharedData>>(), _logger);
        Console.WriteLine("-1");
        var serviceType = await RunRegister(executorContainer);
        Console.WriteLine("-2");
        return serviceType;
    }
}