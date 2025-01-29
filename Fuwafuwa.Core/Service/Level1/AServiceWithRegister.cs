using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.ServiceCore.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Service.Level1;

public abstract class
    AServiceWithRegister<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData> : AService<TServiceCore,
    TServiceData, TSubjectData, InitTuple<
        Register,
        TSharedData>, (Register, TInitData)> where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TSharedData : new()
    where TServiceCore : IServiceCore<TServiceData>, new() {
    protected override Task ProcessData(TServiceData serviceData, TSubjectData subjectData,
        InitTuple<Register, TSharedData> initTuple, Lock sharedDataLock) {
        return ProcessData(serviceData, subjectData, initTuple.Item1, initTuple.Item2, sharedDataLock);
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData, Register register,
        TSharedData sharedData,Lock sharedDataLock);

    protected override InitTuple<Register, TSharedData> Init((Register, TInitData) initData) {
        return new InitTuple<Register, TSharedData>(initData.Item1, SubInit(initData.Item2));
    }

    protected abstract TSharedData SubInit(TInitData initData);
}