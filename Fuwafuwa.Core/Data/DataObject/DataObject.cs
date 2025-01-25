using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;

namespace Fuwafuwa.Core.Data.DataObject;

public class DataObject<TData, TPrimaryInfo> where TData : IData where TPrimaryInfo : IPrimaryInfo {
    public DataObject(TData data, TPrimaryInfo primaryInfo) {
        Data = data;
        PrimaryInfo = primaryInfo;
    }

    public TData Data { get; }

    public TPrimaryInfo PrimaryInfo { get; }
}