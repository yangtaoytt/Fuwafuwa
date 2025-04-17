using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.New.Data;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

class WriteToTestChannelService : ServiceWithRegister<WriteToTestChannelService>, N_ICustomerHandler<WriteToTestChannelService, WriteToTestChannelConsumerData> {
    public override WriteToTestChannelService Implement() {
        return this;
    }
    public WriteToTestChannelService(ushort threadNumber, Channel<string> channel) : base(threadNumber) {
        _channel = channel;
    }

    private readonly Channel<string> _channel;
    public void Handle(WriteToTestChannelConsumerData data) {
        var result = data.Data;
        _channel.Writer.TryWrite(result);
    }
}

class WriteToTestChannelConsumerData : AConsumerData<WriteToTestChannelConsumerData,WriteToTestChannelService> {
    
    public string Data { get; }
    public WriteToTestChannelConsumerData(string data) : base(new PollingDistributor()) {
        Data = data;
    }
    public override WriteToTestChannelConsumerData Implement() {
        return this;
    }
}
