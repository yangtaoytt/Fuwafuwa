using Fuwafuwa.Core.New.Data;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

interface TempInterface {
    void Accept<TService>(N_IServiceData<TService> service) where TService : N_IService<TService>;
}