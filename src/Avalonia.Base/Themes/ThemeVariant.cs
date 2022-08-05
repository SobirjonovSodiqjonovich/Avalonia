using System;
using System.ComponentModel;

namespace Avalonia;

[TypeConverter(typeof(ThemeVariantTypeConverter))]
public class ThemeVariant
{
    public ThemeVariant(object key)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }
        
    public ThemeVariant(object key, ThemeVariant? inheritVariant)
        : this(key)
    {
        InheritVariant = inheritVariant;
    }

    public static ThemeVariant Light { get; } = new(nameof(Light));
    public static ThemeVariant Dark { get; } = new(nameof(Dark));

    public object Key { get; }

    public ThemeVariant? InheritVariant { get; }
        
    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is ThemeVariant theme && Key.Equals(theme.Key);
    }

    public override string ToString()
    {
        return Key.ToString() ?? nameof(ThemeVariant);
    }
    
    public static bool operator ==(ThemeVariant? left, ThemeVariant? right)
    {
        return Equals(left, right);
    }
    
    public static bool operator !=(ThemeVariant? left, ThemeVariant? right)
    {
        return !Equals(left, right);
    }
}
