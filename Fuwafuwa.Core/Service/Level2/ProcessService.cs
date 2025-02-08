using System.Diagnostics;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ExecuteDataSet;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level2;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.Service.Level2;

public class
    ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData> : AServiceWithRegister<TProcessorCore,
        TServiceData, SubjectDataWithCommand, TSharedData, TInitData,
        ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>,
        ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>>,
    IService<ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>, TSharedData, TInitData,
        ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>> where TServiceData : IProcessorData
    where TSharedData : ISharedDataWrapper
    where TProcessorCore : IProcessorCore<TServiceData, TSharedData, TInitData>, new() {
    private ProcessService(Logger2Event? logger) : base(logger) { }

    public static ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData> CreateService(
        Logger2Event? logger,
        ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>? uniqueService = null) {
        return new ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>(logger);
    }

    public static void Final(TSharedData sharedData, Logger2Event? logger,
        ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>? uniqueService = null) {
        TProcessorCore.Final(sharedData, logger);
    }

    public static TSharedData InitService(TInitData initData,
        ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>? uniqueService = null) {
        return TProcessorCore.Init(initData);
    }

    protected override async Task ProcessData(TServiceData serviceData, SubjectDataWithCommand subjectData,
        SimpleSharedDataWrapper<Register> register,
        TSharedData sharedData) {
        await HandleDataAndTask(await ServiceCore.ProcessData(serviceData, sharedData, Logger), subjectData, register);
    }

    private async Task HandleDataAndTask(List<Certificate> certificates,
        SubjectDataWithCommand subjectDataWithCommand, SimpleSharedDataWrapper<Register> register) {
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
            var bufferChannelList =
                register.Execute(reg => reg.Value.GetTypeChannel(typeof(ISubjectBufferAttribute)));
            Debug.Assert(bufferChannelList.Count == 1);
            var bufferChannel = bufferChannelList[0];

            await bufferChannel!.Writer.WriteAsync(
                (new NullServiceData(), new SubjectData(
                    subjectDataWithCommand, subjectDataWithCommand.Index4Child,
                    subjectDataWithCommand.SiblingCount4Child, subjectDataWithCommand.Subject,
                    taskSet), new NullRegisterData()));
        } else {
            foreach (var (key, value) in processorData) {
                var channelList = register.Execute(reg => reg.Value.GetTypeChannel(key));

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
}