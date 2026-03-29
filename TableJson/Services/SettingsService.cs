using System;
using System.IO;
using System.Text.Json;

namespace TableJson.Services
{
    public class AppSettings
    {
        public string ContextMenuItemColor { get; set; } = "#D55C5C5C";
        public string ContextMenuTextColor { get; set; } = "#FF0A0C01";
        public string ContextMenuHotkeyTextColor { get; set; } = "#FF0A0C01";
        public string MainWindowColor { get; set; } = "#1A1A1A";
        public string MacrosWindowColor { get; set; } = "#1A1A1A";
        public string MacrosEditorColor { get; set; } = "#FF604213";
        public string MacrosEditorTextColor { get; set; } = "#FFFFE0";
        public string TabWindowColor { get; set; } = "#FF0A0C01";
        public string TabWindowTextColor { get; set; } = "#FFFFE0";
        public int NewTabFontSize { get; set; } = 14;
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

