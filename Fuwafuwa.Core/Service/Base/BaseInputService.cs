using System.Diagnostics;
using Fuwafuwa.Core.Attributes.Implements;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.ExecuteTask;
using Fuwafuwa.Core.Service.Abstract;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Service.Base;

public abstract class BaseInputService : AServiceWithRegister<InputPackagedData, EmptyInfo>, IInputAttribute {
    protected override async Task ProcessDataObject(DataObject<InputPackagedData, EmptyInfo> dataObject) {
        await HandleData(await ProcessData(dataObject.Data));
    }

    protected abstract Task<Dictionary<Type, IData>> ProcessData(InputPackagedData data);

    private async Task HandleData(Dictionary<Type, IData> dataDic) {
        var initSubject = Subject.GetSubject();

        if (dataDic.Count == 0) {
            var bufferChannelList = Register!.GetTypeChannel(typeof(ISubjectBufferAttribute));
            Debug.Assert(bufferChannelList.Count == 1);
            var bufferChannel = bufferChannelList[0];

            await bufferChannel!.Writer.WriteAsync(
                new DataObject<IData, IPrimaryInfo>(new EmptyData(), new SubjectInfo(
                    null, 0, 1, initSubject, new ExecuteTaskSet())));
        } else {
            foreach (var (key, value) in dataDic) {
                var channelList = Register.GetTypeChannel(key);

                for (var i = 0; i < channelList.Count; ++i) {
                    var channel = channelList[i];
                    await channel.Writer.WriteAsync(
                        new DataObject<IData, IPrimaryInfo>(value, new SubjectInfoWithCommand(
                            null, 0, 1, initSubject, new ExecuteTaskSet(), i, channelList.Count)));
                }
            }
        }
    }
}