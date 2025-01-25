using System.Diagnostics;
using Fuwafuwa.Core.Attributes.Implements;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.ExecuteTask;
using Fuwafuwa.Core.Service.Abstract;

namespace Fuwafuwa.Core.Service.Base;

public abstract class BaseProcessService<TData> : AServiceWithRegister<TData, SubjectInfoWithCommand>
    where TData : IData {
    private async Task HandleDataAndTask(ValueTuple<Dictionary<Type, IData>, ExecuteTaskSet> dataAndTask,
        SubjectInfoWithCommand subjectInfoWithCommand) {
        var (dataDic, taskSet) = dataAndTask;

        if (dataDic.Count == 0) {
            var bufferChannelList = Register!.GetTypeChannel(typeof(ISubjectBufferAttribute));
            Debug.Assert(bufferChannelList.Count == 1);
            var bufferChannel = bufferChannelList[0];

            await bufferChannel!.Writer.WriteAsync(
                new DataObject<IData, IPrimaryInfo>(new EmptyData(), new SubjectInfo(
                    subjectInfoWithCommand, subjectInfoWithCommand.Index4Child,
                    subjectInfoWithCommand.SiblingCount4Child, subjectInfoWithCommand.Subject,
                    taskSet)));
        } else {
            foreach (var (key, value) in dataDic) {
                var channelList = Register.GetTypeChannel(key);

                for (var i = 0; i < channelList.Count; ++i) {
                    var channel = channelList[i];
                    await channel.Writer.WriteAsync(
                        new DataObject<IData, IPrimaryInfo>(value, new SubjectInfoWithCommand(
                            subjectInfoWithCommand, subjectInfoWithCommand.Index4Child,
                            subjectInfoWithCommand.SiblingCount4Child, subjectInfoWithCommand.Subject, taskSet,
                            i, channelList.Count
                        )));
                }
            }
        }
    }

    protected abstract Task<ValueTuple<Dictionary<Type, IData>, ExecuteTaskSet>> ProcessData(TData data);

    protected override async Task ProcessDataObject(DataObject<TData, SubjectInfoWithCommand> dataObject) {
        await HandleDataAndTask(await ProcessData(dataObject.Data), dataObject.PrimaryInfo);
    }
}