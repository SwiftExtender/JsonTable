using System;
using System.IO;
using System.Text.Json;

namespace TableJson.Services
{
    public class AppSettings
    {
        public string ContextMenuItemColor = "#D55C5C5C";
        public string ContextMenuTextColor = "#FF0A0C01";
        public string ContextMenuHotkeyTextColor = "#FF0A0C01";
        public string MainWindowColor = "#1A1A1A";
        public string MacrosWindowColor = "#1A1A1A";
        public string MacrosEditorColor = "#FF604213";
        public string MacrosEditorTextColor = "#FFFFE0";
        public string TabWindowColor = "#FF0A0C01";
        public string TabWindowTextColor = "#FFFFE0";
        public int FontSize = 14;
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

