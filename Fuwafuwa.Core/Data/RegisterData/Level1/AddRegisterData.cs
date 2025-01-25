using System.Threading.Channels;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Data.RegisterData.Level1;

public class AddRegisterData : IRegisterData {
    public AddRegisterData((Type attributeType, Type serviceType) serviceType,
        Channel<(IServiceData, ISubjectData, IRegisterData)> channel,
        ServiceRegisterGroup.RegisterUpdateConfirmDelegate registerConfirmDelegate) {
        ServiceType = serviceType;
        Channel = channel;
        RegisterConfirmDelegate = registerConfirmDelegate;
    }

    public (Type attributeType, Type serviceType) ServiceType { get; init; }
    public Channel<(IServiceData, ISubjectData, IRegisterData)> Channel { get; init; }

    public ServiceRegisterGroup.RegisterUpdateConfirmDelegate RegisterConfirmDelegate { get; init; }
}