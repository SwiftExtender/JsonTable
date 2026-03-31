using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.IO;
using System.Runtime;
using TableJson.Models;
using TableJson.Services;
using TableJson.ViewModels;
using TableJson.Views;

namespace TableJson
{
    public partial class App : Application
    {
        public AppSettings Settings;
        public SettingsService SettingsService;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.Default;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            //AppContext.SetData("GCHeapHardLimit", 0x1600000000);
            AppContext.SetData("GCAllowVeryLargeObjects", true);
            var tempdb = new HelpContext();
            tempdb.Database.EnsureCreated();
            Directory.CreateDirectory("imports");
            SettingsService = new SettingsService();
            SettingsService.CreateDefaultConfig();
            Settings = SettingsService.Load();
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