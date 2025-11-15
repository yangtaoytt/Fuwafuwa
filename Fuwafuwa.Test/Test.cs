using System.Threading.Channels;
using Fuwafuwa.Core.Core.RegisterService.ServiceWithRegisterHandler;
using Fuwafuwa.Core.Logger;
using Fuwafuwa.Test.TestImplements;

namespace Fuwafuwa.Test;

public class Test {
    [SetUp]
    public void Setup() {
        Logger2Event.Instance.WarningLogGenerated += (sender, args) => {
            TestContext.Progress.WriteLine("Warn:"+args.Message);
        };
        Logger2Event.Instance.DebugLogGenerated += (sender, args) => {
            TestContext.Progress.WriteLine("Debug:"+args.Message);
        };
        Logger2Event.Instance.ErrorLogGenerated += (sender, args) => {
            TestContext.Progress.WriteLine("Error:" + args.Message);
        };
        Logger2Event.Instance.InfoLogGenerated += (sender, args) => {
            TestContext.Progress.WriteLine("Info:"+args.Message);
        };
    }

    [TearDown]
    public void TearDown() {

    }

    [Test]
    public async Task TestSimpleCallCustomer() {
        var channel = Channel.CreateUnbounded<string>();
        var testChannelService = new WriteToTestChannelService(1, channel).Start();
        
        new WriteToTestChannelConsumerData("Test").Send(testChannelService);
        
        await foreach (var result in channel.Reader.ReadAllAsync()) {
            Assert.That(result, Is.EqualTo("Test"));
            break;
        }
    }

    [Test]
    public async Task TestSimpleCallProcessor() {
        var channel = Channel.CreateUnbounded<string>();
        var testChannelService = new WriteToTestChannelService(1, channel).Start();
        
        
        var stringService = new StringService(1).Start();
        var data = new StringProcessorData("Test");
        new WriteToTestChannelConsumerData((await data.Send(stringService)).StringData).Send(testChannelService);
        
        await foreach (var result in channel.Reader.ReadAllAsync()) {
            Assert.That(result, Is.EqualTo("Test[processed]"));
            break;
        }
    }

    [Test]
    public async Task TestSimpleCallRegisterService() {
        var channel = Channel.CreateUnbounded<string>();
        var testChannelService = new WriteToTestChannelService(1, channel).Start();
        var stringService = new StringService(1).Start();

        var registerHandler = new ServiceRegisterManageHandler();
        await registerHandler.AddServiceAsync(testChannelService);
        await registerHandler.AddServiceAsync(stringService);
        
        
        new StringConsumerData("Test").Send(stringService);
        
        await foreach (var result in channel.Reader.ReadAllAsync()) {
            Assert.That(result, Is.EqualTo("Test[processed]"));
            break;
        }
    }
    
    
    [Test]
    public async Task TestMultThreadCallRegisterService() {
        const ushort threadNumber = 10;
        
        var channel = Channel.CreateUnbounded<string>();
        var testChannelService = new WriteToTestChannelService(threadNumber, channel).Start();
        var stringService = new StringService(threadNumber).Start();

        var registerHandler = new ServiceRegisterManageHandler();
        await registerHandler.AddServiceAsync(testChannelService);
        await registerHandler.AddServiceAsync(stringService);
        
        
        new StringConsumerData("Test").Send(stringService);
        
        await foreach (var result in channel.Reader.ReadAllAsync()) {
            Assert.That(result, Is.EqualTo("Test[processed]"));
            break;
        }
    }
    
    [Test]
    public async Task TestConcurrentCallRegisterService() {
        const ushort threadNumber = 10;
        
        var channel = Channel.CreateUnbounded<string>();
        var testChannelService = new WriteToTestChannelService(threadNumber, channel).Start();
        var stringService = new StringService(threadNumber).Start();

        var registerHandler = new ServiceRegisterManageHandler();
        await registerHandler.AddServiceAsync(testChannelService);
        await registerHandler.AddServiceAsync(stringService);
        
        
        for (var i = 0;i < 100; i++) {
            new StringConsumerData($"Test({i})" ).Send(stringService);
        }

        var resultSet = new HashSet<string>();
        await foreach (var result in channel.Reader.ReadAllAsync()) {
            resultSet.Add(result);
            if (resultSet.Count == 100) {
                break;
            }
        }
        
        for (var i = 0; i < 100; i++) {
            Assert.That(resultSet.Contains($"Test({i})[processed]"), Is.True);
        }
    }
    
    [Test]
    public async Task TestDynamicConcurrentCallRegisterService() {
        
        var channel = Channel.CreateUnbounded<string>();
        var testChannelService = new WriteToTestChannelService(channel).Start();
        var stringService = new StringService().Start();

        var registerHandler = new ServiceRegisterManageHandler();
        await registerHandler.AddServiceAsync(testChannelService);
        await registerHandler.AddServiceAsync(stringService);
        
        
        for (var i = 0;i < 100; i++) {
            new StringConsumerData($"Test({i})" ).Send(stringService);
        }

        var resultSet = new HashSet<string>();
        await foreach (var result in channel.Reader.ReadAllAsync()) {
            resultSet.Add(result);
            if (resultSet.Count == 100) {
                break;
            }
        }
        
        for (var i = 0; i < 100; i++) {
            Assert.That(resultSet.Contains($"Test({i})[processed]"), Is.True);
        }
    }

}