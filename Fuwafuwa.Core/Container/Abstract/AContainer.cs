using Fuwafuwa.Core.Container.Distributor;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.DataChannel;
using Fuwafuwa.Core.Service.Abstract;

namespace Fuwafuwa.Core.Container.Abstract;

public abstract class AContainer<TService, TData, TPrimaryInfo, TInitType, TSubClassType>
    where TService : AService<TData, TPrimaryInfo, TInitType>, new()
    where TData : IData
    where TPrimaryInfo : IPrimaryInfo
    where TSubClassType : AContainer<TService, TData, TPrimaryInfo, TInitType, TSubClassType> {
    public delegate IDistributor<TPrimaryInfo> DelSetDistribute();

    private readonly int _serviceCount;

    private readonly List<ValueTuple<DataChannel<TData, TPrimaryInfo>, TService>> _services;

    private bool _isInit;

    protected AContainer(int serviceCount, DelSetDistribute setter) {
        _serviceCount = serviceCount;
        _services = [];
        InternalMainChannel = new DataChannel<IData, IPrimaryInfo>();
        Distributor = setter();
    }

    private IDistributor<TPrimaryInfo> Distributor { get; }

    protected DataChannel<IData, IPrimaryInfo> InternalMainChannel { get; }

    public TSubClassType Init() {
        if (!_isInit) {
            var initObject = default(TInitType);
            for (var i = 0; i < _serviceCount; i++) {
                var processor = new TService();
                if (initObject == null) {
                    initObject = GetInitObject(processor.CreateInitObject());
                }

                var channel = processor.InitProcessor(initObject);

                _services.Add(new ValueTuple<DataChannel<TData, TPrimaryInfo>, TService>(channel, processor));
            }

            _isInit = true;
        }


        return (TSubClassType)this;
    }

    protected abstract TInitType GetInitObject(TInitType initObject);


    protected DataChannel<TData, TPrimaryInfo> DistributeData(TPrimaryInfo info) {
        return _services[Distributor.Distribute(_serviceCount, info)].Item1;
    }

    protected abstract Task HandleOtherData(DataObject<IData, IPrimaryInfo> otherDataObject);

    private async Task HandleServiceData(DataObject<TData, TPrimaryInfo> dataObject) {
        await DistributeData(dataObject.PrimaryInfo).Writer.WriteAsync(dataObject);
    }

    public async Task Run(CancellationToken cancellationToken) {
        if (!_isInit) {
            throw new Exception("Container is not initialized");
        }


        var tasks = new List<Task>();
        foreach (var (_, processor) in _services) {
            tasks.Add(processor.Run(cancellationToken));
        }

        try {
            await foreach (DataObject<IData, IPrimaryInfo> dataObject in InternalMainChannel.Reader.ReadAllAsync(
                               cancellationToken)) {
                var data = dataObject.Data;
                var primaryInfo = dataObject.PrimaryInfo;
                if (data is TData tData && primaryInfo is TPrimaryInfo tInfo) {
                    await HandleServiceData(new DataObject<TData, TPrimaryInfo>(tData, tInfo));
                } else {
                    await HandleOtherData(dataObject);
                }
            }
        } catch (OperationCanceledException e) { }

        await Task.WhenAll(tasks);
    }

    public Type GetServiceType() {
        return typeof(TService);
    }
}