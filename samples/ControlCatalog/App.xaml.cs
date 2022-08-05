using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Simple;
using Avalonia.Themes.Fluent;
using ControlCatalog.ViewModels;

namespace ControlCatalog
{
    public class App : Application
    {
        public App()
        {
            DataContext = new ApplicationViewModel();
        }

        public static readonly StyleInclude ColorPickerFluent = new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
        {
            Source = new Uri("avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml")
        };

        public static readonly StyleInclude ColorPickerSimple = new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
        {
            Source = new Uri("avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml")
        };

        public static readonly StyleInclude DataGridFluent = new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
        {
            Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml")
        };

        public static readonly StyleInclude DataGridSimple = new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
        {
            Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml")
        };

        public static FluentTheme Fluent = new FluentTheme(new Uri("avares://ControlCatalog/Styles"));

        public static SimpleTheme Simple = new SimpleTheme(new Uri("avares://ControlCatalog/Styles"));

        public override void Initialize()
        {
            Styles.Insert(0, Fluent);
            Styles.Insert(1, ColorPickerFluent);
            Styles.Insert(2, DataGridFluent);
            
            // Styles.Insert(0, Simple);
            // Styles.Insert(1, ColorPickerSimple);
            // Styles.Insert(2, DataGridSimple);
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                desktopLifetime.MainWindow = new MainWindow();
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
            {
                singleViewLifetime.MainView = new MainView();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
