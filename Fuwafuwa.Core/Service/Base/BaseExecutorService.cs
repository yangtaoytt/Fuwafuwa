using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Service.Abstract;

namespace Fuwafuwa.Core.Service.Base;

public abstract class BaseExecutorService : AServiceWithEmpty<ExecuteTaskData, EmptyInfo> {
    protected abstract Task ProcessData(ExecuteTaskData data);

    protected override async Task ProcessDataObject(DataObject<ExecuteTaskData, EmptyInfo> dataObject) {
        await ProcessData(dataObject.Data);
    }
}