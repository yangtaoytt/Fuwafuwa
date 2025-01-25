using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Service.Level1;

namespace Fuwafuwa.Core.Service.Level2;

public abstract class
    BaseExecutorService<TServiceData, TSharedData> : AServiceWithEmpty<TServiceData, NullSubjectData, TSharedData>
    where TSharedData : new() where TServiceData : IServiceData {
    protected abstract Task ExecuteTask(TServiceData data, TSharedData sharedData);

    protected override Task ProcessData(TServiceData serviceData, NullSubjectData subjectData, TSharedData sharedData) {
        return ExecuteTask(serviceData, sharedData);
    }
}