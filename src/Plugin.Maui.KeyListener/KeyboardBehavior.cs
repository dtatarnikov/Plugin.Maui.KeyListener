namespace Plugin.Maui.KeyListener;

public partial class KeyboardBehavior : PlatformBehavior<VisualElement>
{
	public event EventHandler<KeyPressedEventArgs> KeyDown;

	public event EventHandler<KeyPressedEventArgs> KeyUp;

	/// <summary>
	/// Raises the <see cref="KeyDown"/> event. Handlers are invoked on the UI thread.
	/// </summary>
	internal void RaiseKeyDown(KeyPressedEventArgs args) => KeyDown?.Invoke(this, args);

	/// <summary>
	/// Raises the <see cref="KeyUp"/> event. Handlers are invoked on the UI thread.
	/// </summary>
	internal void RaiseKeyUp(KeyPressedEventArgs args) => KeyUp?.Invoke(this, args);
}