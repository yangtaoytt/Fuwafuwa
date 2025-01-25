using Fuwafuwa.Core.Container.Base;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.Service.Base;

namespace Fuwafuwa.Core.Container.Implements;

public class InputHandler<TInputType> {
    public delegate Task DelInput(TInputType input);

    public event DelInput? OnInputEvent;

    public async Task Input(TInputType input) {
        if (OnInputEvent != null) {
            await OnInputEvent.Invoke(input);
        }
    }
}

public class
    InputContainer<TService, TInputType> : BaseContainerWithRegister<TService, InputPackagedData, EmptyInfo,
    InputContainer<TService, TInputType>> where TService : BaseInputService, new() {
    public InputContainer(int processorCount, DelSetDistribute setter, InputHandler<TInputType> inputHandler) : base(
        processorCount, setter) {
        inputHandler.OnInputEvent += OnInput;
    }

    private async Task OnInput(TInputType input) {
        var packagedData = new InputPackagedData(input);
        await MainChannel.Writer.WriteAsync(new DataObject<IData, IPrimaryInfo>(packagedData, new EmptyInfo()));
    }
}