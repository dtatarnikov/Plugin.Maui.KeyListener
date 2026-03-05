using System.Diagnostics;
using Android.Views;
using Android.Widget;
using View = Android.Views.View;

namespace Plugin.Maui.KeyListener;

public partial class KeyboardBehavior : PlatformBehavior<VisualElement>
{
	private View _attachedLayout;

	/// <summary>
	/// Similarly to the Apple and Windows implementations, find the outermost layout to connect the key events to.
	/// </summary>
	private static View GetParentLayout(View platformView)
	{
		View view = platformView;
		View layout = null;
		while (view.Parent is View parent)
		{
			view = parent;
			// TODO: The outermost layout is a FrameLayout, but no KeyPress events get raised for it in MAUI.
			if (view is LinearLayout parentLayout)
				layout = parentLayout;
		}

		return layout;
	}

	protected override void OnAttachedTo(VisualElement bindable, View platformView)
	{
		base.OnAttachedTo(bindable, platformView);

		var layout = GetParentLayout(platformView);
		if (layout is null)
		{
			Debug.WriteLine("[KeyboardBehavior] No suitable parent LinearLayout found. Keyboard events will not be received.");
			return;
		}

		_attachedLayout = layout;
		layout.KeyPress += OnKeyPress;
		layout.Focusable = true;
		layout.FocusableInTouchMode = true;
		layout.RequestFocus();
	}

	protected override void OnDetachedFrom(VisualElement bindable, View platformView)
	{
		if (_attachedLayout is not null)
		{
			_attachedLayout.KeyPress -= OnKeyPress;
			_attachedLayout = null;
		}

		base.OnDetachedFrom(bindable, platformView);
	}

    private void OnKeyPress(object sender, View.KeyEventArgs e)
    {
        var ev = e.Event;
        if (ev is null)
            return;

        var chars = ev.UnicodeChar;
        var isDead = (chars & KeyCharacterMap.CombiningAccent) != 0;
		var args = new KeyPressedEventArgs
		{
			Key = ev.KeyCode.ToKeyboardKeys(),
			Modifiers = ev.Modifiers.ToKeyboardModifiers(),
			States = ev.MetaState.ToKeyboardStates(), //KeyEvent.NormalizeMetaState(ev.MetaState)
			KeyChar = char.ConvertFromUtf32(isDead ? (ev.UnicodeChar & KeyCharacterMap.CombiningAccentMask) : ev.UnicodeChar) //char.ToUpper((char)ev.UnicodeChar)
		};

		switch (ev.Action)
		{
			case KeyEventActions.Down:
				RaiseKeyDown(args);
				break;
			case KeyEventActions.Up:
				RaiseKeyUp(args);
				break;
		}
	}
}
