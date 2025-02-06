using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWapper.Implement;
using Fuwafuwa.Core.Data.SharedDataWapper.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level3;

public sealed class
    InputContainer<TInputCore, TInputType, TSharedData, TInitData> : BaseContainerWithRegister<TInputCore,
    InputService<TInputCore, TSharedData, TInitData>, InputPackagedData, NullSubjectData, TSharedData, TInitData,
    InputService<TInputCore, TSharedData, TInitData>>
    where TInputCore : IInputCore<TSharedData, TInitData>, new() where TSharedData : ISharedDataWrapper {

    public InputContainer(int serviceCount, DelSetDistribute setter,InputHandler<TInputType> inputHandler,
        (SimpleSharedDataWrapper<Register>, TInitData) initData, Logger2Event? logger = null) : base(serviceCount,
        setter, initData, logger) {
        inputHandler.OnInputEvent += OnInput;
    }

    private async Task OnInput(TInputType input) {
        Logger?.Debug(this, "OnInput");

        var packagedData = new InputPackagedData(input);
        await MainChannel.Writer.WriteAsync((packagedData, new NullSubjectData(), new NullRegisterData()));
    }
}

public sealed class InputHandler<TInputType> {
    public delegate Task DelInput(TInputType input);

    public event DelInput? OnInputEvent;

    public async Task Input(TInputType input) {
        if (OnInputEvent != null) {
            await OnInputEvent.Invoke(input);
        }
    }
}