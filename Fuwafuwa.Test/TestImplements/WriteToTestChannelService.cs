using System.Threading.Channels;
using Fuwafuwa.Core.Core.RegisterService.ServiceWithRegisterHandler;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Handle;
using Fuwafuwa.Core.Core.Service.Others.Distributor;
using Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

namespace Fuwafuwa.Test.TestImplements;

internal class WriteToTestChannelService : ServiceWithRegister<WriteToTestChannelService>,
    ICustomerHandler<WriteToTestChannelService, WriteToTestChannelConsumerData> {
    private readonly Channel<string> _channel;

    private bool _isOpen = true;
    private Lock _lock = new Lock();
    public bool IsOpen {
        set {
            lock (_lock) {
                _isOpen = value;
            }
        }
        get {
            lock (_lock) {
                return _isOpen;
            }
        }
    }

    public WriteToTestChannelService(ushort threadNumber, Channel<string> channel) :
        base(new StaticThreadStrategy<WriteToTestChannelService>(threadNumber)) {
        _channel = channel;
    }

    public WriteToTestChannelService(Channel<string> channel, IServiceStrategy<WriteToTestChannelService> strategy) :
        base(strategy) {
        _channel = channel;
    }

    public WriteToTestChannelService(Channel<string> channel) :
        base(new DynamicThreadStrategy<WriteToTestChannelService>()) {
        _channel = channel;
    }

    public void Handle(WriteToTestChannelConsumerData data) {
        var result = data.Data;
        if (IsOpen) {
            _channel.Writer.TryWrite(result);
        }
    }

    public override WriteToTestChannelService Implement() {
        return this;
    }
}

internal class
    WriteToTestChannelConsumerData : AConsumerData<WriteToTestChannelConsumerData, WriteToTestChannelService> {
    public WriteToTestChannelConsumerData(string data) : base(new PollingDistributor()) {
        Data = data;
    }

    public string Data { get; }

    public override WriteToTestChannelConsumerData Implement() {
        return this;
    }
}