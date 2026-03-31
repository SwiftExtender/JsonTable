using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace TableJson.Services
{
    public class AppSettings : INotifyPropertyChanged
    {
        private string _ContextMenuItemColor { get; set; } = "#D55C5C5C";
        public string ContextMenuItemColor {
            get { return _ContextMenuItemColor; }
            set
            {
                { _ContextMenuItemColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            } }
        private string _ContextMenuTextColor { get; set; } = "#FF0A0C01";
        public string ContextMenuTextColor
        {
            get { return _ContextMenuTextColor; }
            set
            {
                {
                    _ContextMenuTextColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _ContextMenuHotkeyTextColor { get; set; } = "#FF0A0C01";
        public string ContextMenuHotkeyTextColor
        {
            get { return _ContextMenuHotkeyTextColor; }
            set
            {
                {
                    _ContextMenuHotkeyTextColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _MainWindowColor { get; set; } = "#1A1A1A";
        public string MainWindowColor
        {
            get { return _MainWindowColor; }
            set
            {
                {
                    _MainWindowColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _MacrosWindowColor { get; set; } = "#1A1A1A";
        public string MacrosWindowColor
        {
            get { return _MacrosWindowColor; }
            set
            {
                {
                    _MacrosWindowColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _MacrosEditorColor { get; set; } = "#FF604213";
        public string MacrosEditorColor
        {
            get { return _MacrosEditorColor; }
            set
            {
                {
                    _MacrosEditorColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _MacrosEditorTextColor { get; set; } = "#FFFFE0";
        public string MacrosEditorTextColor
        {
            get { return _MacrosEditorTextColor; }
            set
            {
                {
                    _MacrosEditorTextColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _TabWindowColor { get; set; } = "#FF0A0C01";
        public string TabWindowColor
        {
            get { return _TabWindowColor; }
            set
            {
                {
                    _TabWindowColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _TabWindowTextColor { get; set; } = "#FFFFE0";
        public string TabWindowTextColor
        {
            get { return _TabWindowTextColor; }
            set
            {
                {
                    _TabWindowTextColor = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _NewTabFontSize { get; set; } = "14";
        public string NewTabFontSize
        {
            get { return _NewTabFontSize; }
            set
            {
                {
                    _NewTabFontSize = value;
                    OnPropertyChanged("ContextMenuItemColor");
                }
            }
        }
        private string _SettingsWindowTextColor { get; set; } = "#1A1A1A";
        public string SettingsWindowTextColor
        {
            get { return _SettingsWindowTextColor; }
            set
            {
                {
                    _SettingsWindowTextColor = value;
                    OnPropertyChanged("SettingsWindowTextColor");
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) { 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class SettingsService
    {
        private string _settingsPath;
        public AppSettings Load()
        {
            if (!File.Exists(_settingsPath))
            {
                return new AppSettings();
            }
            string settings = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(settings) ?? new AppSettings();
        }
        public void CreateDefaultConfig()
        {
            if (!File.Exists(_settingsPath)) {
                string config = JsonSerializer.Serialize(new AppSettings(), new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_settingsPath, config);
            }
        }
        public void Save(AppSettings settings)
        {
            var directory = Path.GetDirectoryName(_settingsPath)!;
            Directory.CreateDirectory(directory);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_settingsPath, json);
        }
        public SettingsService()
        {
            _settingsPath = Path.Join(Environment.CurrentDirectory, "settings.json");
        }
    }
}

