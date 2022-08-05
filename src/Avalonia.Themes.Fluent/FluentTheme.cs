using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

#nullable enable

namespace Avalonia.Themes.Fluent
{
    public enum DensityStyle
    {
        Normal,
        Compact
    }

    /// <summary>
    /// Includes the fluent theme in an application.
    /// </summary>
    public class FluentTheme : AvaloniaObject, IStyle, IResourceProvider
    {
        private readonly Uri _baseUri;
        private IStyle? _densityStyles;
        private bool _isLoading;
        private IStyle? _loaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentTheme"/> class.
        /// </summary>
        /// <param name="baseUri">The base URL for the XAML context.</param>
        public FluentTheme(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentTheme"/> class.
        /// </summary>
        /// <param name="serviceProvider">The XAML service provider.</param>
        public FluentTheme(IServiceProvider serviceProvider)
        {
            var ctx  = serviceProvider.GetService(typeof(IUriContext)) as IUriContext
                 ?? throw new NullReferenceException("Unable retrive UriContext");
            _baseUri = ctx.BaseUri;
        }

        public static readonly StyledProperty<DensityStyle> DensityStyleProperty =
            AvaloniaProperty.Register<FluentTheme, DensityStyle>(nameof(DensityStyle));
        
        /// <summary>
        /// Gets or sets the density style of the fluent theme (normal, compact).
        /// </summary>
        public DensityStyle DensityStyle
        {
            get => GetValue(DensityStyleProperty);
            set => SetValue(DensityStyleProperty, value);
        }
        
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            
            if (_loaded is null)
            {
                // If style wasn't yet loaded, no need to change children styles,
                // it will be applied later in Loaded getter.
                return;
            }

            if (change.Property == DensityStyleProperty)
            {
                if (DensityStyle == DensityStyle.Compact)
                {
                    (Loaded as Styles)!.Add(_densityStyles);
                }
                else if (DensityStyle == DensityStyle.Normal)
                {
                    (Loaded as Styles)!.Remove(_densityStyles);
                }
            }
        }

        public IResourceHost? Owner => (Loaded as IResourceProvider)?.Owner;

        /// <summary>
        /// Gets the loaded style.
        /// </summary>
        public IStyle Loaded
        {
            get
            {
                if (_loaded == null)
                {
                    _isLoading = true;

                    var style = new Styles();
                    style.Resources.MergedDictionaries.Add(new ResourceInclude(_baseUri)
                    {
                        Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/Base.xaml")
                    });
                    style.Add(new StyleInclude(_baseUri)
                    {
                        Source = new Uri("avares://Avalonia.Themes.Fluent/Controls/FluentControls.xaml")
                    });

                    _loaded = style;

                    _densityStyles ??= new StyleInclude(_baseUri)
                    {
                        Source = new Uri("avares://Avalonia.Themes.Fluent/DensityStyles/Compact.xaml")
                    };
                    
                    if (DensityStyle == DensityStyle.Compact)
                    {
                        (_loaded as Styles)!.Add(_densityStyles);
                    }

                    _isLoading = false;
                }

                return _loaded!;
            }
        }

        bool IResourceNode.HasResources => (Loaded as IResourceProvider)?.HasResources ?? false;

        IReadOnlyList<IStyle> IStyle.Children => _loaded?.Children ?? Array.Empty<IStyle>();

        public event EventHandler? OwnerChanged
        {
            add
            {
                if (Loaded is IResourceProvider rp)
                {
                    rp.OwnerChanged += value;
                }
            }
            remove
            {
                if (Loaded is IResourceProvider rp)
                {
                    rp.OwnerChanged -= value;
                }
            }
        }

        public SelectorMatchResult TryAttach(IStyleable target, object? host) => Loaded.TryAttach(target, host);

        public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
        {
            if (!_isLoading && Loaded is IResourceProvider p)
            {
                return p.TryGetResource(key, theme, out value);
            }

            value = null;
            return false;
        }

        void IResourceProvider.AddOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.AddOwner(owner);

        void IResourceProvider.RemoveOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.RemoveOwner(owner);
    }
}
