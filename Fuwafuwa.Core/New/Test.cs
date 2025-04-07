using Fuwafuwa.Core.New.Data;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

class TestService : N_IStaticThreadService<TestService>, N_ICustomerHandle<TestService, TestServiceData> {
    public TestService(ushort threadNumber) : base(threadNumber) { }
    public override TestService Implement() {
        return this;
    }

    public void Handle(TestServiceData data) {
        var result = data.GetData();
        
        Console.WriteLine("Handle data: " + result);
    }
}

class TestServiceData : N_AConsumerrData<TestService,TestServiceData> {
    
    public string GetData() {
        return "Test";
    }
    public override ushort Distribute(N_DistributionData distributionData) {
        return (ushort)((distributionData.LastThreadId + 1) % distributionData.ThreadCount);
    }
    public override TestServiceData Implement() {
        return this;
    }
}


public class Test {
    public static void TestFun() {
        TestService service = new TestService(1);
        TestServiceData data = new TestServiceData();
        service.Start();
        data.Send(service);
        
        Task.Delay(10000).Wait();
    }
}