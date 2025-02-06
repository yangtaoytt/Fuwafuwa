using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SharedDataWapper.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level0;
using Fuwafuwa.Core.ServiceCore.Level2;

namespace Fuwafuwa.Core.Service.Interface;


// ReSharper disable TypeParameterCanBeVariant
// intended to disable the warning of TypeParameterCanBeVariant
public interface IPrimitiveService<TUniqueService,TSharedData, in TInitData, out TResService> 
    // ReSharper restore TypeParameterCanBeVariant
    where TUniqueService : class {
    public static abstract TResService CreateService(Logger2Event? logger, TUniqueService? uniqueService = null);
    public static abstract void FinalPrimitive(TSharedData sharedData, Logger2Event? logger, TUniqueService? uniqueService = null);
    public static abstract TSharedData InitServicePrimitive(TInitData initData, TUniqueService? uniqueService = null);

}
// public interface IService<TServiceCore,TServiceData, TSharedData, in TInitData,out TService> :
//     IPrimitiveService<TSharedData, TInitData, TService>
//     where TService : IService<TServiceCore,TServiceData, TSharedData, TInitData, TService>
//     where TServiceCore : IFinalAbleServiceCore<TServiceData, TSharedData ,TInitData>
//     where TServiceData : IServiceData
//     where TSharedData : ISharedDataWrapper {
//     static void IPrimitiveService<TSharedData, TInitData, TService>.FinalPrimitive(TSharedData sharedData, Logger2Event? logger) {
//         TService.Final(sharedData, logger);
//     }
//
//     static TSharedData IPrimitiveService<TSharedData, TInitData, TService>.InitServicePrimitive(TInitData initData) {
//         return TService.Init(initData);
//     }
//     
//     public static virtual void Final(TSharedData sharedData, Logger2Event? logger) {
//         TServiceCore.Final(sharedData, logger);
//     }
//     
//     public static virtual TSharedData Init(TInitData initData) {
//         return TServiceCore.Init(initData);
//     }
// }