using System.Threading.Channels;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;

namespace Fuwafuwa.Core.Service.Level0;

public abstract class AService<TServiceData, TSubjectData, TSharedData>
    where TServiceData : IServiceData where TSubjectData : ISubjectData {
    private readonly Channel<(TServiceData, TSubjectData, TSharedData)> _channel;

    protected AService() {
        _channel = Channel.CreateUnbounded<(TServiceData, TSubjectData, TSharedData)>();
    }

    public ChannelWriter<(TServiceData, TSubjectData, TSharedData)> Writer => _channel.Writer;

    public async Task Run(CancellationToken cancellationToken) {
        try {
            await foreach (var dataObject in _channel.Reader.ReadAllAsync(cancellationToken)) {
                await ProcessData(dataObject.Item1, dataObject.Item2, dataObject.Item3);
            }
        } catch (OperationCanceledException e) { }
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData, TSharedData sharedData);

    public abstract IServiceAttribute<TServiceData> GetServiceAttribute();
}