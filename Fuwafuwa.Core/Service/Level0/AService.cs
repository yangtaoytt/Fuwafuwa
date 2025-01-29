using System.Threading.Channels;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.Service.Level0;

public abstract class AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData>
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TServiceCore : IServiceCore<TServiceData>, new() {
    private readonly Channel<(TServiceData, TSubjectData, TSharedData)> _channel;

    protected readonly TServiceCore ServiceCore;

    protected Logger2Event? Logger;

    protected AService() {
        _channel = Channel.CreateUnbounded<(TServiceData, TSubjectData, TSharedData)>();
        ServiceCore = new TServiceCore();
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

    public IServiceAttribute<TServiceData> GetServiceAttribute() {
        return TServiceCore.GetServiceAttribute();
    }

    protected abstract TSharedData Init(TInitData initData);

    public TSharedData Init(TInitData initData, Logger2Event? logger) {
        logger?.Debug(this, "Init service");
        Logger = logger;
        return Init(initData);
    }

    public virtual void Final(TSharedData sharedData, Logger2Event? logger) {
        logger?.Info(this, "Final service");
    }
}