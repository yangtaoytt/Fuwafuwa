using System.Threading.Channels;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.Service.Level0;

public abstract class
    AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService> :IPrimitiveService<AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>,TSharedData, TInitData, TResService>
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TServiceCore : IServiceCore<TServiceData>, new() 
    where TService : class, IPrimitiveService<TService, TSharedData, TInitData, TResService> {
    private readonly Channel<(TServiceData, TSubjectData, TSharedData)> _channel;

    protected readonly TServiceCore ServiceCore;

    protected readonly Logger2Event? Logger;

    protected AService(Logger2Event? logger) {
        _channel = Channel.CreateUnbounded<(TServiceData, TSubjectData, TSharedData)>();
        ServiceCore = new TServiceCore();
        
        Logger = logger;
    }

    public ChannelWriter<(TServiceData, TSubjectData, TSharedData)> Writer => _channel.Writer;

    public async Task Run(CancellationToken cancellationToken) {
        Logger?.Info(this, "Run service");

        try {
            try {
                await foreach (var dataObject in _channel.Reader.ReadAllAsync(cancellationToken)) {
                    Logger?.Debug(this, "Process data");

                    await ProcessData(dataObject.Item1, dataObject.Item2, dataObject.Item3);
                }
            } catch (OperationCanceledException) {
                Logger?.Debug(this, "Service cancelled");
            }
        } catch (Exception e) {
            Logger?.Error(this, e.Message);
            throw;
        }
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData, TSharedData sharedData);

    public static IServiceAttribute<TServiceData> GetServiceAttribute() {
        return TServiceCore.GetServiceAttribute();
    }

    public static TResService CreateService(Logger2Event? logger, AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>? uniqueService = null) {
        return TService.CreateService(logger);
    }
    public static void FinalPrimitive(TSharedData sharedData, Logger2Event? logger, AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>? uniqueService = null) {
        TService.FinalPrimitive(sharedData, logger);
    }
    public static TSharedData InitServicePrimitive(TInitData initData, AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TService, TResService>? uniqueService = null) {
        return TService.InitServicePrimitive(initData);
    }
}