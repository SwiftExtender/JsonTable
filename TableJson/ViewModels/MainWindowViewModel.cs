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
        private string _WindowColor;
        public string WindowColor
        {
            get => _WindowColor;
            set => this.RaiseAndSetIfChanged(ref _WindowColor, value);
        }
        public void LoadSettings()
        {
            SettingsService settingsService = new SettingsService();
            AppSettings = settingsService.Load();
        }
        public MainWindowViewModel()
        {
            LoadSettings();
            WindowColor = AppSettings.MainWindowColor;
        }
    }
}
