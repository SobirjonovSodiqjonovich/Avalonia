using System;

using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using Avalonia.UnitTests;

using BenchmarkDotNet.Attributes;

namespace Avalonia.Benchmarks.Themes
{
    [MemoryDiagnoser]
    public class ThemeBenchmark : IDisposable
    {
        private IDisposable _app;

        public ThemeBenchmark()
        {
            AssetLoader.RegisterResUriParsers();

            _app = UnitTestApplication.Start(TestServices.StyledWindow.With(theme: () => null));
            // Add empty style to override it later
            UnitTestApplication.Current.Styles.Add(new Style());
        }

        [Benchmark]
        public bool InitFluentTheme()
        {
            UnitTestApplication.Current.Styles[0] = new FluentTheme(new Uri("resm:Styles?assembly=Avalonia.Benchmarks"));
            return ((IResourceHost)UnitTestApplication.Current).TryGetResource("SystemAccentColor", ThemeVariant.Dark, out _);
        }

        [Benchmark]
        public bool InitSimpleTheme()
        {
            UnitTestApplication.Current.Styles[0] = new SimpleTheme(new Uri("resm:Styles?assembly=Avalonia.Benchmarks"));
            return ((IResourceHost)UnitTestApplication.Current).TryGetResource("ThemeAccentColor", ThemeVariant.Dark, out _);
        }

        public void Dispose()
        {
            _app.Dispose();
        }
    }
}
