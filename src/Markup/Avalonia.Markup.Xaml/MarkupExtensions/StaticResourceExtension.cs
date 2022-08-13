using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Markup.Data;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Styling;

#nullable enable

namespace Avalonia.Markup.Xaml.MarkupExtensions
{
    public class StaticResourceExtension
    {
        public StaticResourceExtension()
        {
        }

        public StaticResourceExtension(object resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public object? ResourceKey { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var stack = serviceProvider.GetService<IAvaloniaXamlIlParentStackProvider>();
            var provideTarget = serviceProvider.GetService<IProvideValueTarget>();
            var themeVariant = (provideTarget.TargetObject as IStyleable)?.ThemeVariant;
            IResourceDictionary? containingDictionary = null;

            var targetType = provideTarget.TargetProperty switch
            {
                AvaloniaProperty ap => ap.PropertyType,
                PropertyInfo pi => pi.PropertyType,
                _ => null,
            };

            if (provideTarget.TargetObject is Setter setter)
            {
                targetType = setter.Property?.PropertyType;
            }

            var previousWasControlTheme = false;
            var firstParentVisited = false;

            // Look upwards though the ambient context for IResourceNodes
            // which might be able to give us the resource.
            foreach (var parent in stack.Parents)
            {
                if (parent is IResourceNode node && node.TryGetResource(ResourceKey, themeVariant, out var value))
                {
                    return ColorToBrushConverter.Convert(value, targetType);
                }
                
                // To get a fallback theme variant, check if static resource was invoked inside of the ResourceDictionary.ThemeDictionaries. 
                if (themeVariant is null && firstParentVisited && containingDictionary is not null)
                {
                    if (parent is IResourceDictionary parentDictionary
                        && parentDictionary.ThemeDictionaries
                            .FirstOrDefault(p => p.Value == containingDictionary).Key is { } key)
                    {
                        themeVariant = key;
                    }
                    containingDictionary = null;
                }
                
                if (!firstParentVisited)
                {
                    firstParentVisited = true;
                    containingDictionary = parent as IResourceDictionary;
                }

                // HACK: Temporary fix for #8678. Hard-coded to only work for the DevTools main
                // window as we don't want 3rd parties to start relying on this hack.
                //
                // We need to implement compile-time merging of resource dictionaries and this
                // hack can be removed.
                if (previousWasControlTheme &&
                    parent is ResourceDictionary hack &&
                    hack.Owner?.GetType().FullName == "Avalonia.Diagnostics.Views.MainWindow" &&
                    hack.Owner.TryGetResource(ResourceKey, themeVariant, out value))
                {
                    return ColorToBrushConverter.Convert(value, targetType);
                }

                previousWasControlTheme = parent is ControlTheme;
            }

            if (provideTarget.TargetObject is IControl target &&
                provideTarget.TargetProperty is PropertyInfo property)
            {
                // This is stored locally to avoid allocating closure in the outer scope.
                var localTargetType = targetType;
                var localInstance = this;
                
                DelayedBinding.Add(target, property, x => localInstance.GetValue(x, localTargetType));
                return AvaloniaProperty.UnsetValue;
            }

            throw new KeyNotFoundException($"Static resource '{ResourceKey}' not found.");
        }

        private object GetValue(IStyledElement control, Type? targetType)
        {
            return ColorToBrushConverter.Convert(control.FindResource(ResourceKey), targetType);
        }
    }
}

