using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Test.TestImplement.Attribute.Processor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.ServiceCore.Processor;

public class TestSyncProcessor<TSharedDataWrapper> : IProcessorCore<StringData, TSharedDataWrapper, object>
    where TSharedDataWrapper : ISyncSharedDataWrapper<bool, TSharedDataWrapper> {
    public static IServiceAttribute<StringData> GetServiceAttribute() {
        return IReadString.GetInstance();
    }

    public static TSharedDataWrapper Init(object initData) {
        var res = TSharedDataWrapper.CreateWrapper(false);
        return res;
    }

    public static void Final(TSharedDataWrapper sharedData, Logger2Event? logger) { }

    public async Task<List<Certificate>> ProcessData(StringData data, TSharedDataWrapper sharedData,
        Logger2Event? logger) {
        await Task.CompletedTask;

        sharedData.Execute(refData => {
            if (refData.Value) {
                logger?.Error(this, "Data is in processing");
            }

            refData.Value = true;
            logger?.Info(this,
                $"Processing thread:{Thread.CurrentThread.Name}:{Environment.CurrentManagedThreadId} level 0");
            Task.Delay(1000).Wait();
            logger?.Info(this,
                $"Processing thread:{Thread.CurrentThread.Name}:{Environment.CurrentManagedThreadId} level 1");
            Task.Delay(1000).Wait();
            logger?.Info(this,
                $"Processing thread:{Thread.CurrentThread.Name}:{Environment.CurrentManagedThreadId} level 2");
            Task.Delay(1000).Wait();
            refData.Value = false;
        });
        return [];
    }
}