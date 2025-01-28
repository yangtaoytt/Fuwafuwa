using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;

namespace Fuwafuwa.Core.Container.Level3;

public class
    InputContainer<TService, TInputType, TSharedData> : BaseContainerWithRegister<TService, InputPackagedData,
    NullSubjectData,
    TSharedData> where TService : BaseInputService<TSharedData>, new()
    where TSharedData : new() {
    public InputContainer(int serviceCount, DelSetDistribute setter, InputHandler<TInputType> inputHandler,Logger2Event? logger) : base(
        serviceCount, setter, logger) {
        inputHandler.OnInputEvent += OnInput;
        Logger?.Debug(this, "InputContainer initialized.");
    }

    private async Task OnInput(TInputType input) {
        var packagedData = new InputPackagedData(input);
        Logger?.Debug(this, $"Input received: {input}");
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