using Fuwafuwa.Core.Data.PrimaryInfo.Interface;

namespace Fuwafuwa.Core.Container.Distributor;

public interface IDistributor<TPrimaryInfo> where TPrimaryInfo : IPrimaryInfo {
    int Distribute(int processorCount, TPrimaryInfo info);
}