#if MACCATALYST || IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace Plugin.Maui.KeyListener
{
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
		static KeyboardPageViewController? FindKeyboardPageViewController(VisualElement bindable, UIView platformView)
		{
			// Try the parent page's handler first (fast path)
			var page = GetParentPage(bindable);
			if (page?.Handler is IPlatformViewHandler viewHandler &&
				viewHandler.ViewController is KeyboardPageViewController kvc)
				return kvc;

			// Walk the native VC hierarchy for nested navigation (Shell, NavigationPage, etc.)
			UIResponder? responder = platformView.NextResponder;
			while (responder is not null)
			{
				if (responder is KeyboardPageViewController found)
					return found;
				responder = responder.NextResponder;
			}

			return null;
		}

		static Page? GetParentPage(VisualElement element)
		{
			if (element is Page)
				return element as Page;

			Element currentElement = element;

			while (currentElement != null && currentElement is not Page)
				currentElement = currentElement.Parent;

			return currentElement as Page;
		}
	}
}
#endif