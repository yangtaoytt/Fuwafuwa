using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level3;

public class
    InputContainer<TInputCore, TInputType, TSharedData, TInitData> : BaseContainerWithRegister<TInputCore,
    InputService<TInputCore, TSharedData, TInitData>, InputPackagedData, NullSubjectData, TSharedData, TInitData>
    where TSharedData : new()
    where TInputCore : IInputCore<TSharedData, TInitData>, new() {
    public InputContainer(int serviceCount, DelSetDistribute setter, InputHandler<TInputType> inputHandler,
        (Register, TInitData) initData, Logger2Event? logger) : base(
        serviceCount, setter, initData, logger) {
        inputHandler.OnInputEvent += OnInput;
    }

    private async Task OnInput(TInputType input) {
        Logger?.Debug(this, "OnInput");

        var packagedData = new InputPackagedData(input);
        await MainChannel.Writer.WriteAsync((packagedData, new NullSubjectData(), new NullRegisterData()));
    }
}

public class InputHandler<TInputType> {
    public delegate Task DelInput(TInputType input);

    public event DelInput? OnInputEvent;

    public async Task Input(TInputType input) {
        if (OnInputEvent != null) {
            await OnInputEvent.Invoke(input);
        }
    }
}