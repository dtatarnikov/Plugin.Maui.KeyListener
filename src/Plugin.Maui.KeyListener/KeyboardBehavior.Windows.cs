using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Text;
using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Plugin.Maui.KeyListener;

public partial class KeyboardBehavior : PlatformBehavior<VisualElement>
{
    private readonly byte[] _keyboardState = new byte[256];
    private readonly char[] _char = new char[4];

	protected override void OnAttachedTo(VisualElement bindable, FrameworkElement platformView)
	{
		base.OnAttachedTo(bindable, platformView);

		if (platformView.XamlRoot?.Content is FrameworkElement content)
		{
			content.PreviewKeyDown += OnKeyDown;
			content.PreviewKeyUp += OnKeyUp;
		}
	}

	protected override void OnDetachedFrom(VisualElement bindable, FrameworkElement platformView)
	{
		if (platformView.XamlRoot?.Content is FrameworkElement content)
		{
			content.PreviewKeyDown -= OnKeyDown;
			content.PreviewKeyUp -= OnKeyUp;
		}

		base.OnDetachedFrom(bindable, platformView);
	}

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
	{
		var eventArgs = ToKeyPressedEventArgs(e);
		RaiseKeyDown(eventArgs);
		if (eventArgs.Handled)
			e.Handled = true;
	}

    private void OnKeyUp(object sender, KeyRoutedEventArgs e)
	{
		var eventArgs = ToKeyPressedEventArgs(e);
		RaiseKeyUp(eventArgs);
		if (eventArgs.Handled)
			e.Handled = true;
	}
	
    private KeyPressedEventArgs ToKeyPressedEventArgs(KeyRoutedEventArgs e)
    {
        var key = e.Key.IsModifierKey() ? (VirtualKey)Windows.Win32.PInvoke.MapVirtualKey(e.KeyStatus.ScanCode, MAP_VIRTUAL_KEY_TYPE.MAPVK_VSC_TO_VK_EX) : e.Key;
        
        Windows.Win32.PInvoke.GetKeyboardState(_keyboardState);
        var charCount = !e.Key.IsModifierKey() ? Windows.Win32.PInvoke.ToUnicode((uint)key, e.KeyStatus.ScanCode, _keyboardState, _char, 0) : 0;

        return new KeyPressedEventArgs
        {
            Modifiers = KeyboardModifiersExtensions.GetKeyboardModifiers(),
			States = KeyboardStatesExtensions.GetKeyboardStates(),
            Key = key.ToKeyboardKeys(),
            KeyChar = charCount > 0 ? Encoding.UTF8.GetString(_char.Select(i => (byte)i).Take(charCount).ToArray()) : "", //Windows.Win32.PInvoke.MapVirtualKey((uint)vk, MAP_VIRTUAL_KEY_TYPE.MAPVK_VK_TO_CHAR)
        };
    }
}