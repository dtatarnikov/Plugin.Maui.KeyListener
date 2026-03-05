using Android.Views;

namespace Plugin.Maui.KeyListener;

public static partial class KeyboardModifiersExtensions
{
    public static KeyboardModifiers GetKeyboardModifiers()
    {
        //TODO implement
        var dummyEvent = new KeyEvent(KeyEventActions.Down, Keycode.Unknown);
        return dummyEvent.Modifiers.ToKeyboardModifiers();
    }

    /// <remarks>
    /// https://developer.android.com/reference/android/view/KeyEvent
    /// </remarks>
    internal static KeyboardModifiers ToKeyboardModifiers(this MetaKeyStates metaState)
    {
        var result = KeyboardModifiers.None;

        if (metaState.HasFlag(MetaKeyStates.ShiftOn) || metaState.HasFlag(MetaKeyStates.ShiftLeftOn) || metaState.HasFlag(MetaKeyStates.ShiftRightOn))
            result |= KeyboardModifiers.Shift;
        if (metaState.HasFlag(MetaKeyStates.CtrlOn) || metaState.HasFlag(MetaKeyStates.CtrlLeftOn) || metaState.HasFlag(MetaKeyStates.CtrlRightOn))
            result |= KeyboardModifiers.Control;
        if (metaState.HasFlag(MetaKeyStates.AltOn) || metaState.HasFlag(MetaKeyStates.AltLeftOn) || metaState.HasFlag(MetaKeyStates.AltRightOn))
            result |= KeyboardModifiers.Alt;
        if (metaState.HasFlag(MetaKeyStates.MetaOn) || metaState.HasFlag(MetaKeyStates.MetaLeftOn) || metaState.HasFlag(MetaKeyStates.MetaRightOn))
            result |= KeyboardModifiers.Command;

        return result;
    }
}
