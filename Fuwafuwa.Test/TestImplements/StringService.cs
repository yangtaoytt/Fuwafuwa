using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.New.Data;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

public class StringService : ServiceWithRegister<StringService>,
    N_IProcessorHandler<StringService, StringProcessorData, StringProcessorData>,
    N_ICustomerHandler<StringService, StringConsumerData> {
    
    private RegisterBuffer<WriteToTestChannelService> _writeToTestChannelService;

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