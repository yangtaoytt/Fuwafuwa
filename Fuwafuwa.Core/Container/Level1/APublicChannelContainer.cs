using System.Threading.Channels;
using Fuwafuwa.Core.Container.Level0;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level1;

public abstract class
    APublicChannelContainer<TService, TServiceData, TSubjectData, TSharedData> : AContainer<TService, TServiceData,
        TSubjectData, TSharedData>,
    IRegistrableContainer where TService : AService<TServiceData, TSubjectData, TSharedData>, new()
    where TServiceData : IServiceData
    where TSharedData : new()
    where TSubjectData : ISubjectData {
    public APublicChannelContainer(int serviceCount, DelSetDistribute setter) : base(serviceCount, setter) { }
    public Channel<(IServiceData, ISubjectData, IRegisterData)> MainChannel => InternalMainChannel;
}