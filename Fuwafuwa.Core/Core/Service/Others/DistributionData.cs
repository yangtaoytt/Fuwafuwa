namespace Fuwafuwa.Core.New.Serviece;

public class DistributionData {
    public ushort ThreadCount;
    public ushort LastThreadId;
    public DistributionData(ushort lastThreadId, ushort threadCount) {
        this.LastThreadId = lastThreadId;
        this.ThreadCount = threadCount;
    }
}