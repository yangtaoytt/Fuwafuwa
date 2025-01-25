using System.Collections.Concurrent;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.DataChannel;
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.ServiceRegister;

public class Register {
    public Register() {
        ServiceTypes = new ConcurrentDictionary<Type, DataChannel<IData, IPrimaryInfo>>();
    }

    public Register(Register other) {
        ServiceTypes =
            new ConcurrentDictionary<Type, DataChannel<IData, IPrimaryInfo>>(other.ServiceTypes);
    }

    public ConcurrentDictionary<Type, DataChannel<IData, IPrimaryInfo>> ServiceTypes { get; }

    public List<DataChannel<IData, IPrimaryInfo>> GetTypeChannel(Type type) {
        return ServiceTypes
            .Where(kvp => Util.Is(kvp.Key, type))
            .Select(kvp => kvp.Value)
            .ToList();
    }
}