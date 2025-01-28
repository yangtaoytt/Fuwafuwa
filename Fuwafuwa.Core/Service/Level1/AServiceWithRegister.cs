using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Service.Level1;

public abstract class
    AServiceWithRegister<TServiceData, TSubjectData, TSharedData> : AService<TServiceData, TSubjectData, InitTuple<
    Register,
    TSharedData>> where TServiceData : IServiceData where TSubjectData : ISubjectData where TSharedData : new() {
    protected override Task ProcessData(TServiceData serviceData, TSubjectData subjectData,
        InitTuple<Register, TSharedData> initTuple, Logger2Event? logger) {
        return ProcessData(serviceData, subjectData, initTuple.Item1, initTuple.Item2 , logger);
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData, Register register,
        TSharedData sharedData , Logger2Event? logger);
}