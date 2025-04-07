using Fuwafuwa.Core.New.Data;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

class TestService : N_IStaticThreadService<TestService>, N_ICustomerHandle<TestService, TestServiceData> {
    public TestService(ushort threadNumber) : base(threadNumber) { }
    public override TestService Implement() {
        throw new NotImplementedException();
    }

    public void Handle(TestServiceData data) {
        throw new NotImplementedException();
    }
}

class TestServiceData : N_AConsumerrData<TestService,TestServiceData> {
    public override ushort Distribute(N_DistributionData distributionData) {
        throw new NotImplementedException();
    }
    public override TestServiceData Implement() {
        throw new NotImplementedException();
    }
}


public class Test {
    public static void TestFun() {
        TestService service = new TestService(1);
        TestServiceData data = new TestServiceData();
        data.Send(service);
    }
}