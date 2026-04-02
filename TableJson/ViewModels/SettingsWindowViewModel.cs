using Avalonia;
using ReactiveUI;
using System.Reactive;

namespace TableJson.ViewModels
{
    public class SettingsWindowViewModel : ViewModelBase
    {
        public string NewTabFontSize
        {
            get => (Application.Current as App).Settings.NewTabFontSize;
            set { (Application.Current as App).Settings.NewTabFontSize = value; }
        }
        public string MainWindowColor
        {
            get => (Application.Current as App).Settings.MainWindowColor;
            set { (Application.Current as App).Settings.MainWindowColor = value; }
        }
        public string MacrosWindowColor
        {
            get => (Application.Current as App).Settings.MacrosWindowColor;
            set { (Application.Current as App).Settings.MacrosWindowColor = value; }
        }
        public string MacrosEditorColor
        {
            get => (Application.Current as App).Settings.MacrosEditorColor;
            set { (Application.Current as App).Settings.MacrosEditorColor = value; }
        }
        public string MacrosEditorTextColor
        {
            get => (Application.Current as App).Settings.MacrosEditorTextColor;
            set { (Application.Current as App).Settings.MacrosEditorTextColor = value; }
        }
        public string ContextMenuTextColor
        {
            get => (Application.Current as App).Settings.ContextMenuTextColor;
            set { (Application.Current as App).Settings.ContextMenuTextColor = value; }
        }
        public string ContextMenuItemColor
        {
            get => (Application.Current as App).Settings.ContextMenuItemColor;
            set { (Application.Current as App).Settings.ContextMenuItemColor = value; }
        }
        public string ContextMenuHotkeyTextColor
        {
            get => (Application.Current as App).Settings.ContextMenuHotkeyTextColor;
            set { (Application.Current as App).Settings.ContextMenuHotkeyTextColor = value; }
        }
        public string TabWindowColor
        {
            get => (Application.Current as App).Settings.TabWindowColor;
            set { (Application.Current as App).Settings.TabWindowColor = value; }
        }
        public string TabWindowTextColor
        {
            get => (Application.Current as App).Settings.TabWindowTextColor;
            set { (Application.Current as App).Settings.TabWindowTextColor = value; }
        }
        private string _SettingsWindowColor = "";
        public string SettingsWindowColor
        {
            get { return _SettingsWindowColor; }
            set => this.RaiseAndSetIfChanged(ref _SettingsWindowColor, value);
        }
        public void SaveSettings()
        {
            (Application.Current as App).Settings.ContextMenuHotkeyTextColor = ContextMenuHotkeyTextColor;
            (Application.Current as App).Settings.ContextMenuItemColor = ContextMenuItemColor;
            (Application.Current as App).Settings.ContextMenuTextColor = ContextMenuTextColor;
            (Application.Current as App).Settings.MacrosEditorColor = MacrosEditorColor;
            (Application.Current as App).Settings.MacrosEditorTextColor = MacrosEditorTextColor;
            (Application.Current as App).Settings.MacrosWindowColor = MacrosWindowColor;
            (Application.Current as App).Settings.MainWindowColor = MainWindowColor;
            (Application.Current as App).Settings.NewTabFontSize = NewTabFontSize;
            (Application.Current as App).Settings.TabWindowColor = TabWindowColor;
            (Application.Current as App).Settings.TabWindowTextColor = TabWindowTextColor;
            (Application.Current as App).Settings.SettingsWindowColor = SettingsWindowColor;
            (Application.Current as App).SettingsService.Save((Application.Current as App).Settings);
        }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public SettingsWindowViewModel()
        {
            SaveCommand = ReactiveCommand.Create(SaveSettings);
            SettingsWindowColor = (Application.Current as App).Settings.SettingsWindowColor;
        }
    }
}
