using Avalonia;
using ReactiveUI;
using TableJson.Services;

namespace TableJson.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public AppSettings AppSettings { get; set; }
        private bool _IsPinnedWindow = false;
        public bool IsPinnedWindow
        {
            get => _IsPinnedWindow;
            set => this.RaiseAndSetIfChanged(ref _IsPinnedWindow, value);
        }
        //private string _WindowColor;
        public string MainWindowColor
        {
            get => (Application.Current as App).Settings.MainWindowColor;
            set { 
                this.RaisePropertyChanging();
                (Application.Current as App).Settings.MainWindowColor = value;
                this.RaisePropertyChanged();
            }
            //get => _WindowColor;
            //set => this.RaiseAndSetIfChanged(ref _WindowColor, value);
        }
        //public void LoadSettings()
        //{
        //    SettingsService settingsService = new SettingsService();
        //    AppSettings = settingsService.Load();
        //    settingsService.CreateDefaultConfig();
        //}
        public MainWindowViewModel()
        {
            //LoadSettings();
            //WindowColor = AppSettings.MainWindowColor;
        }
    }
}
