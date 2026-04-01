using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System;
using System.IO;
using TableJson.ViewModels;

namespace TableJson.Views
{
    public partial class MainWindow : Window
    {
        private SettingsWindow settingsWindow;
        private MacrosCodeWindow macrosWindow;
        public MainWindow()
        {
            Name = "TheHighestWindow";
            InitializeComponent();
            AddTabButton(HighestMultiTab);
            TabItem initTab = AddTab("New " + HighestMultiTab.Items.Count, new TabWindow());
            if (initTab != null)
            {
                initTab.IsSelected = true;
                initTab.Focus();
            }
        }
        private TabItem GetActiveTab()
        {
            return (TabItem)HighestMultiTab.SelectedItem;
        }
        private Button AddTabDeleteButton()
        {
            Button btn = new Button() { Content = "X" };
            btn.Click += (sender, e) =>
            {
                if (sender is Button btn && btn.Parent is DockPanel dckPanel && dckPanel.Parent is TabItem titem)
                {
                    HighestMultiTab.Items.Remove(titem);
                }
            };
            return btn;
        }
        private TabItem AddTab(string header, TabWindow content)
        {
            DockPanel panel = new DockPanel();
            panel.Children.Add(new Label() { Content = header });
            Button btn = AddTabDeleteButton();
            panel.Children.Add(btn);
            TabItem newItem = new TabItem()
            {
                Header = panel,
                Content = content,
            };
            HighestMultiTab.Items.Add(newItem);
            return newItem;
        }
        private TabItem AddTab(IStorageFile file, TabWindow content)
        {
            DockPanel panel = new DockPanel();
            panel.Children.Add(new Label() { Content = file.Name });
            Button btn = AddTabDeleteButton();
            panel.Children.Add(btn);
            TabItem newItem = new TabItem()
            {
                Header = panel,
                Content = content,
            };
            HighestMultiTab.Items.Add(newItem);
            return newItem;
        }
        private void AddTabButton(TabControl multiTab)
        {
            Button addButton = new Button { Content = "+" };
            addButton.Click += (sender, e) =>
            {
                AddTab("New " + multiTab.Items.Count, new TabWindow());
            };
            TabItem addTabItem = new TabItem()
            {
                Header = addButton,
                //Content = addButton,
            };
            addButton.Focusable = false;
            multiTab.Items.Add(addTabItem);
        }
        private void MacrosOpenWindow_Clicked(object sender, RoutedEventArgs args)
        {
            if (macrosWindow != null)
            {
                macrosWindow.Activate();
            }
            else {
                macrosWindow = new MacrosCodeWindow() { DataContext = new MacrosWindowViewModel(), WindowState = WindowState.Maximized };
                macrosWindow.Closed += (s ,e) => macrosWindow = null;
                macrosWindow.Show();
            }
                
        }
        private void SettingsOpenWindow_Clicked(object sender, RoutedEventArgs args)
        {
            if (settingsWindow != null)
            {
                settingsWindow.Activate();
            }
            else
            {
                settingsWindow = new SettingsWindow() { DataContext = new SettingsWindowViewModel(), WindowState = WindowState.Maximized };
                settingsWindow.Closed += (s, e) => settingsWindow = null;
                settingsWindow.Show();
            }
        }
        private async void OpenFile_Clicked(object sender, RoutedEventArgs args)
        {
            TopLevel topLevel = GetTopLevel(this);
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File with JSON",
                AllowMultiple = false
            });
            if (files.Count == 1)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TabWindow fromFileTab = new TabWindow(files[0]);
                    AddTab(files[0], fromFileTab);
                });
            }
        }
        private async void SaveFile_Clicked(object sender, RoutedEventArgs args)
        {
            TabItem tab = GetActiveTab();
            TabWindow? tabWindow = tab.Content as TabWindow; //getting child TabWindow
            TabWindowViewModel? tabWindowViewModel = tabWindow.DataContext as TabWindowViewModel;
            if (tabWindowViewModel.FileFullPath == "")
            {
                TopLevel topLevel = GetTopLevel(this);
                FilePickerSaveOptions saveOptions = new FilePickerSaveOptions { Title = "Save new file" };
                IStorageFile file = await topLevel.StorageProvider.SaveFilePickerAsync(saveOptions);
                if (file != null)
                {
                    try
                    {
                        await using (var stream = await file.OpenWriteAsync())
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                await writer.WriteAsync(tabWindowViewModel.RawText.Text);
                            }
                        }

                        DockPanel panel = new DockPanel();
                        panel.Children.Add(new Label() { Content = file.Name });
                        Button btn = AddTabDeleteButton();
                        panel.Children.Add(btn);
                        tab.Header = panel;
                        file.Dispose();
                        string createdFilePath = file.TryGetLocalPath();
                        tabWindowViewModel.StatusText = "New file saved " + createdFilePath;
                        tabWindowViewModel.FileFullPath = createdFilePath;
                    }
                    catch (Exception e)
                    {
                        tabWindowViewModel.StatusText = "Exception: New file saving error " + e.ToString();
                    }
                }
            }
            else
            {
                try
                {
                    File.WriteAllText(tabWindowViewModel.FileFullPath, tabWindowViewModel.RawText.Text);
                    tabWindowViewModel.StatusText = "File saved " + tabWindowViewModel.FileFullPath;
                }
                catch (Exception e)
                {
                    tabWindowViewModel.StatusText = "Exception: file saving error " + e.ToString();
                }
            }
        }
    }
}