namespace Avalonia.Controls
{
    /// <summary>
    /// Decorator control that isolates controls subtree with locally defined <see cref="ThemeVariant"/> property.
    /// </summary>
    public class ThemeControl : Decorator
    {
        /// <summary>
        /// Defines the <see cref="ThemeVariant"/> property.
        /// </summary>
        public static readonly StyledProperty<ThemeVariant> ThemeVariantProperty =
            AvaloniaProperty.Register<ThemeControl, ThemeVariant>(
                nameof(ThemeVariant),
                inherits: true,
                defaultValue: ThemeVariant.Light);

        /// <summary>
        /// Gets or sets the UI theme variant that is used by the control (and its child elements) for resource determination.
        /// The UI theme you specify with ThemeVariant can override the app-level ThemeVariant.
        /// </summary>
        /// <remarks>
        /// To reset local value and inherit parent theme, call <see cref="ThemeControl.ClearValue(AvaloniaProperty)" /> with <see cref="ThemeVariantProperty"/> as an argument.
        /// </remarks>
        public ThemeVariant ThemeVariant
        {
            get => GetValue(ThemeVariantProperty);
            set => SetValue(ThemeVariantProperty, value);
        }
    }
}
