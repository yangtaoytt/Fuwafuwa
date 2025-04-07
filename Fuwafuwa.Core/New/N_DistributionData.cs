namespace Fuwafuwa.Core.New.Serviece;

class N_DistributionData {
    public uint ThreadCount;
    public uint LastThreadId;
    public N_DistributionData(uint lastThreadId, uint threadCount) {
        this.LastThreadId = lastThreadId;
        this.ThreadCount = threadCount;
    }
}