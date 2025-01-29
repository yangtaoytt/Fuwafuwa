using System.Diagnostics;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ExecuteDataSet;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.Service.Level2;

public class
    ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData> : AServiceWithRegister<TProcessorCore,
    TServiceData, SubjectDataWithCommand,
    TSharedData, TInitData> where TServiceData : IProcessorData
    where TSharedData : new()
    where TProcessorCore : IProcessorCore<TServiceData, TSharedData, TInitData>, new() {
    protected override async Task ProcessData(TServiceData serviceData, SubjectDataWithCommand subjectData,
        Register register,
        TSharedData sharedData) {
        await HandleDataAndTask(await ServiceCore.ProcessData(serviceData, sharedData, Logger), subjectData, register);
    }

    protected override TSharedData SubInit(TInitData initData) {
        return TProcessorCore.Init(initData);
    }

    private async Task HandleDataAndTask(List<Certificate> certificates,
        SubjectDataWithCommand subjectDataWithCommand, Register register) {
        Logger?.Debug(this, "HandleDataAndTask");
        var processorData = new Dictionary<Type, IServiceData>();
        var taskSet = new ExecuteDataSet();
        foreach (var certificate in certificates) {
            var type = certificate.ServiceAttribute.GetType();
            var data = certificate.ServiceData;

            if (Util.Is(type, typeof(IExecutorAttribute))) {
                taskSet.AddTask((AExecutorData)data);
            } else {
                processorData.Add(type, data);
            }
        }

        if (processorData.Count == 0) {
            var bufferChannelList = register.GetTypeChannel(typeof(ISubjectBufferAttribute));
            Debug.Assert(bufferChannelList.Count == 1);
            var bufferChannel = bufferChannelList[0];

            await bufferChannel!.Writer.WriteAsync(
                (new NullServiceData(), new SubjectData(
                    subjectDataWithCommand, subjectDataWithCommand.Index4Child,
                    subjectDataWithCommand.SiblingCount4Child, subjectDataWithCommand.Subject,
                    taskSet), new NullRegisterData()));
        } else {
            foreach (var (key, value) in processorData) {
                var channelList = register.GetTypeChannel(key);

                for (var i = 0; i < channelList.Count; ++i) {
                    var channel = channelList[i];
                    await channel.Writer.WriteAsync(
                        (value, new SubjectDataWithCommand(
                            subjectDataWithCommand, subjectDataWithCommand.Index4Child,
                            subjectDataWithCommand.SiblingCount4Child, subjectDataWithCommand.Subject, taskSet,
                            i, channelList.Count
                        ), new NullRegisterData()));
                }
            }
        }
    }

    public override void Final(InitTuple<Register, TSharedData> sharedData, Logger2Event? logger) {
        base.Final(sharedData, logger);
        TProcessorCore.Final(sharedData.Item2, logger);
    }
}