using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.DataChannel;

namespace Fuwafuwa.Core.Service.Abstract;

public abstract class AService<TData, TPrimaryInfo, TInitType>
    where TData : IData where TPrimaryInfo : IPrimaryInfo {
    private readonly DataChannel<TData, TPrimaryInfo> _channel;

    protected AService() {
        _channel = new DataChannel<TData, TPrimaryInfo>();
    }

    public DataChannel<TData, TPrimaryInfo> InitProcessor(TInitType initObject) {
        Init(initObject);
        return _channel;
    }

    protected abstract void Init(TInitType initObject);

    public async Task Run(CancellationToken cancellationToken) {
        if (!InitCheck()) {
            throw new Exception("Processor Init check failed");
        }

        try {
            await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken)) {
                await ProcessDataObject(item);
            }
        } catch (OperationCanceledException e) { }
    }

    protected abstract bool InitCheck();

    protected abstract Task ProcessDataObject(DataObject<TData, TPrimaryInfo> dataObject);


    public abstract TInitType CreateInitObject();
}