using System.Diagnostics;
using System.Threading.Channels;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ExecuteDataSet;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level2;
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
        NullSubjectData, TSharedData, TInitData, InputService<TInputCore, TSharedData, TInitData>,
        InputService<TInputCore, TSharedData, TInitData>>,
    IService<InputService<TInputCore, TSharedData, TInitData>, TSharedData, TInitData,
        InputService<TInputCore, TSharedData, TInitData>> where TSharedData : ISharedDataWrapper
    where TInputCore : IInputCore<TSharedData, TInitData>, new() {
    private InputService(Logger2Event? logger) : base(logger) { }

    public static InputService<TInputCore, TSharedData, TInitData> CreateService(Logger2Event? logger,
        InputService<TInputCore, TSharedData, TInitData>? uniqueService = null) {
        return new InputService<TInputCore, TSharedData, TInitData>(logger);
    }

    public static void Final(TSharedData sharedData, Logger2Event? logger,
        InputService<TInputCore, TSharedData, TInitData>? uniqueService = null) {
        TInputCore.Final(sharedData, logger);
    }

    public static TSharedData InitService(TInitData initData,
        InputService<TInputCore, TSharedData, TInitData>? uniqueService = null) {
        return TInputCore.Init(initData);
    }

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
            
            if (register.Execute(reference => reference.Value.ServiceTypes.Count) == 0) {
                var channelList = register.Execute(reg => reg.Value.ServiceRegisterGroup.GetTypeChannel(typeof(ISubjectBufferAttribute)));
                if (channelList.Count == 0) {
                    return;
                }
                var channel = channelList[0];
                await channel.Writer.WriteAsync(
                    (new NullServiceData(), new SubjectData(
                        null, 0,
                        1, initSubject,
                        new ExecuteDataSet()), new NullRegisterData()));
                return;
            }
            
            
            var bufferChannel = bufferChannelList[0];

            await bufferChannel!.Writer.WriteAsync(
                (new NullServiceData(), new SubjectData(
                    null, 0,
                    1, initSubject,
                    taskSet), new NullRegisterData()));
        } else {
            List<(List<Channel<(IServiceData, ISubjectData, IRegisterData)>>, IServiceData)> allList = [];
            int count = 0;
            foreach (var (key, value) in processorData) {
                var channelList = register.Execute(reg => reg.Value.GetTypeChannel(key));
                allList.Add((channelList,value));
                count += channelList.Count;
            }

            if (register.Execute(reference => reference.Value.ServiceTypes.Count) == 0) {
                var channelList = register.Execute(reg => reg.Value.ServiceRegisterGroup.GetTypeChannel(typeof(ISubjectBufferAttribute)));
                if (channelList.Count == 0) {
                    return;
                }
                var channel = channelList[0];
                await channel.Writer.WriteAsync(
                    (new NullServiceData(), new SubjectData(
                        null, 0,
                        1, initSubject,
                        new ExecuteDataSet()), new NullRegisterData()));
                return;
            }

            int j = 0;
            foreach (var (channelList,value) in allList) {
                for (var i = 0; i < channelList.Count; ++i) {
                    var channel = channelList[i];
                    await channel.Writer.WriteAsync(
                        (value, new SubjectDataWithCommand(
                            null, 0,
                            1, initSubject, taskSet,
                            j++, count
                        ), new NullRegisterData()));
                }
            }
        }
    }

    protected override async Task ProcessData(InputPackagedData serviceData, NullSubjectData subjectData,
        SimpleSharedDataWrapper<Register> register,
        TSharedData sharedData) {
        await HandleResult(await ServiceCore.ProcessData(serviceData, sharedData, Logger), register);
    }
}