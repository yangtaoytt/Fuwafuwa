using System.Threading.Channels;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Distributor.Interface;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.Container.Level0;

public interface IContainer {
    public (Type attributeType, Type serviceType) ServiceAttributeType { get; }

    public Task Run(CancellationToken cancellationToken);
}

public abstract class
    AContainer<TServiceCore, TService, TServiceData, TSubjectData, TSharedData, TInitData, TNextService> : IContainer
    where TService : AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData,TNextService,TService>
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TServiceCore : IServiceCore<TServiceData>, new()
    where TNextService : class, IPrimitiveService<TNextService, TSharedData, TInitData, TService> {
    public delegate IDistributor<TServiceData, TSubjectData, TSharedData> DelSetDistribute();

    private readonly int _serviceCount;

    private readonly List<TService> _services;

    protected readonly Logger2Event? Logger;
    
    protected readonly TSharedData SharedData;
    

    protected AContainer(int serviceCount, DelSetDistribute setter, TInitData initData, Logger2Event? logger = null) {
        logger?.Info(this, "Init container");

        _serviceCount = serviceCount;
        Logger = logger;
        _services = [];
        InternalMainChannel = Channel.CreateUnbounded<(IServiceData, ISubjectData, IRegisterData)>();
        Distributor = setter();
        SharedData = AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData,TNextService,TService>.InitServicePrimitive(initData);


        for (var i = 0; i < _serviceCount; i++) {
            _services.Add(AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData,TNextService,TService>.CreateService(logger));
        }

        ServiceAttributeType = (AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData,TNextService,TService>.GetServiceAttribute().GetType(), GetType());
    }

    private IDistributor<TServiceData, TSubjectData, TSharedData> Distributor { get; }

    protected Channel<(IServiceData, ISubjectData, IRegisterData)> InternalMainChannel { get; }

    public async Task Run(CancellationToken cancellationToken) {
        Logger?.Info(this, "Run container");

        CancellationTokenSource serviceCancellationTokenSource = new();
        try {
            var tasks = _services.Select(processor => processor.Run(serviceCancellationTokenSource.Token)).ToList();

            try {
                await foreach (var dataObject in InternalMainChannel.Reader.ReadAllAsync(
                                   cancellationToken)) {
                    Logger?.Debug(this, "Received data");
                    var (serviceData, subjectData, registerData) = dataObject;
                    if (serviceData is TServiceData tServiceData && subjectData is TSubjectData tSubjectData) {
                        await HandleServiceData(tServiceData, tSubjectData);
                    } else {
                        Logger?.Debug(this, "Handle other data");
                        await HandleOtherData(serviceData, subjectData, registerData);
                    }
                }
            } catch (OperationCanceledException) {
                Logger?.Debug(this, "Container canceled");
            } finally {
                AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData,TNextService,TService>.FinalPrimitive(SharedData, Logger);
                await serviceCancellationTokenSource.CancelAsync();
                await Task.WhenAll(tasks);

                Logger?.Info(this, "Container final");
            }
        } catch (Exception e) {
            Logger?.Error(this, e.Message);
            throw;
        }
    }


    public (Type attributeType, Type serviceType) ServiceAttributeType { get; init; }


    private ChannelWriter<(TServiceData, TSubjectData, TSharedData)> DistributeData(TServiceData serviceData,
        TSubjectData subjectData, TSharedData sharedData) {
        return _services[Distributor.Distribute(_serviceCount, serviceData, subjectData, sharedData)].Writer;
    }

    protected abstract Task HandleOtherData(IServiceData serviceData, ISubjectData subjectData,
        IRegisterData registerData);

    private async Task HandleServiceData(TServiceData serviceData, TSubjectData subjectData) {
        Logger?.Debug(this, "Handle service data");
        await DistributeData(serviceData, subjectData, SharedData).WriteAsync((serviceData, subjectData, SharedData));
    }
}