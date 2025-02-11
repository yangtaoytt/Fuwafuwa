using System.Collections.Concurrent;
using System.Threading.Channels;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.ServiceRegister;

public class Register {
    public readonly ServiceRegisterGroup ServiceRegisterGroup;

    public Register(ServiceRegisterGroup serviceRegisterGroup) {
        ServiceTypes = new ConcurrentDictionary<(Type, Type), Channel<(IServiceData, ISubjectData, IRegisterData)>>();
        ServiceRegisterGroup = serviceRegisterGroup;
    }

    public Register(Register other) {
        ServiceTypes =
            new ConcurrentDictionary<(Type, Type), Channel<(IServiceData, ISubjectData, IRegisterData)>>(
                other.ServiceTypes);
        ServiceRegisterGroup = other.ServiceRegisterGroup;
    }

    public ConcurrentDictionary<(Type attributeType, Type serviceType),
        Channel<(IServiceData, ISubjectData, IRegisterData)>> ServiceTypes { get; }

    public List<Channel<(IServiceData, ISubjectData, IRegisterData)>> GetTypeChannel(Type type) {
        return ServiceTypes
            .Where(kvp => Util.Is(kvp.Key.attributeType, type))
            .Select(kvp => kvp.Value)
            .ToList();
    }
}