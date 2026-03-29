using ReactiveUI;
using System.Reactive;
using TableJson.Services;

namespace TableJson.ViewModels
{
    public class SettingsWindowViewModel : ViewModelBase
    {
        private SettingsService _settingsService;
        private string _NewTabFontSize;
        public string NewTabFontSize
        {
            get => _NewTabFontSize;
            set => this.RaiseAndSetIfChanged(ref _NewTabFontSize, value);
        }
        private string _MainWindowColor;
        public string MainWindowColor
        {
            get => _MainWindowColor;
            set => this.RaiseAndSetIfChanged(ref _MainWindowColor, value);
        }
        private string _MacrosWindowColor;
        public string MacrosWindowColor
        {
            get => _MacrosWindowColor;
            set => this.RaiseAndSetIfChanged(ref _MacrosWindowColor, value);
        }
        private string _MacrosEditorColor;
        public string MacrosEditorColor
        {
            get => _MacrosEditorColor;
            set => this.RaiseAndSetIfChanged(ref _MacrosEditorColor, value);
        }
        private string _MacrosEditorTextColor;
        public string MacrosEditorTextColor
        {
            get => _MacrosEditorTextColor;
            set => this.RaiseAndSetIfChanged(ref _MacrosEditorTextColor, value);
        }
        private string _ContextMenuTextColor;
        public string ContextMenuTextColor
        {
            get => _ContextMenuTextColor;
            set => this.RaiseAndSetIfChanged(ref _ContextMenuTextColor, value);
        }
        private string _ContextMenuItemColor;
        public string ContextMenuItemColor
        {
            get => _ContextMenuItemColor;
            set => this.RaiseAndSetIfChanged(ref _ContextMenuItemColor, value);
        }
        private string _ContextMenuHotkeyTextColor;
        public string ContextMenuHotkeyTextColor
        {
            get => _ContextMenuHotkeyTextColor;
            set => this.RaiseAndSetIfChanged(ref _ContextMenuHotkeyTextColor, value);
        }
        private string _TabWindowColor;
        public string TabWindowColor
        {
            get => _TabWindowColor;
            set => this.RaiseAndSetIfChanged(ref _TabWindowColor, value);
        }
        private string _TabWindowTextColor;
        public string TabWindowTextColor
        {
            get => _TabWindowTextColor;
            set => this.RaiseAndSetIfChanged(ref _TabWindowTextColor, value);
        }
        public void SaveSettings()
        {
            AppSettings newSettings = new() { ContextMenuHotkeyTextColor = ContextMenuHotkeyTextColor, ContextMenuItemColor = ContextMenuItemColor, ContextMenuTextColor = ContextMenuTextColor, MacrosEditorColor = MacrosEditorColor, MacrosEditorTextColor = MacrosEditorTextColor, MacrosWindowColor = MacrosWindowColor, MainWindowColor = MainWindowColor, NewTabFontSize = NewTabFontSize, TabWindowColor = TabWindowColor, TabWindowTextColor = TabWindowTextColor };
            _settingsService.Save(newSettings);
        }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public SettingsWindowViewModel()
        {
            _settingsService = new SettingsService();
            AppSettings _settings = _settingsService.Load();
            NewTabFontSize = _settings.NewTabFontSize;
            MainWindowColor = _settings.MainWindowColor;
            MacrosWindowColor = _settings.MacrosWindowColor;
            MacrosEditorColor = _settings.MacrosEditorColor;
            MacrosEditorTextColor = _settings.MacrosEditorTextColor;
            ContextMenuTextColor = _settings.ContextMenuTextColor;
            ContextMenuItemColor = _settings.ContextMenuItemColor;
            ContextMenuHotkeyTextColor = _settings.ContextMenuHotkeyTextColor;
            TabWindowColor = _settings.TabWindowColor;
            TabWindowTextColor = _settings.TabWindowTextColor;
            SaveCommand = ReactiveCommand.Create(SaveSettings);
        }
    }
}
