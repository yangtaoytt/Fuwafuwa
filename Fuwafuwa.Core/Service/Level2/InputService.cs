using System.Diagnostics;
using System.Threading.Channels;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ExecuteDataSet;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWapper.Implement;
using Fuwafuwa.Core.Data.SharedDataWapper.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;
using Fuwafuwa.Core.Subjects;
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.Service.Level2;

public class
    InputService<TInputCore, TSharedData, TInitData> : AServiceWithRegister<TInputCore, InputPackagedData,
    NullSubjectData, TSharedData, TInitData,InputService<TInputCore, TSharedData, TInitData>,InputService<TInputCore, TSharedData, TInitData>>, IPrimitiveService<InputService<TInputCore, TSharedData, TInitData>, TSharedData, TInitData, InputService<TInputCore, TSharedData, TInitData>> where TSharedData : ISharedDataWrapper
    where TInputCore : IInputCore<TSharedData, TInitData>, new() {
    private InputService(Logger2Event? logger) : base(logger) { }

    private async Task HandleResult(List<Certificate> certificates, SimpleSharedDataWrapper<Register> register) {
        Logger?.Debug(this, "HandleResult");

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

        var initSubject = Subject.GetSubject();

        if (processorData.Count == 0) {
            var bufferChannelList =
                register.Execute(reg => reg.Value.GetTypeChannel(typeof(ISubjectBufferAttribute)));
            Debug.Assert(bufferChannelList.Count == 1);
            var bufferChannel = bufferChannelList[0];

            await bufferChannel!.Writer.WriteAsync(
                (new NullServiceData(), new SubjectData(
                    null, 0, 1, initSubject, new ExecuteDataSet()), new NullRegisterData()));
        } else {
            foreach (var (key, value) in processorData) {
                var channelList = 
                    register.Execute(reg => reg.Value.GetTypeChannel(key));

                for (var i = 0; i < channelList.Count; ++i) {
                    var channel = channelList[i];
                    await channel.Writer.WriteAsync(
                        (
                            value,
                            new SubjectDataWithCommand(
                                null, 0, 1, initSubject, new ExecuteDataSet(), i, channelList.Count),
                            new NullRegisterData()));
                }
            }
        }
    }

    protected override async Task ProcessData(InputPackagedData serviceData, NullSubjectData subjectData, SimpleSharedDataWrapper<Register> register,
        TSharedData sharedData) {
        await HandleResult(await ServiceCore.ProcessData(serviceData, sharedData, Logger), register);
    }

    public static InputService<TInputCore, TSharedData, TInitData> CreateService(Logger2Event? logger, InputService<TInputCore, TSharedData, TInitData>? uniqueService = null) {
        return new InputService<TInputCore, TSharedData, TInitData>(logger);
    }
    public static void FinalPrimitive(TSharedData sharedData, Logger2Event? logger, InputService<TInputCore, TSharedData, TInitData>? uniqueService = null) {
        TInputCore.Final(sharedData, logger);
    }
    public static TSharedData InitServicePrimitive(TInitData initData, InputService<TInputCore, TSharedData, TInitData>? uniqueService = null) {
        return TInputCore.Init(initData);
    }
}