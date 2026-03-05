using Microsoft.Maui.Handlers;

namespace Plugin.Maui.KeyListener;

public static class MauiProgramExtensions
{
	public static MauiAppBuilder UseKeyListener(this MauiAppBuilder builder)
	{
#if IOS || MACCATALYST
		PageHandler.PlatformViewFactory = (handler) =>
		{
			if (handler is not PageHandler)
				return null;

			if (handler.MauiContext is null)
				return null;

			var vc = new KeyboardPageViewController(handler.VirtualView, handler.MauiContext);
			handler.ViewController = vc;

			if (vc.View?.Subviews is not { Length: > 0 } subviews)
				return null;

			return (Microsoft.Maui.Platform.ContentView)subviews[0];
		};
#endif

		return builder;
	}
}