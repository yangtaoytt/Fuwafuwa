using System.Threading.Channels;
using Fuwafuwa.Core.Core.RegisterService.ServiceWithRegisterHandler;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Handle;
using Fuwafuwa.Core.Core.Service.Others.Distributor;
using Fuwafuwa.Core.Core.Service.ServiceStrategry;

namespace Fuwafuwa.Test.TestImplements;

internal class WriteToTestChannelService : ServiceWithRegister<WriteToTestChannelService>, ICustomerHandler<WriteToTestChannelService, WriteToTestChannelConsumerData> {
    public override WriteToTestChannelService Implement() {
        return this;
    }
    public WriteToTestChannelService(ushort threadNumber, Channel<string> channel) : 
        base(new StaticThreadStrategy<WriteToTestChannelService>(threadNumber)) {
        _channel = channel;
    }
    public WriteToTestChannelService(Channel<string> channel) :
        base(new DynamicThreadStrategy<WriteToTestChannelService>()) {
        _channel = channel;
    }

    private readonly Channel<string> _channel;
    public void Handle(WriteToTestChannelConsumerData data) {
        var result = data.Data;
        _channel.Writer.TryWrite(result);
    }
}

internal class WriteToTestChannelConsumerData : AConsumerData<WriteToTestChannelConsumerData,WriteToTestChannelService> {
    
    public string Data { get; }
    public WriteToTestChannelConsumerData(string data) : base(new PollingDistributor()) {
        Data = data;
    }
    public override WriteToTestChannelConsumerData Implement() {
        return this;
    }
}
