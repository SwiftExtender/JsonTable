using System;
using System.IO;
using System.Text.Json;

namespace TableJson.Services
{
    public class AppSettings()
    {
        public string DefaultContextMenuItemColor = "#D55C5C5C";
        public string DefaultContextMenuTextColor = "#FF0A0C01";
        public string DefaultContextMenuHotkryTextColor = "#FF0A0C01";
        public string DefaultMainWindowColor = "#1A1A1A";
        public string DefaultMacrosWindowColor = "#1A1A1A";
        public string DefaultTabWindowColor = "#FF0A0C01";
        public string DefaultTabWindowTextColor = "#FFFFE0";
        public int DefaultFontSize = 14;
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

