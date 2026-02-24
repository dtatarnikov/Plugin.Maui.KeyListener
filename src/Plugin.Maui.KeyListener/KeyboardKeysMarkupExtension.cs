namespace Plugin.Maui.KeyListener;

[ContentProperty(nameof(Key))]
public sealed class KeyboardKeysMarkupExtension : IMarkupExtension
{
    public string Key { get; set; } = string.Empty;

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.TryParse(typeof(KeyboardKeys), Key, out var enumValue) ? enumValue : KeyboardKeys.None;
    }
}