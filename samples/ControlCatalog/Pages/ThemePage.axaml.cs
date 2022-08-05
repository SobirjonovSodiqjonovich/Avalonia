using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ControlCatalog.Pages
{
    public class ThemePage : UserControl
    {
        public static ThemeVariant Pink { get; } = new("Pink", ThemeVariant.Light);
        
        public ThemePage()
        {
            AvaloniaXamlLoader.Load(this);

            var selector = this.FindControl<ComboBox>("Selector")!;
            var themeControl = this.FindControl<ThemeControl>("ThemeControl")!;

            selector.Items = new[]
            {
                new ThemeVariant("Default"),
                ThemeVariant.Dark,
                ThemeVariant.Light,
                Pink
            };
            selector.SelectedIndex = 0;

            selector.SelectionChanged += (_, _) =>
            {
                var theme = (ThemeVariant)selector.SelectedItem!;
                if ((string)theme.Key == "Default")
                {
                    themeControl.ClearValue(ThemeControl.ThemeVariantProperty);
                }
                else
                {
                    themeControl.ThemeVariant = theme;
                }
            };
        }
    }
}
