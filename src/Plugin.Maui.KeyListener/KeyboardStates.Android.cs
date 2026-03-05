using Android.Views;

namespace Plugin.Maui.KeyListener;

public static partial class KeyboardStatesExtensions
{
    public static KeyboardStates GetKeyboardStates()
    {
        //TODO implement
        var dummyEvent = new KeyEvent(KeyEventActions.Down, Keycode.Unknown);
        return dummyEvent.MetaState.ToKeyboardStates();
    }

    /// <remarks>
    /// https://developer.android.com/reference/android/view/KeyEvent
    /// </remarks>
    internal static KeyboardStates ToKeyboardStates(this MetaKeyStates states)
    {
        var result = KeyboardStates.None;

        if (states.HasFlag(MetaKeyStates.CapsLockOn))
            result |= KeyboardStates.CapsLock;
        if (states.HasFlag(MetaKeyStates.NumLockOn))
            result |= KeyboardStates.NumLock;
        if (states.HasFlag(MetaKeyStates.ScrollLockOn))
            result |= KeyboardStates.ScrollLock;

        return result;
    }
}
