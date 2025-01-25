using Fuwafuwa.Core.Container.Level3;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Distributor.Implement;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Env;

// this class is not concurrent safe
public class Env {
    private readonly CancellationTokenSource _cancelSource;

    private readonly Dictionary<Type, (IRegistrableContainer container, Task task, bool isRegisted)> _customContainers;

    private readonly List<Task> _defaultServiceTask;

    private readonly ServiceRegisterGroup _group;


    public Env(int concurrencyLevel) {
        ConcurrencyLevel = concurrencyLevel;

        _cancelSource = new CancellationTokenSource();

        var subjectBufferContainer = new SubjectBufferContainer(concurrencyLevel,
            () => new HashDistributor<NullServiceData, SubjectData, InitTuple<Register, object>>());

        var taskAgentContainer = new TaskAgentContainer(ConcurrencyLevel,
            () => new PollingDistributor<TaskAgentData, NullSubjectData, InitTuple<Register, object>>());

        _defaultServiceTask = [
            subjectBufferContainer.Run(_cancelSource.Token),
            taskAgentContainer.Run(_cancelSource.Token)
        ];

        _group = new ServiceRegisterGroup();

        _group.RegisterAndBroadcast(subjectBufferContainer).Wait();
        _group.RegisterAndBroadcast(taskAgentContainer).Wait();


        _customContainers = [];
    }

    public int ConcurrencyLevel { get; init; }

    public Type Add(IRegistrableContainer container) {
        var serviceType = container.ServiceAttributeType.serviceType;
        var task = container.Run(_cancelSource.Token);
        _customContainers.Add(serviceType, (container, task, false));
        return serviceType;
    }

    public async Task Run(Type serviceType) {
        var (container, task, isRegistered) = _customContainers[serviceType];
        if (isRegistered) {
            return;
        }

        _customContainers[serviceType] = (container, task, true);
        await _group.RegisterAndBroadcast(container);
    }

    public async Task<Type> AddRun(IRegistrableContainer container) {
        var res = Add(container);

        await Run(container.ServiceAttributeType.serviceType);

        return res;
    }

    // be careful, sometimes if you stop a service and then write to input, but immediately restart it,
    // the stop function may not work,
    // this is because the input is async, so before the service get input message, the restart is already executed.
    public async Task Stop(Type serviceType) {
        var (container, task, isRegistered) = _customContainers[serviceType];
        if (!isRegistered) {
            return;
        }

        _customContainers[serviceType] = (container, task, false);
        await _group.UnregisterAndBroadcast(container);
    }

    public void Close() {
        _cancelSource.Cancel();
        Task.WaitAll(_defaultServiceTask.ToArray());
    }

    public void StopAll() {
        _customContainers.ToList().ForEach(kvp => Stop(kvp.Key).Wait());
    }

    public void RunAll() {
        _customContainers.ToList().ForEach(kvp => Run(kvp.Key).Wait());
    }


    public Task<(Type, InputHandler<TInputData>)> CreateAddRunPollingInputWithoutSharedData<TService, TInputData>()
        where TService : BaseInputService<object>, new() {
        return CreateAddRunPollingInput<TService, TInputData, object>();
    }

    public async Task<(Type, InputHandler<TInputData>)> CreateAddRunPollingInput<TService, TInputData, TSharedData>()
        where TService : BaseInputService<TSharedData>, new() where TSharedData : new() {
        var inputHandler = new InputHandler<TInputData>();

        var inputContainer = new InputContainer<TService, TInputData, TSharedData>(
            ConcurrencyLevel,
            () => new PollingDistributor<InputPackagedData, NullSubjectData, InitTuple<Register, TSharedData>>(),
            inputHandler);
        var serviceType = await AddRun(inputContainer);

        return (serviceType, inputHandler);
    }


    public Task<Type> CreateAddRunPollingProcessorWithoutSharedData<TService, TServiceData>()
        where TService : BaseProcessService<TServiceData, object>, new()
        where TServiceData : IProcessorData {
        return CreateAddRunPollingProcessor<TService, TServiceData, object>();
    }

    public async Task<Type> CreateAddRunPollingProcessor<TService, TServiceData, TSharedData>()
        where TService : BaseProcessService<TServiceData, TSharedData>, new()
        where TServiceData : IProcessorData
        where TSharedData : new() {
        var processorContainer = new ProcessorContainer<TService, TServiceData, TSharedData>(
            ConcurrencyLevel,
            () => new PollingDistributor<TServiceData, SubjectDataWithCommand, InitTuple<Register, TSharedData>>());
        var serviceType = await AddRun(processorContainer);

        return serviceType;
    }

    public Task<Type> CreateAddRunPollingExecutorWithoutSharedData<TService, TServiceData>()
        where TService : BaseExecutorService<TServiceData, object>, new()
        where TServiceData : AExecutorData {
        return CreateAddRunPollingExecutor<TService, TServiceData, object>();
    }

    public async Task<Type> CreateAddRunPollingExecutor<TService, TServiceData, TSharedData>()
        where TService : BaseExecutorService<TServiceData, TSharedData>, new()
        where TServiceData : AExecutorData
        where TSharedData : new() {
        var executorContainer = new ExecutorContainer<TService, TServiceData, TSharedData>(
            ConcurrencyLevel,
            () => new PollingDistributor<TServiceData, NullSubjectData, InitTuple<TSharedData>>());
        var serviceType = await AddRun(executorContainer);

        return serviceType;
    }
}