using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using System;
using System.IO;
using System.Reflection.Metadata;
using TableJson.ViewModels;

namespace TableJson.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Name += "TheHighestWindow";
            InitializeComponent();
            AddTabButton();
            //AddTab("New", new TabWindow() {DataContext= new TabWindowViewModel() });
            //AddTab("New1", new TabWindow() {DataContext= new TabWindowViewModel() });
            //AddTab("New2", new TabWindow() {DataContext= new TabWindowViewModel() });
        }
        
        private void AddTab(string header, Control content)
        {
            TabControl multiTab = this.FindControl<TabControl>("HighestMultiTab");
            var panel = new DockPanel();
            panel.Children.Add(new Label() { Content = header });
            Button btn = new Button() { Content = "X" };
            btn.Click += (sender, e) =>
            {
                if (sender is Button btn && btn.Parent is DockPanel dckPanel && dckPanel.Parent is TabItem titem)
                {
                    multiTab.Items.Remove(titem);
                }
            };
            panel.Children.Add(btn);
            var newItem = new TabItem()
            {
                Header = panel,
                Content = content,
            };
            
            multiTab.Items.Add(newItem);
        }
        private void AddTabButton()
        {
            TabControl multiTab = this.FindControl<TabControl>("HighestMultiTab");
            var addButton = new Button { Content = "+" };
            addButton.Click += (sender, e) =>
            {
                
                var newTabNumber = multiTab.Items.Count;
                AddTab("New " + newTabNumber, new TabWindow() { DataContext = new TabWindowViewModel() });
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
        //    TextEditor focused = this.FindControl<TextEditor>("editor");
        //    focused.FontSize = 13;
        //    if (focused != null)
        //    {
        //        focused.Loaded += (s, e) => focused.Focus();
        //    }
        //}
        private async void ImportJsonFile_Clicked(object sender, RoutedEventArgs args)
        {
            var topLevel = GetTopLevel(this);
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File with JSON",
                AllowMultiple = false
            });
            if (files.Count == 1)
            {
                //await using var stream = await files[0].OpenReadAsync();
                //using var streamReader = new StreamReader(stream);
               //var fileContent = await streamReader.ReadToEndAsync();
                //TabControl multiTab = this.FindControl<TabControl>("HighestMultiTab");
                //TabWindow fromFileTab = new TabWindow() { }; //DataContext = new TabWindowViewModel()
                //AddTab(files[0].Name, fromFileTab);

                //TextEditor editor = fromFileTab.FindControl<TextEditor>("editor");
                //editor.Document = new TextDocument(fileContent);
                //MainWindowViewModel.DataRequested?.Invoke(this, dataToSend);
                //fromFileTab.Ch
                //TextEditor editor = this.FindControl<TextEditor>("editor");
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TabWindow fromFileTab = new TabWindow() { };
                    fromFileTab.DataContext = new TabWindowViewModel() { FilePath = files[0].Name };
                    AddTab(files[0].Name, fromFileTab);
                });

            }     
        }
        //public async void CopyText(object sender, RoutedEventArgs args)
        //{
        //    if (!string.IsNullOrEmpty(selectedText))
        //    {
        //        var clipboard = GetTopLevel(sender).Clipboard;
        //        var dataObject = new DataObject();
        //        dataObject.Set(DataFormats.Text, selectedText);
        //        _mainWindow.Clipboard.SetDataObjectAsync(dataObject);
        //    }
        //}
    }
}