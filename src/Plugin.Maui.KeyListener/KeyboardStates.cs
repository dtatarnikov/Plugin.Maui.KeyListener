namespace Plugin.Maui.KeyListener;

[Flags]
public enum KeyboardStates
{
    None = 0,
    CapsLock = 1,
    NumLock = 2,
    ScrollLock = 4,
    Insert = 8
}

public static partial class KeyboardStatesExtensions
{

}
