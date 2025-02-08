using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level2;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.ServiceCore.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Service.Level1;

public abstract class
    AServiceWithRegister<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService> :
    AService<TServiceCore,
        TServiceData, TSubjectData, (SimpleSharedDataWrapper<Register>, TSharedData),
        (SimpleSharedDataWrapper<Register>, TInitData),
        AServiceWithRegister<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>,
        TResService>,
    IService<
        AServiceWithRegister<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>, (
        SimpleSharedDataWrapper<Register>, TSharedData), (SimpleSharedDataWrapper<Register>, TInitData), TResService>
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TServiceCore : IServiceCore<TServiceData>, new()
    where TService : class, IService<TService, TSharedData, TInitData, TResService> {
    protected AServiceWithRegister(Logger2Event? logger) : base(logger) { }

    public static TResService CreateService(Logger2Event? logger,
        AServiceWithRegister<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>?
            uniqueService = null) {
        return TService.CreateService(logger);
    }

    public static void Final((SimpleSharedDataWrapper<Register>, TSharedData) sharedData, Logger2Event? logger,
        AServiceWithRegister<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>?
            uniqueService = null) {
        TService.Final(sharedData.Item2, logger);
    }

    public static (SimpleSharedDataWrapper<Register>, TSharedData) InitService(
        (SimpleSharedDataWrapper<Register>, TInitData) initData,
        AServiceWithRegister<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>?
            uniqueService = null) {
        return (new SimpleSharedDataWrapper<Register>(new Register()), TService.InitService(initData.Item2));
    }

    protected sealed override Task ProcessData(TServiceData serviceData, TSubjectData subjectData,
        (SimpleSharedDataWrapper<Register>, TSharedData) sharedData) {
        CreateService(null);
        return ProcessData(serviceData, subjectData, sharedData.Item1, sharedData.Item2);
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData,
        SimpleSharedDataWrapper<Register> register,
        TSharedData sharedData);
}