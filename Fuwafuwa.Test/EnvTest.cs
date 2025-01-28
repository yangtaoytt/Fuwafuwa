using Fuwafuwa.Core.Env;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Test.TestImplement.Data;
using Fuwafuwa.Test.TestImplement.Executor;
using Fuwafuwa.Test.TestImplement.Input;
using Fuwafuwa.Test.TestImplement.Processor;

namespace Fuwafuwa.Test;

public class EnvTest {
    private const int ConcurrencyLevel = 10;

    private Env _env;

    [SetUp]
    public void Setup() {
        TestContext.Progress.WriteLine("Setup");
        Logger2Event logger = new();
        logger.DebugLogGenerated += (sender, args) => {
            TestContext.Progress.WriteLine(args.Message);
        };
        logger.ErrorLogGenerated += (sender, args) => {
            TestContext.Progress.WriteLine(args.Message);
        };
        _env = new Env(ConcurrencyLevel, logger);
    }

    [TearDown]
    public void TearDown() {
        TestContext.Progress.WriteLine("TearDown");
        _env.Close();
    }
    
    // whether or not the key is to make sure one thread can not make other thread to wait for it forever, when the thread throw exception 
    [Test]
    [Repeat(3000)]
    public async Task TestDeadLock() {
        var (inputType, inputHandler) = await _env.CreateRunRegisterPollingInputWithoutSharedData<StringInput, string>();

        var processorType = await _env.CreateRunRegisterPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var executorType =
            await _env.CreateRunRegisterPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }
    }
    
    [Test]
    public async Task TestOneProcessor() {
        var (inputType, inputHandler) = await _env.CreateRunRegisterPollingInputWithoutSharedData<StringInput, string>();

        var processorType = await _env.CreateRunRegisterPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var executorType =
            await _env.CreateRunRegisterPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");

            await Task.Delay(100);
        }
    }

    [Test]
    public async Task TestTwoProcessor() {
        var (inputType, inputHandler) = await _env.CreateRunRegisterPollingInputWithoutSharedData<StringInput, string>();

        var processor1Type = await _env.CreateRunRegisterPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var processor2Type =
            await _env.CreateRunRegisterPollingProcessorWithoutSharedData<AnotherStringProcessor, StringData>();
        var executorType =
            await _env.CreateRunRegisterPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");

            await Task.Delay(100);
        }
    }

    [Test]
    public async Task TestTwoProcessorQuick() {
        var (inputType, inputHandler) = await _env.CreateRunRegisterPollingInputWithoutSharedData<StringInput, string>();

        var processor1Type = await _env.CreateRunRegisterPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var processor2Type =
            await _env.CreateRunRegisterPollingProcessorWithoutSharedData<AnotherStringProcessor, StringData>();
        var executorType =
            await _env.CreateRunRegisterPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(1000);
    }

    [Test]
    [Repeat(30)]
    public async Task TestServiceStop() {
        var (inputType, inputHandler) = await _env.CreateRunRegisterPollingInputWithoutSharedData<StringInput, string>();

        var processor1Type = await _env.CreateRunRegisterPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var processor2Type =
            await _env.CreateRunRegisterPollingProcessorWithoutSharedData<AnotherStringProcessor, StringData>();
        var executorType =
            await _env.CreateRunRegisterPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 1; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(100);

        await _env.UnRegister(processor2Type);

        for (var i = 1; i < 2; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(100);

        await _env.Register(processor2Type);
        for (var i = 2; i < 3; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(100);
    }
}