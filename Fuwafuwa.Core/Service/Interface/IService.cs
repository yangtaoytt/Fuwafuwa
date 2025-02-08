using Fuwafuwa.Core.Log;

namespace Fuwafuwa.Core.Service.Interface;

// ReSharper disable TypeParameterCanBeVariant
// intended to disable the warning of TypeParameterCanBeVariant
public interface IService<TUniqueService, TSharedData, in TInitData, out TResService>
    // ReSharper restore TypeParameterCanBeVariant
    where TUniqueService : class {
    public static abstract TResService CreateService(Logger2Event? logger, TUniqueService? uniqueService = null);

    public static abstract void Final(TSharedData sharedData, Logger2Event? logger,
        TUniqueService? uniqueService = null);

    public static abstract TSharedData InitService(TInitData initData, TUniqueService? uniqueService = null);
}