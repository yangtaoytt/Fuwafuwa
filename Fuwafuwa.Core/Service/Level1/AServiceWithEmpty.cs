using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.Service.Level1;

public abstract class
    AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService> :
    AService<TServiceCore, TServiceData, TSubjectData, ValueTuple<TSharedData>, TInitData,
        AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>,
        TResService>,
    IService<AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>,
        ValueTuple<TSharedData>, TInitData, TResService> where TServiceData : IServiceData
    where TServiceCore : IServiceCore<TServiceData>, new()
    where TSubjectData : ISubjectData
    where TService : class, IService<TService, TSharedData, TInitData, TResService> {
    protected AServiceWithEmpty(Logger2Event? logger) : base(logger) { }

    public static TResService CreateService(Logger2Event? logger,
        AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>?
            uniqueService = null) {
        return TService.CreateService(logger);
    }

    public static void Final(ValueTuple<TSharedData> sharedData, Logger2Event? logger,
        AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>?
            uniqueService = null) {
        TService.Final(sharedData.Item1, logger);
    }

    public static ValueTuple<TSharedData> InitService(TInitData initData,
        AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>?
            uniqueService = null) {
        return new ValueTuple<TSharedData>(TService.InitService(initData));
    }

    protected sealed override Task ProcessData(TServiceData serviceData, TSubjectData subjectData,
        ValueTuple<TSharedData> sharedData) {
        return ProcessData(serviceData, subjectData, sharedData.Item1);
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData, TSharedData sharedData);
}