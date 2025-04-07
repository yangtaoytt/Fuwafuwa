namespace Fuwafuwa.Core.New.Serviece;

class N_DistributionData {
    public ushort ThreadCount;
    public ushort LastThreadId;
    public N_DistributionData(ushort lastThreadId, ushort threadCount) {
        this.LastThreadId = lastThreadId;
        this.ThreadCount = threadCount;
    }
}