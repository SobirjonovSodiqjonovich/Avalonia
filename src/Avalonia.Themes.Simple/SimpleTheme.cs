using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
#nullable enable

namespace Avalonia.Themes.Simple
{
    public class SimpleTheme : AvaloniaObject, IStyle, IResourceProvider
    {
        private readonly Uri _baseUri;
        private bool _isLoading;
        private IStyle? _loaded;
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTheme"/> class.
        /// </summary>
        /// <param name="baseUri">The base URL for the XAML context.</param>
        public SimpleTheme(Uri? baseUri = null)
        {
            _baseUri = baseUri ?? new Uri("avares://Avalonia.Themes.Simple/");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTheme"/> class.
        /// </summary>
        /// <param name="serviceProvider">The XAML service provider.</param>
        public SimpleTheme(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof(IUriContext));
            if (service == null)
            {
                throw new Exception("There is no service object of type IUriContext!");
            }
            _baseUri = ((IUriContext)service).BaseUri;
        }

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

        IReadOnlyList<IStyle> IStyle.Children => _loaded?.Children ?? Array.Empty<IStyle>();

        bool IResourceNode.HasResources => (Loaded as IResourceProvider)?.HasResources ?? false;

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
                        Source = new Uri("avares://Avalonia.Themes.Simple/Accents/Base.xaml")
                    });
                    style.Add(new StyleInclude(_baseUri)
                    {
                        Source = new Uri("avares://Avalonia.Themes.Simple/Controls/SimpleControls.xaml")
                    });

                    _loaded = style;

                    _isLoading = false;
                }

                return _loaded!;
            }
        }
        
        public IResourceHost? Owner => (Loaded as IResourceProvider)?.Owner;

        void IResourceProvider.AddOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.AddOwner(owner);

        void IResourceProvider.RemoveOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.RemoveOwner(owner);

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
    }
}
