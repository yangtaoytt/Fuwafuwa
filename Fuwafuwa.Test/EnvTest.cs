using Fuwafuwa.Core.Data.SharedDataWrapper.Level2;
using Fuwafuwa.Core.Env;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Log.LogEventArgs.Interface;
using Fuwafuwa.Test.TestImplement.Data;
using Fuwafuwa.Test.TestImplement.ServiceCore.Executor;
using Fuwafuwa.Test.TestImplement.ServiceCore.Input;
using Fuwafuwa.Test.TestImplement.ServiceCore.Processor;

namespace Fuwafuwa.Test;

public class EnvTest {
    private const int ConcurrencyLevel = 10;
    private const int NameMaxWidth = 25; // > 3
    private const int ModuleMaxWidth = 12; // > 3
    private Env _env;

    private static void OutputHandler(object? sender, BaseLogEventArgs args) {
        var source = sender?.GetType().Name ?? "Unknown";
        var name = source.Length > NameMaxWidth ? "..." + source.Substring(source.Length - NameMaxWidth + 3) : source;
        var module = source.Substring(source.LastIndexOf(source.Last(char.IsUpper)));
        var res = module.Length > ModuleMaxWidth
            ? "..." + module.Substring(module.Length - ModuleMaxWidth + 3)
            : module;
        TestContext.Progress.WriteLine($"[{name,-NameMaxWidth}]({res,-ModuleMaxWidth}):{args.Message}");
    }

    [SetUp]
    public void Setup() {
        Logger2Event logger = new();
        // logger.DebugLogGenerated += OutputHandler;
        logger.ErrorLogGenerated += OutputHandler;
        logger.InfoLogGenerated += OutputHandler;
        _env = new Env(ConcurrencyLevel, logger);
    }

    [TearDown]
    public void TearDown() {
        _env.Close();
    }


    [Test]
    public async Task TestOneProcessor() {
        var (inputType, inputHandler) =
            await _env.CreateRunRegisterPollingInput<StringInput, string, NullSharedDataWrapper<object>, object>(
                new object());

        var processorType =
            await _env
                .CreateRunRegisterPollingProcessor<StringProcessor, StringData, NullSharedDataWrapper<object>, object>(
                    new object());
        var executorType =
            await _env
                .CreateRunRegisterPollingExecutor<WriteToConsoleExecutor, WriteToConsoleData,
                    NullSharedDataWrapper<object>, object>(new object());

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");

            await Task.Delay(100);
        }

        await Task.Delay(1000);
    }


    [Test]
    public async Task TestTwoProcessorQuick() {
        var (inputType, inputHandler) =
            await _env.CreateRunRegisterPollingInput<StringInput, string, NullSharedDataWrapper<object>, object>(
                new object());

        var processor1Type =
            await _env
                .CreateRunRegisterPollingProcessor<StringProcessor, StringData, NullSharedDataWrapper<object>, object>(
                    new object());
        var processor2Type =
            await _env
                .CreateRunRegisterPollingProcessor<AnotherStringProcessor, StringData, NullSharedDataWrapper<object>,
                    object>(new object());
        var executorType =
            await _env
                .CreateRunRegisterPollingExecutor<WriteToConsoleExecutor, WriteToConsoleData,
                    NullSharedDataWrapper<object>, object>(new object());

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(1000);
    }

    [Test]
    public async Task TestServiceUnregister() {
        var (inputType, inputHandler) =
            await _env.CreateRunRegisterPollingInput<StringInput, string, NullSharedDataWrapper<object>, object>(
                new object());

        var processor1Type =
            await _env
                .CreateRunRegisterPollingProcessor<StringProcessor, StringData, NullSharedDataWrapper<object>, object>(
                    new object());
        var processor2Type =
            await _env
                .CreateRunRegisterPollingProcessor<AnotherStringProcessor, StringData, NullSharedDataWrapper<object>,
                    object>(new object());
        var executorType =
            await _env
                .CreateRunRegisterPollingExecutor<WriteToConsoleExecutor, WriteToConsoleData,
                    NullSharedDataWrapper<object>, object>(new object());

        await inputHandler.Input("Hello World!:[0]");

        await Task.Delay(100);
        await _env.UnRegister(processor2Type);
        await inputHandler.Input("Hello World!:[1]");

        await Task.Delay(100);
        await _env.Register(processor2Type);
        await inputHandler.Input("Hello World!:[2]");

        await Task.Delay(100);
    }

    [Test]
    public async Task TestServiceUnregisterAll() {
        var (inputType, inputHandler) =
            await _env.CreateRunRegisterPollingInput<StringInput, string, NullSharedDataWrapper<object>, object>(
                new object());

        var processor1Type =
            await _env
                .CreateRunRegisterPollingProcessor<StringProcessor, StringData, NullSharedDataWrapper<object>, object>(
                    new object());
        var processor2Type =
            await _env
                .CreateRunRegisterPollingProcessor<AnotherStringProcessor, StringData, NullSharedDataWrapper<object>,
                    object>(new object());
        var executorType =
            await _env
                .CreateRunRegisterPollingExecutor<WriteToConsoleExecutor, WriteToConsoleData,
                    NullSharedDataWrapper<object>, object>(new object());

        await inputHandler.Input("Hello World!:[0]");

        await Task.Delay(100);
        await _env.UnRegisterAll();
        await inputHandler.Input("Hello World!:[1]");

        await Task.Delay(100);
        await _env.RegisterAll();
        await inputHandler.Input("Hello World!:[2]");

        await Task.Delay(100);
    }


    // whether or not the key is to make sure one thread can not make other thread to wait for it forever, when the thread throw exception 
    [Test]
    [Repeat(300)]
    public async Task TestDeadLock() {
        await TestContext.Progress.WriteLineAsync("Start");

        var (inputType, inputHandler) =
            await _env.CreateRunRegisterPollingInput<StringInput, string, NullSharedDataWrapper<object>, object>(
                new object());

        var processorType =
            await _env
                .CreateRunRegisterPollingProcessor<StringProcessor, StringData, NullSharedDataWrapper<object>, object>(
                    new object());
        var executorType =
            await _env
                .CreateRunRegisterPollingExecutor<WriteToConsoleExecutor, WriteToConsoleData,
                    NullSharedDataWrapper<object>, object>(new object());

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");
        }

        await Task.Delay(50);
    }


    [Test]
    [Repeat(10)]
    public async Task TestSyncSharedData() {
        var (inputType, inputHandler) =
            await _env.CreateRunRegisterPollingInput<StringInput, string, NullSharedDataWrapper<object>, object>(
                new object());

        // var processorType = await _env.CreateRunRegisterPollingProcessor<TestSyncProcessor<NullSharedDataWrapper<bool>>, StringData, NullSharedDataWrapper<bool>, object>(new object());
        var processorType =
            await _env
                .CreateRunRegisterPollingProcessor<TestSyncProcessor<SimpleSharedDataWrapper<bool>>, StringData,
                    SimpleSharedDataWrapper<bool>, object>(new object());
        var executorType =
            await _env
                .CreateRunRegisterPollingExecutor<WriteToConsoleExecutor, WriteToConsoleData,
                    NullSharedDataWrapper<object>, object>(new object());

        for (var i = 0; i < 5; i++) {
            await inputHandler.Input($"Hello World!:[{i}]");

            await Task.Delay(3000);
        }

        await Task.Delay(2000);
    }
}