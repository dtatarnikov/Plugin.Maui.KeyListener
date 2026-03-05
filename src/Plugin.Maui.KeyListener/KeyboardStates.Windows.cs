using Microsoft.UI.Input;
using Windows.System;
using Windows.UI.Core;

namespace Plugin.Maui.KeyListener;

public static partial class KeyboardStatesExtensions
{
    public static KeyboardStates GetKeyboardStates()
    {
        var states = KeyboardStates.None;
        if (IsKeyLocked(VirtualKey.CapitalLock))
            states |= KeyboardStates.CapsLock;
        if (IsKeyLocked(VirtualKey.NumberKeyLock))
            states |= KeyboardStates.NumLock;
        if (IsKeyLocked(VirtualKey.Scroll))
            states |= KeyboardStates.ScrollLock;
        if (IsKeyLocked(VirtualKey.Insert))
            states |= KeyboardStates.Insert;
        return states;
    }

    private static bool IsKeyLocked(VirtualKey key) => InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Locked);
}
