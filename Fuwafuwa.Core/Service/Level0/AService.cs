using System.Threading.Channels;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;

namespace Fuwafuwa.Core.Service.Level0;

public abstract class AService<TServiceData, TSubjectData, TSharedData>
    where TServiceData : IServiceData where TSubjectData : ISubjectData {
    private readonly Channel<(TServiceData, TSubjectData, TSharedData, Logger2Event?)> _channel;

    protected AService() {
        _channel = Channel.CreateUnbounded<(TServiceData, TSubjectData, TSharedData, Logger2Event?)>();
    }

    public ChannelWriter<(TServiceData, TSubjectData, TSharedData, Logger2Event?)> Writer => _channel.Writer;

    public async Task Run(CancellationToken cancellationToken) {
        try {
            try {
                await foreach (var dataObject in _channel.Reader.ReadAllAsync(cancellationToken)) {
                    await ProcessData(dataObject.Item1, dataObject.Item2, dataObject.Item3, dataObject.Item4);
                }
            } catch (OperationCanceledException e) { }
        } catch (Exception e) {
            
            throw;
        }
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData, TSharedData sharedData , Logger2Event? logger);

    public abstract IServiceAttribute<TServiceData> GetServiceAttribute();
}