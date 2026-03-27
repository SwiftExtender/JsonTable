using TableJson.Services;

namespace TableJson.ViewModels
{
    public class SettingsWindowViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly AppSettings _settings;

        public SettingsWindowViewModel()
        {
            _settingsService = new SettingsService();
            _settings = _settingsService.Load();
        }
    }
}
