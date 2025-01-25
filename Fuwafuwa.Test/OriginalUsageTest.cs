using Fuwafuwa.Core.Container.Level3;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Distributor.Implement;
using Fuwafuwa.Core.ServiceRegister;
using Fuwafuwa.Test.TestImplement.Data;
using Fuwafuwa.Test.TestImplement.Executor;
using Fuwafuwa.Test.TestImplement.Input;
using Fuwafuwa.Test.TestImplement.Processor;

namespace Fuwafuwa.Test;

public class OriginalUsageTest {
    private const int ConcurrencyLevel = 10;

    private CancellationTokenSource _cancelSource;


    private ServiceRegisterGroup _group;

    private InputContainer<StringInput, string, object> _inputContainer;

    private InputHandler<string> _inputHandler;

    private SubjectBufferContainer _subjectBufferContainer;

    private TaskAgentContainer _taskAgentContainer;

    private List<Task> _tasks;

    [SetUp]
    public async Task Setup() {
        _inputHandler = new InputHandler<string>();

        _inputContainer = new InputContainer<StringInput, string, object>(
            ConcurrencyLevel,
            () => new PollingDistributor<InputPackagedData, NullSubjectData, InitTuple<Register, object>>(),
            _inputHandler);

        _subjectBufferContainer = new SubjectBufferContainer(ConcurrencyLevel,
            () => new HashDistributor<NullServiceData, SubjectData, InitTuple<Register, object>>());

        _taskAgentContainer = new TaskAgentContainer(ConcurrencyLevel,
            () => new PollingDistributor<TaskAgentData, NullSubjectData, InitTuple<Register, object>>());

        _cancelSource = new CancellationTokenSource();

        _tasks = [
            _inputContainer.Run(_cancelSource.Token),
            _subjectBufferContainer.Run(_cancelSource.Token),
            _taskAgentContainer.Run(_cancelSource.Token)
        ];

        _group = new ServiceRegisterGroup();

        await _group.RegisterAndBroadcast(_inputContainer);
        await _group.RegisterAndBroadcast(_subjectBufferContainer);

        await _group.RegisterAndBroadcast(_taskAgentContainer);
    }

    [TearDown]
    public void TearDown() {
        _cancelSource.Cancel();
        Task.WaitAll(_tasks.ToArray());
        _cancelSource.Dispose();
    }

    [Test]
    public async Task TestOneProcessor() {
        var processorContainer = new ProcessorContainer<StringProcessor, StringData, object>(
            ConcurrencyLevel,
            () => new PollingDistributor<StringData, SubjectDataWithCommand, InitTuple<Register, object>>());
        _tasks.Add(processorContainer.Run(_cancelSource.Token));
        await _group.RegisterAndBroadcast(processorContainer);

        var executorContainer = new ExecutorContainer<WriteToConsoleExecutor, WriteToConsoleData, object>(
            ConcurrencyLevel,
            () => new PollingDistributor<WriteToConsoleData, NullSubjectData, InitTuple<object>>());
        _tasks.Add(executorContainer.Run(_cancelSource.Token));
        await _group.RegisterAndBroadcast(executorContainer);


        for (var i = 0; i < 5; i++) {
            await _inputHandler.Input($"Hello World!:[{i}]");

            await Task.Delay(100);
        }
    }

    [Test]
    public async Task TestTwoProcessor() {
        var processorContainer = new ProcessorContainer<StringProcessor, StringData, object>(
            ConcurrencyLevel,
            () => new PollingDistributor<StringData, SubjectDataWithCommand, InitTuple<Register, object>>());
        _tasks.Add(processorContainer.Run(_cancelSource.Token));
        await _group.RegisterAndBroadcast(processorContainer);


        var anotherProcessorContainer = new ProcessorContainer<AnotherStringProcessor, StringData, object>(
            ConcurrencyLevel,
            () => new PollingDistributor<StringData, SubjectDataWithCommand, InitTuple<Register, object>>());
        _tasks.Add(anotherProcessorContainer.Run(_cancelSource.Token));
        await _group.RegisterAndBroadcast(anotherProcessorContainer);


        var executorContainer = new ExecutorContainer<WriteToConsoleExecutor, WriteToConsoleData, object>(
            ConcurrencyLevel,
            () => new PollingDistributor<WriteToConsoleData, NullSubjectData, InitTuple<object>>());
        _tasks.Add(executorContainer.Run(_cancelSource.Token));
        await _group.RegisterAndBroadcast(executorContainer);


        for (var i = 0; i < 5; i++) {
            await _inputHandler.Input($"Hello World!:[{i}]");

            await Task.Delay(100);
        }
    }
}