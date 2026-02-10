using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Runtime;
using TableJson.Models;
using TableJson.ViewModels;
using TableJson.Views;

namespace TableJson
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            //AppContext.SetData("GCHeapHardLimit", 0x1600000000);
            AppContext.SetData("GCAllowVeryLargeObjects", true);
            var tempdb = new HelpContext();

            tempdb.Database.EnsureCreated();

            this.AttachDevTools();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    WindowState = Avalonia.Controls.WindowState.Maximized,
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}