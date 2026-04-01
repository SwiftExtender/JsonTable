using ReactiveUI;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace TableJson.Services
{
    public class AppSettings : ReactiveObject
    {
        private string _ContextMenuItemColor = "#D55C5C5C";
        public string ContextMenuItemColor
        {
            get { return _ContextMenuItemColor; }
            set => this.RaiseAndSetIfChanged(ref _ContextMenuItemColor, value);
        }
        private string _ContextMenuTextColor = "#FF0A0C01";
        public string ContextMenuTextColor
        {
            get { return _ContextMenuTextColor; }
            set => this.RaiseAndSetIfChanged(ref _ContextMenuTextColor, value);
        }
        private string _ContextMenuHotkeyTextColor = "#FF0A0C01";
        public string ContextMenuHotkeyTextColor
        {
            get { return _ContextMenuHotkeyTextColor; }
            set => this.RaiseAndSetIfChanged(ref _ContextMenuHotkeyTextColor, value);
        }
        private string _MainWindowColor = "#1A1A1A";
        public string MainWindowColor
        {
            get { return _MainWindowColor; }
            set => this.RaiseAndSetIfChanged(ref _MainWindowColor, value);
        }
        private string _MacrosWindowColor = "#1A1A1A";
        public string MacrosWindowColor
        {
            get { return _MacrosWindowColor; }
            set => this.RaiseAndSetIfChanged(ref _MacrosWindowColor, value);
        }
        private string _MacrosEditorColor = "#FF604213";
        public string MacrosEditorColor
        {
            get { return _MacrosEditorColor; }
            set => this.RaiseAndSetIfChanged(ref _MacrosEditorColor, value);
        }
        private string _MacrosEditorTextColor = "#FFFFE0";
        public string MacrosEditorTextColor
        {
            get { return _MacrosEditorTextColor; }
            set => this.RaiseAndSetIfChanged(ref _MacrosEditorTextColor, value);
        }
        private string _TabWindowColor = "#FF0A0C01";
        public string TabWindowColor
        {
            get { return _TabWindowColor; }
            set => this.RaiseAndSetIfChanged(ref _TabWindowColor, value);
        }
        private string _TabWindowTextColor = "#FFFFE0";
        public string TabWindowTextColor
        {
            get { return _TabWindowTextColor; }
            set => this.RaiseAndSetIfChanged(ref _TabWindowTextColor, value);
        }
        private string _NewTabFontSize = "14";
        public string NewTabFontSize
        {
            get { return _NewTabFontSize; }
            set => this.RaiseAndSetIfChanged(ref _NewTabFontSize, value);
        }
        private string _SettingsWindowColor = "#1A1A1A";
        public string SettingsWindowColor
        {
            get { return _SettingsWindowColor; }
            set => this.RaiseAndSetIfChanged(ref _SettingsWindowColor, value);
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

