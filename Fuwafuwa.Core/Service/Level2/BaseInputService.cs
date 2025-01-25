using System.Diagnostics;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ExecuteDataSet;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceRegister;
using Fuwafuwa.Core.Subjects;
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.Service.Level2;

public abstract class
    BaseInputService<TSharedData> : AServiceWithRegister<InputPackagedData, NullSubjectData, TSharedData>
    where TSharedData : new() {
    protected abstract Task<List<Certificate>> ProcessData(InputPackagedData data, TSharedData sharedData);

    private async Task HandleResult(List<Certificate> certificates, Register register) {
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
            var bufferChannelList = register.GetTypeChannel(typeof(ISubjectBufferAttribute));
            Debug.Assert(bufferChannelList.Count == 1);
            var bufferChannel = bufferChannelList[0];

            await bufferChannel!.Writer.WriteAsync(
                (new NullServiceData(), new SubjectData(
                    null, 0, 1, initSubject, new ExecuteDataSet()), new NullRegisterData()));
        } else {
            foreach (var (key, value) in processorData) {
                var channelList = register.GetTypeChannel(key);

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

    protected override async Task ProcessData(InputPackagedData serviceData, NullSubjectData subjectData,
        Register register, TSharedData sharedData) {
        await HandleResult(await ProcessData(serviceData, sharedData), register);
    }

    public override IServiceAttribute<InputPackagedData> GetServiceAttribute() {
        return new IInputAttribute();
    }
}