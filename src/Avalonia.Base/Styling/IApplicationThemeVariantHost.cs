using System;
using Avalonia.Controls;

namespace Avalonia.Styling;

/// <summary>
/// Interface for a host element with a root theme.
/// </summary>
public interface IApplicationThemeVariantHost : IResourceHost
{
    /// <summary>
    /// Gets the UI theme that is used by the control (and its child elements) for resource determination.
    /// </summary>
    ThemeVariant ThemeVariant { get; }

    /// <summary>
    /// Raised when the theme is changed on the element or an ancestor of the element.
    /// </summary>
    event EventHandler? ThemeVariantChanged;
}
