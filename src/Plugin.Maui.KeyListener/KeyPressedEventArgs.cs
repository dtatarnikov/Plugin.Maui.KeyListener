namespace Plugin.Maui.KeyListener;

public sealed class KeyPressedEventArgs : EventArgs
{
	public KeyboardStates States { get; internal set; }

	public KeyboardModifiers Modifiers { get; internal set; }

	public KeyboardKeys Key { get; internal set; }

	public string KeyChar { get; internal set; }

	public bool Handled { get; set; }
}