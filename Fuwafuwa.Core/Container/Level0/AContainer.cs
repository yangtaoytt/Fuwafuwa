using System.Threading.Channels;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Distributor.Interface;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level0;

namespace Fuwafuwa.Core.Container.Level0;

public interface IContainer {
    public (Type attributeType, Type serviceType) ServiceAttributeType { get; }

    public Task Run(CancellationToken cancellationToken);
}

public abstract class AContainer<TService, TServiceData, TSubjectData, TSharedData> : IContainer
    where TService : AService<TServiceData, TSubjectData, TSharedData>, new()
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TSharedData : new() {
    public delegate IDistributor<TServiceData, TSubjectData, TSharedData> DelSetDistribute();

    private readonly int _serviceCount;

    private readonly List<TService> _services;


    protected readonly TSharedData SharedData;
    
    protected readonly Logger2Event? Logger;

    protected AContainer(int serviceCount, DelSetDistribute setter, Logger2Event? logger) {
        _serviceCount = serviceCount;
        Logger = logger;
        _services = [];
        InternalMainChannel = Channel.CreateUnbounded<(IServiceData, ISubjectData, IRegisterData)>();
        Distributor = setter();
        SharedData = new TSharedData();


        for (var i = 0; i < _serviceCount; i++) {
            _services.Add(new TService());
        }

        ServiceAttributeType = (new TService().GetServiceAttribute().GetType(), GetType());
    }

    private IDistributor<TServiceData, TSubjectData, TSharedData> Distributor { get; }

    protected Channel<(IServiceData, ISubjectData, IRegisterData)> InternalMainChannel { get; }

    public async Task Run(CancellationToken cancellationToken) {
        try {
            var tasks = _services.Select(processor => processor.Run(cancellationToken)).ToList();

            try {
                await foreach (var dataObject in InternalMainChannel.Reader.ReadAllAsync(
                                   cancellationToken)) {
                    Logger?.Debug(this, "Received data");
                    var (serviceData, subjectData, registerData) = dataObject;
                    if (serviceData is TServiceData tServiceData && subjectData is TSubjectData tSubjectData) {
                        Logger?.Debug(this, "Handle service data");
                        await HandleServiceData(tServiceData, tSubjectData);
                    } else {
                        Logger?.Debug(this, "Handle other data");
                        await HandleOtherData(serviceData, subjectData, registerData);
                    }
                }
            } catch (OperationCanceledException e) { }

            await Task.WhenAll(tasks);
        } catch (Exception e) {
            Logger?.Error(this, e.Message);
            throw;
        }
    }


    public (Type attributeType, Type serviceType) ServiceAttributeType { get; init; }


    protected ChannelWriter<(TServiceData, TSubjectData, TSharedData, Logger2Event?)> DistributeData(TServiceData serviceData,
        TSubjectData subjectData, TSharedData sharedData) {
        return _services[Distributor.Distribute(_serviceCount, serviceData, subjectData, sharedData)].Writer;
    }

    protected abstract Task HandleOtherData(IServiceData serviceData, ISubjectData subjectData,
        IRegisterData registerData);

    private async Task HandleServiceData(TServiceData serviceData, TSubjectData subjectData) {
        await DistributeData(serviceData, subjectData, SharedData).WriteAsync((serviceData, subjectData, SharedData, Logger));
    }
}