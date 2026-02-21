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
        internal double DynamicMaxHeight;

        public MainWindow()
        {
            this.Name = "TheHighestWindow";
            InitializeComponent();
            TabControl multiTab = GetMultiTab();
            AddTabButton(multiTab);
            TabItem initTab = AddTab("New " + multiTab.Items.Count, new TabWindow() { DataContext = new TabWindowViewModel() });
            if (initTab != null)
            {
                initTab.IsSelected = true;
                initTab.Focus();
            }
        }
        private TabControl GetMultiTab()
        {
            TabControl multiTab = this.FindControl<TabControl>("HighestMultiTab");
            return multiTab;
        }
        private TabItem GetActiveTab()
        {
            TabControl multiTab = GetMultiTab();
            return (TabItem)multiTab.SelectedItem;
        }
        private Button AddTabDeleteButton()
        {
            TabControl multiTab = GetMultiTab();
            Button btn = new Button() { Content = "X" };
            btn.Click += (sender, e) =>
            {
                if (sender is Button btn && btn.Parent is DockPanel dckPanel && dckPanel.Parent is TabItem titem)
                {
                    multiTab.Items.Remove(titem);
                }
            };
            return btn;
        }
        private TabItem AddTab(string header, TabWindow content)
        {
            var panel = new DockPanel();
            panel.Children.Add(new Label() { Content = header });
            Button btn = AddTabDeleteButton();
            panel.Children.Add(btn);
            var newItem = new TabItem()
            {
                Header = panel,
                Content = content,
            };
            TabControl multiTab = GetMultiTab();
            multiTab.Items.Add(newItem);
            return newItem;
        }
        private TabItem AddTab(IStorageFile file, Control content)
        {
            var panel = new DockPanel();
            panel.Children.Add(new Label() { Content = file.Name });
            Button btn = AddTabDeleteButton();
            panel.Children.Add(btn);
            var newItem = new TabItem()
            {
                Header = panel,
                Content = content,
            };

            TabControl multiTab = GetMultiTab();
            multiTab.Items.Add(newItem);
            return newItem;
        }
        private void AddTabButton(TabControl multiTab)
        {
            var addButton = new Button { Content = "+" };
            addButton.Click += (sender, e) =>
            {
                AddTab("New " + multiTab.Items.Count, new TabWindow() { DataContext = new TabWindowViewModel() });
            };
            var addTabItem = new TabItem()
            {
                Header = addButton,
                //Content = addButton,
            };
            multiTab.Items.Add(addTabItem);
        }
        //private void StartFocusing(object sender, EventArgs arg)
        //{
        //    //TextEditor focused = this.FindControl<TextEditor>("editor");
        //    TabItem tab = GetActiveTab();
        //    TabWindow? tabWindow = tab.Content as TabWindow; //getting child TabWindow
        //    TabWindowViewModel? tabWindowViewModel = tabWindow.DataContext as TabWindowViewModel;
        //    tab.FontSize = tabWindowViewModel.EditorFontSizes;
        //    TabWindowViewModel vm = tab.DataContext;
        //    if (tab != null)
        //    {
        //        tab.Loaded += (s, e) => tab.Focus();
        //    }
        //}
        public async void MacrosOpenWindow_Clicked(object sender, RoutedEventArgs args)
        {
            MacrosCodeWindow w1 = new MacrosCodeWindow() { DataContext = new MacrosWindowViewModel(), WindowState = WindowState.Normal };
            w1.Show();
        }
        public async void OpenFile_Clicked(object sender, RoutedEventArgs args)
        {
            var topLevel = GetTopLevel(this);
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File with JSON",
                AllowMultiple = false
            });
            if (files.Count == 1)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TabWindow fromFileTab = new TabWindow() { };
                    fromFileTab.DataContext = new TabWindowViewModel(files[0]);
                    AddTab(files[0], fromFileTab);
                });
            }
        }
        public async void SaveFile_Clicked(object sender, RoutedEventArgs args)
        {
            TabItem tab = GetActiveTab();
            TabWindow? tabWindow = tab.Content as TabWindow; //getting child TabWindow
            TabWindowViewModel? tabWindowViewModel = tabWindow.DataContext as TabWindowViewModel;
            if (tabWindowViewModel.FileFullPath == "")
            {
                var topLevel = GetTopLevel(this);
                var saveOptions = new FilePickerSaveOptions { Title = "Save new file" };
                var file = await topLevel.StorageProvider.SaveFilePickerAsync(saveOptions);
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
                        tab.Header = file.Name;
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
        public async void CopyText(object sender, RoutedEventArgs args)
        {
            //if (!string.IsNullOrEmpty(selectedText))
            //{
            //    var clipboard = GetTopLevel(sender).Clipboard;
            //    var dataObject = new DataObject();
            //    dataObject.Set(DataFormats.Text, selectedText);
            //    _mainWindow.Clipboard.SetDataObjectAsync(dataObject);
            //}
        }
    }
}