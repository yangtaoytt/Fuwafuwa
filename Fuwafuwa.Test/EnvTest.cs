using Fuwafuwa.Core.Env;
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
        _env = new Env(ConcurrencyLevel);
    }

    [TearDown]
    public void TearDown() {
        _env.Close();
    }

    [Test]
    public async Task TestOneProcessor() {
        var (inputType, inputHandler) = await _env.CreateAddRunPollingInputWithoutSharedData<StringInput, string>();

        var processorType = await _env.CreateAddRunPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var executorType =
            await _env.CreateAddRunPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");

            await Task.Delay(100);
        }
    }

    [Test]
    public async Task TestTwoProcessor() {
        var (inputType, inputHandler) = await _env.CreateAddRunPollingInputWithoutSharedData<StringInput, string>();

        var processor1Type = await _env.CreateAddRunPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var processor2Type =
            await _env.CreateAddRunPollingProcessorWithoutSharedData<AnotherStringProcessor, StringData>();
        var executorType =
            await _env.CreateAddRunPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");

            await Task.Delay(100);
        }
    }

    [Test]
    public async Task TestTwoProcessorQuick() {
        var (inputType, inputHandler) = await _env.CreateAddRunPollingInputWithoutSharedData<StringInput, string>();

        var processor1Type = await _env.CreateAddRunPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var processor2Type =
            await _env.CreateAddRunPollingProcessorWithoutSharedData<AnotherStringProcessor, StringData>();
        var executorType =
            await _env.CreateAddRunPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(1000);
    }

    [Test]
    public async Task TestServiceStop() {
        var (inputType, inputHandler) = await _env.CreateAddRunPollingInputWithoutSharedData<StringInput, string>();

        var processor1Type = await _env.CreateAddRunPollingProcessorWithoutSharedData<StringProcessor, StringData>();
        var processor2Type =
            await _env.CreateAddRunPollingProcessorWithoutSharedData<AnotherStringProcessor, StringData>();
        var executorType =
            await _env.CreateAddRunPollingExecutorWithoutSharedData<WriteToConsoleExecutor, WriteToConsoleData>();

        for (var i = 0; i < 1; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(1000);

        await _env.Stop(processor2Type);

        for (var i = 1; i < 2; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(1000);

        await _env.Run(processor2Type);
        for (var i = 2; i < 3; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(1000);
    }
}