using Fuwafuwa.Core.Core.RegisterService.Register;
using Fuwafuwa.Core.Core.RegisterService.ServiceWithRegisterHandler;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Handle;
using Fuwafuwa.Core.Core.Service.Others.Distributor;

namespace Fuwafuwa.Test.TestImplements;

public class StringService : ServiceWithRegister<StringService>,
    IProcessorHandler<StringService, StringProcessorData, StringProcessorData>,
    ICustomerHandler<StringService, StringConsumerData> {
    
    private readonly RegisterBuffer<WriteToTestChannelService> _writeToTestChannelService;

    public StringService(ushort threadNumber) : base(threadNumber) {
        _writeToTestChannelService = Register.CreateRegisterBuffer<WriteToTestChannelService>();
    }
    public override StringService Implement() {
        return this;
    }

    public StringProcessorData Handle(StringProcessorData data) {
        return new StringProcessorData(data.StringData + "[processed]");
    }

    public void Handle(StringConsumerData data) {
        new WriteToTestChannelConsumerData(data.StringData + "[processed]").Send(_writeToTestChannelService
            .GetService()!);
    }
}

public class StringProcessorData : AProcessorData<StringProcessorData,StringProcessorData,StringService> {
    public string StringData { get; }
    
    public StringProcessorData(string stringData) : base(new PollingDistributor()) {
        StringData = stringData;
    }
    public override StringProcessorData Implement() {
        return this;
    }
}

public class StringConsumerData : AConsumerData<StringConsumerData,StringService> {
    public StringConsumerData(string stringData) : base(new PollingDistributor()) {
        StringData = stringData;
    }
    public string StringData { get; }


    public override StringConsumerData Implement() {
        return this;
    }
}