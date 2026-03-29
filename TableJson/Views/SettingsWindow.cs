using Avalonia.Controls;
using Avalonia.Data;
using System.ComponentModel;
using TableJson.Converters;
using TableJson.Services;

namespace TableJson.Views
{
    public class SettingsWindow : Window
    {
        private SettingsService _settingsService;
        private AppSettings _settings;
        public void LoadSettings()
        {
            _settingsService = new SettingsService();
            _settings = _settingsService.Load();
        }
        public static ColorPicker ColorSetting(string path)
        {
            var binding = new Binding() {  Source = path, Converter = new ColorSetConverter() };
            var picker = new ColorPicker();
            picker.Bind(ColorPicker.ColorProperty, binding);
            return picker;
        }
        public SettingsWindow()
        {
            LoadSettings();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_settings);
            StackPanel panel = new();
            this.Content = panel;
            panel.Children.Add(new TextBlock() { Text = "Tab font size" });
            panel.Children.Add(new TextBox() { Text = _settings.NewTabFontSize.ToString() });
            panel.Children.Add(new TextBlock() { Text = "Main window color" });
            panel.Children.Add(ColorSetting(_settings.MainWindowColor));
            panel.Children.Add(new TextBlock() { Text = "Scripts window background color" });
            panel.Children.Add(ColorSetting(_settings.MacrosWindowColor));
            panel.Children.Add(new TextBlock() { Text = "Scripts editor background color" });
            panel.Children.Add(ColorSetting(_settings.MacrosEditorColor));
            panel.Children.Add(new TextBlock() { Text = "Scripts editor text color" });
            panel.Children.Add(ColorSetting(_settings.MacrosEditorTextColor));
            panel.Children.Add(new TextBlock() { Text = "Context menu items color" });
            panel.Children.Add(ColorSetting(_settings.ContextMenuTextColor));
            panel.Children.Add(new TextBlock() { Text = "Context menu labels color" });
            panel.Children.Add(ColorSetting(_settings.ContextMenuItemColor));
            panel.Children.Add(new TextBlock() { Text = "Context menu hotkey labels color" });
            panel.Children.Add(ColorSetting(_settings.ContextMenuHotkeyTextColor));
            panel.Children.Add(new TextBlock() { Text = "Tab background color" });
            panel.Children.Add(ColorSetting(_settings.TabWindowColor));
            panel.Children.Add(new TextBlock() { Text = "Tab text color" });
            panel.Children.Add(ColorSetting(_settings.TabWindowTextColor));
        }
    }
}
