using Fuwafuwa.Core.Container.Level1;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level2;

public abstract class
    BaseContainerWithRegister<TServiceCore, TService, TServiceData, TSubjectData, TSharedData, TInitData> :
    APublicChannelContainer<TServiceCore, TService,
        TServiceData, TSubjectData, InitTuple<Register, TSharedData>, (Register, TInitData)>
    where TService : AServiceWithRegister<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData>, new()
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TSharedData : new()
    where TServiceCore : IServiceCore<TServiceData>, new() {
    protected BaseContainerWithRegister(int serviceCount, DelSetDistribute setter, (Register, TInitData) initData,
        Logger2Event? logger) : base(serviceCount, setter, initData, logger) { }

    protected override Task HandleOtherData(IServiceData serviceData, ISubjectData subjectData,
        IRegisterData registerData) {
        var register = SharedData.Item1;

        switch (registerData) {
            case AddRegisterData addRegisterData:
                Logger?.Debug(this, "Add register data");
                register.ServiceTypes.TryAdd(
                    addRegisterData.ServiceType,
                    addRegisterData.Channel
                );
                addRegisterData.RegisterConfirmDelegate(ServiceAttributeType, addRegisterData.ServiceType);
                break;
            case InitRegisterData initRegisterData:
                Logger?.Debug(this, "Init register data");
                register.ServiceTypes.Clear();
                foreach (var (key, value) in initRegisterData.InitRegister.ServiceTypes) {
                    register.ServiceTypes.TryAdd(key, value);
                }

                initRegisterData.InitConfirmDelegate(ServiceAttributeType);
                break;
            case RemoveRegisterData removeRegisterData:
                Logger?.Debug(this, "Remove register data");
                register.ServiceTypes.TryRemove(
                    removeRegisterData.ServiceType,
                    out _
                );
                removeRegisterData.UnregisterConfirmDelegate(ServiceAttributeType, removeRegisterData.ServiceType);
                break;
            default:
                throw new Exception("strange RegisterData type");
        }

        if (serviceData is not NullServiceData || subjectData is not NullSubjectData) {
            throw new Exception("strange ServiceData/SubjectData type");
        }

        return Task.CompletedTask;
    }
}