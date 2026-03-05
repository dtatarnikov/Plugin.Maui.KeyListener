using System.Runtime.InteropServices;
using ObjCRuntime;
using UIKit;

namespace Plugin.Maui.KeyListener;

public static partial class KeyboardModifiersExtensions
{
#if MACCATALYST
    [DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
    private static extern nint int_objc_msgSend(IntPtr receiver, IntPtr selector);
#endif

    public static KeyboardModifiers GetKeyboardModifiers()
    {
        var modifiers = KeyboardModifiers.None;

#if MACCATALYST
        IntPtr nsEventClass = Class.GetHandle("NSEvent");
        var uiModifierFlags = (UIKeyModifierFlags)int_objc_msgSend(nsEventClass, Selector.GetHandle("modifierFlags")); //currentModifierFlags

        modifiers |= uiModifierFlags.ToKeyboardModifiers();
#endif

        return modifiers;
    }

    /// <remarks>
    /// https://developer.apple.com/documentation/uikit/uikeymodifierflags
    /// </remarks>
    internal static KeyboardModifiers ToKeyboardModifiers(this UIKeyModifierFlags platformModifiers)
    {
        KeyboardModifiers virtualModifiers = 0;

        if (platformModifiers.HasFlag(UIKeyModifierFlags.Shift))
            virtualModifiers |= KeyboardModifiers.Shift;
        if (platformModifiers.HasFlag(UIKeyModifierFlags.Control))
            virtualModifiers |= KeyboardModifiers.Control;
        if (platformModifiers.HasFlag(UIKeyModifierFlags.Alternate))
            virtualModifiers |= KeyboardModifiers.Alt;
        if (platformModifiers.HasFlag(UIKeyModifierFlags.Command))
            virtualModifiers |= KeyboardModifiers.Command;
        
        return virtualModifiers;
    }
}