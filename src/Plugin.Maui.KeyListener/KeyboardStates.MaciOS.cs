using System.Runtime.InteropServices;
using ObjCRuntime;
using UIKit;

namespace Plugin.Maui.KeyListener;

public static partial class KeyboardStatesExtensions
{
#if MACCATALYST
    [DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
    private static extern nint int_objc_msgSend(IntPtr receiver, IntPtr selector);
#endif

    public static KeyboardStates GetKeyboardStates()
    {
        var states = KeyboardStates.None;
        
#if MACCATALYST
        IntPtr nsEventClass = Class.GetHandle("NSEvent");
        var uiModifierFlags = (UIKeyModifierFlags)int_objc_msgSend(nsEventClass, Selector.GetHandle("modifierFlags")); //currentModifierFlags

        states |= uiModifierFlags.ToKeyboardStates();
#endif

        return states;
    }

    /// <remarks>
    /// https://developer.apple.com/documentation/uikit/uikeymodifierflags
    /// </remarks>
    internal static KeyboardStates ToKeyboardStates(this UIKeyModifierFlags platformStates)
    {
        KeyboardStates virtualStates = 0;
        
        if (platformStates.HasFlag(UIKeyModifierFlags.AlphaShift))
            virtualStates |= KeyboardStates.CapsLock;
        if (platformStates.HasFlag(UIKeyModifierFlags.NumericPad))
            virtualStates |= KeyboardStates.NumLock;

        return virtualStates;
    }
}
