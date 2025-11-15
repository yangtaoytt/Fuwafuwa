namespace Fuwafuwa.Core.Core.Service.Others;

/// <summary>
/// The data structure for thread distribution information.
/// </summary>
public class DistributionData {
    public readonly ushort ThreadCount;
    public ushort LastThreadId;
    public DistributionData(ushort lastThreadId, ushort threadCount) {
        this.LastThreadId = lastThreadId;
        this.ThreadCount = threadCount;
    }
}