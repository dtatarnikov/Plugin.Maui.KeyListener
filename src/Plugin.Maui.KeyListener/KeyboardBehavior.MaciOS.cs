using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace Plugin.Maui.KeyListener;

public partial class KeyboardBehavior : PlatformBehavior<VisualElement>
{
	protected override void OnAttachedTo(VisualElement bindable, UIView platformView)
	{
		base.OnAttachedTo(bindable, platformView);

		var kvc = FindKeyboardPageViewController(bindable, platformView);
		if (kvc is null)
			return;

		kvc.RegisterKeyboardBehavior(this);
	}

	protected override void OnDetachedFrom(VisualElement bindable, UIView platformView)
	{
		base.OnDetachedFrom(bindable, platformView);

		var kvc = FindKeyboardPageViewController(bindable, platformView);
		if (kvc is null)
			return;

		kvc.UnregisterKeyboardBehavior(this);
	}

	/// <summary>
	/// Finds the <see cref="KeyboardPageViewController"/> by first checking the parent page's handler,
	/// then walking the UIViewController hierarchy as a fallback for nested navigation scenarios (e.g. Shell).
	/// </summary>
    private static KeyboardPageViewController FindKeyboardPageViewController(VisualElement bindable, UIView platformView)
	{
		// Try the parent page's handler first (fast path)
		var page = GetParentPage(bindable);
		if (page?.Handler is IPlatformViewHandler viewHandler &&
		    viewHandler.ViewController is KeyboardPageViewController kvc)
			return kvc;

		// Walk the native VC hierarchy for nested navigation (Shell, NavigationPage, etc.)
		var responder = platformView.NextResponder;
		while (responder is not null)
		{
			if (responder is KeyboardPageViewController found)
				return found;
			responder = responder.NextResponder;
		}

		return null;
	}

    private static Page GetParentPage(VisualElement element)
	{
		if (element is Page page)
			return page;

		Element currentElement = element;

		while (currentElement != null && currentElement is not Page)
			currentElement = currentElement.Parent;

		return currentElement as Page;
	}
}

internal sealed class KeyboardPageViewController : PageViewController
{
    private readonly List<WeakReference<KeyboardBehavior>> _keyboardBehaviors = new();

	internal KeyboardPageViewController(IView page, IMauiContext mauiContext)
		: base(page, mauiContext) { }

#if IOS // Workaround to allow receiving keys when the root page is a Shell
	public override bool CanBecomeFirstResponder => true;

	public override void ViewIsAppearing(bool animated)
	{
		base.ViewIsAppearing(animated);

		BecomeFirstResponder();
	}
#endif
    
    public override void PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
	{
		if (ProcessPresses(presses, evt, false))
			return;

		base.PressesBegan(presses, evt);
	}

	public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
	{
		if (ProcessPresses(presses, evt, true))
			return;

		base.PressesEnded(presses, evt);
	}
	
    public override void PressesCancelled(NSSet<UIPress> presses, UIPressesEvent evt)
    {
        ProcessPresses(presses, evt, true);

        base.PressesCancelled(presses, evt);
    }

	internal void RegisterKeyboardBehavior(KeyboardBehavior keyboardBehavior)
	{
		if (_keyboardBehaviors.Any(weakRef => WeakReferenceEquals(weakRef, keyboardBehavior)))
			return;

		_keyboardBehaviors.Add(new WeakReference<KeyboardBehavior>(keyboardBehavior));
    }

	internal void UnregisterKeyboardBehavior(KeyboardBehavior keyboardBehavior)
	{
		foreach (var weakRef in _keyboardBehaviors)
		{
			if (WeakReferenceEquals(weakRef, keyboardBehavior))
			{
				_keyboardBehaviors.Remove(weakRef);
				break;
			}
		}
	}

	/// <remarks>
	/// Special key 'CapsLock' is handled differently: when it ON - it fires keyDown event only, when it OFF - it fires keyUp event only.
	/// Try to avoid this behaviour to fire both keyDown and keyUp events manually
	/// </remarks>
    private bool ProcessPresses(NSSet<UIPress> presses, UIPressesEvent e, bool isKeyUp)
	{
		//cleanup targets
        _keyboardBehaviors.RemoveAll(weakRef => !weakRef.TryGetTarget(out _));

		if (_keyboardBehaviors.Count == 0)
			return false;

		var modifiers = e.ModifierFlags.ToKeyboardModifiers();
        var states = e.ModifierFlags.ToKeyboardStates();
		var handled = false;

		foreach (var press in presses)
		{
            var key = press.Key;
            if (key is null)
				continue;

            var chars = key.Characters; //key.CharactersIgnoringModifiers
			var args = new KeyPressedEventArgs
			{
				Modifiers = modifiers,
				States = states,
				Key = key.KeyCode.ToKeyboardKeys(),
				KeyChar = chars, //chars.ToUpperInvariant()
			};

			foreach (var weakBehavior in _keyboardBehaviors.ToArray())
			{
				if (weakBehavior.TryGetTarget(out var target) && target is not null)
				{
					if (isKeyUp)
                    {
                        if (args.Key == KeyboardKeys.CapsLock)
                            target.RaiseKeyDown(args);
                        target.RaiseKeyUp(args);
                    }
					else
                    {
                        target.RaiseKeyDown(args);
                        if (args.Key == KeyboardKeys.CapsLock)
                            target.RaiseKeyUp(args);
                    }

					if (args.Handled)
					{
						handled = true;
						break;
					}
				}
			}
		}

		return handled;
	}

    private static bool WeakReferenceEquals<T>(WeakReference<T> weakRef, T target) where T : class
    {
        return weakRef.TryGetTarget(out var currentTarget) && ReferenceEquals(currentTarget, target);
    }
}