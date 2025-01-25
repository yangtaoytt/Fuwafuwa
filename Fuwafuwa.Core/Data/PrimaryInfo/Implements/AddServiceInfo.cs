using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.DataChannel;

namespace Fuwafuwa.Core.Data.PrimaryInfo.Implements;

public class AddServiceInfo : IPrimaryInfo {
    public AddServiceInfo(Type serviceType, DataChannel<IData, IPrimaryInfo> dataChannel) {
        ServiceType = serviceType;
        DataChannel = dataChannel;
    }

    public Type ServiceType { get; init; }
    public DataChannel<IData, IPrimaryInfo> DataChannel { get; init; }
}