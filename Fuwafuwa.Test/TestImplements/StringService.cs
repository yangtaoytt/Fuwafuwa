using Fuwafuwa.Core.Core.RegisterService.Register;
using Fuwafuwa.Core.Core.RegisterService.ServiceWithRegisterHandler;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Handle;
using Fuwafuwa.Core.Core.Service.Others.Distributor;
using Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

namespace Fuwafuwa.Test.TestImplements;

public class StringService : ServiceWithRegister<StringService>,
    IProcessorHandler<StringService, StringProcessorData, StringProcessorData>,
    ICustomerHandler<StringService, StringConsumerData> {
    private readonly RegisterBuffer<WriteToTestChannelService> _writeToTestChannelService;

    public StringService(ushort threadNumber) : base(new StaticThreadStrategy<StringService>(threadNumber)) {
        _writeToTestChannelService = Register.CreateRegisterBuffer<WriteToTestChannelService>();
    }

    public StringService(IServiceStrategy<StringService> serviceStrategy) : base(serviceStrategy) {
        _writeToTestChannelService = Register.CreateRegisterBuffer<WriteToTestChannelService>();
    }

    public StringService() : base(new DynamicThreadStrategy<StringService>()) {
        _writeToTestChannelService = Register.CreateRegisterBuffer<WriteToTestChannelService>();
    }

    public void Handle(StringConsumerData data) {
        _writeToTestChannelService.Execute(service => {
            if (service != null) {
                new WriteToTestChannelConsumerData(data.StringData + "[processed]").Send(service!);
            }
            
        });
    }

    public StringProcessorData Handle(StringProcessorData data) {
        return new StringProcessorData(data.StringData + "[processed]");
    }

    public override StringService Implement() {
        return this;
    }
}

public class StringProcessorData : AProcessorData<StringProcessorData, StringProcessorData, StringService> {
    public StringProcessorData(string stringData) : base(new PollingDistributor()) {
        StringData = stringData;
    }

    public string StringData { get; }

    public override StringProcessorData Implement() {
        return this;
    }
}

public class StringConsumerData : AConsumerData<StringConsumerData, StringService> {
    public StringConsumerData(string stringData) : base(new PollingDistributor()) {
        StringData = stringData;
    }

    public string StringData { get; }


    public override StringConsumerData Implement() {
        return this;
    }
}