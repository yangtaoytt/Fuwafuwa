using System.Threading.Channels;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;

namespace Fuwafuwa.Core.DataChannel;

public class DataChannel<TData, TPrimaryInfo>
    where TData : IData where TPrimaryInfo : IPrimaryInfo {
    private readonly Channel<DataObject<TData, TPrimaryInfo>> _channel;

    public DataChannel() {
        _channel = Channel.CreateUnbounded<DataObject<TData, TPrimaryInfo>>();
    }

    public ChannelReader<DataObject<TData, TPrimaryInfo>> Reader => _channel.Reader;
    public ChannelWriter<DataObject<TData, TPrimaryInfo>> Writer => _channel.Writer;
}