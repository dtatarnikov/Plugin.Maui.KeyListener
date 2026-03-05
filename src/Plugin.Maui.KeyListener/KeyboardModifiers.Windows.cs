using Microsoft.UI.Input;
using Windows.System;
using Windows.UI.Core;

namespace Plugin.Maui.KeyListener;

public static partial class KeyboardModifiersExtensions
{
    public static KeyboardModifiers GetKeyboardModifiers()
    {
        var modifiers = KeyboardModifiers.None;
        if (IsKeyDown(VirtualKey.Control))
            modifiers |= KeyboardModifiers.Control;
        if (IsKeyDown(VirtualKey.Menu))
            modifiers |= KeyboardModifiers.Alt;
        if (IsKeyDown(VirtualKey.Shift))
            modifiers |= KeyboardModifiers.Shift;
        if (IsKeyDown(VirtualKey.LeftWindows) || IsKeyDown(VirtualKey.RightWindows))
            modifiers |= KeyboardModifiers.Command;
        return modifiers;
    }

    internal static KeyboardModifiers ToKeyboardModifiers(this VirtualKeyModifiers platformModifiers)
    {
        var virtualModifiers = KeyboardModifiers.None;

        if (platformModifiers.HasFlag(VirtualKeyModifiers.Control))
            virtualModifiers |= KeyboardModifiers.Control;
        if (platformModifiers.HasFlag(VirtualKeyModifiers.Menu))
            virtualModifiers |= KeyboardModifiers.Alt;
        if (platformModifiers.HasFlag(VirtualKeyModifiers.Shift))
            virtualModifiers |= KeyboardModifiers.Shift;
        if (platformModifiers.HasFlag(VirtualKeyModifiers.Windows))
            virtualModifiers |= KeyboardModifiers.Command;

        return virtualModifiers;
    }

    private static bool IsKeyDown(VirtualKey key) => InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Down);
}
