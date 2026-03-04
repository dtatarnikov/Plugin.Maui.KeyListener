# Copilot Instructions for Plugin.Maui.KeyListener

## Build Commands

```bash
# Build the plugin
dotnet build src/Plugin.Maui.KeyListener.sln -c Release

# Build the sample app
dotnet build samples/Plugin.Maui.KeyListener.Sample.sln -c Release

# Pack NuGet (replace version as needed)
dotnet pack src/Plugin.Maui.KeyListener.sln -c Release -p:PackageVersion=1.0.0
```

There are no unit tests in this project.

## Architecture

This is a .NET MAUI plugin that provides `KeyboardBehavior` — a `PlatformBehavior<VisualElement>` that exposes `KeyDown` and `KeyUp` events with unified `KeyPressedEventArgs` (containing `Key`, `Modifiers`, `KeyChar`, and `Handled` properties).

### Platform-specific pattern

The plugin uses **partial classes** to split platform implementations:

- **Shared** (`KeyboardBehavior.cs`): Defines events and `RaiseKeyDown`/`RaiseKeyUp` methods
- **Android** (`KeyboardBehavior.Android.cs`): Traverses parent view tree to find the outermost `LinearLayout`, hooks its `KeyPress` event
- **iOS/Mac** (`KeyboardBehavior.MaciOS.cs` + `KeyboardPageViewController.MaciOS.cs`): Replaces `PageHandler.PlatformViewFactory` with a custom `KeyboardPageViewController` that overrides `PressesBegan`/`PressesEnded`/`PressesCancelled`. Uses weak references to track registered behaviors.
- **Windows** (`KeyboardBehavior.Windows.cs`): Hooks `XamlRoot.Content` PreviewKeyDown/PreviewKeyUp events on the root `FrameworkElement`

### Key type mapping

Each platform has bidirectional extension methods (`KeyboardKeysExtensions.{Platform}.cs` and `KeyboardModifiersExtensions.{Platform}.cs`) that convert between native key codes and the shared `KeyboardKeys`/`KeyboardModifiers` enums.

### Registration

Consumers call `.UseKeyListener()` in their `MauiProgram.cs`. On iOS/Mac this configures the `PageHandler` to use `KeyboardPageViewController`; on other platforms it's a no-op.

### XAML support

`KeyboardBehaviorTrigger` pairs with `KeyboardBehavior` for declarative key matching. `KeyboardKeysMarkupExtension` and `KeyboardModifiersMarkupExtension` provide XAML-friendly enum parsing.

## Conventions

- **Tabs for indentation** (4-wide) in C# files
- Platform-specific files use the naming convention `ClassName.{Platform}.cs` (e.g., `KeyboardBehavior.Android.cs`)
- The csproj conditionally excludes platform files that don't belong to the current target framework
- Target frameworks: `net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst`, `net9.0-windows10.0.19041.0`
- Windows builds use `Microsoft.Windows.CsWin32` for P/Invoke generation (configured via `NativeMethods.txt`)

## Release Process

Create a git tag matching `v1.0.0` or `v1.0.0-preview1` to trigger the NuGet release workflow. Requires the `NUGET_API_KEY` repository secret.
