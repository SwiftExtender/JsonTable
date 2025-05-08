using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using System;
using System.IO;
using TableJson.ViewModels;

namespace TableJson.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //this.Opened += StartFocusing;
            InitializeComponent();
            AddTabButton();
            AddTab("New", new TabWindow() {DataContext= new TabWindowViewModel() });
            //AddTab("New1", new TabWindow() {DataContext= new TabWindowViewModel() });
            //AddTab("New2", new TabWindow() {DataContext= new TabWindowViewModel() });
        }
        private void AddTab(string header, Control content)
        {
            var newItem = new TabItem()
            {
                Header = header,
                Content = content
            };
            TabControl multiTab = this.FindControl<TabControl>("MultiTab");
            multiTab.Items.Add(newItem);
        }
        private void AddTabButton()
        {
            var addButton = new Button { Content = "+" };
            addButton.Click += OnAddTabClicked;
            TabControl multiTab = this.FindControl<TabControl>("MultiTab");
            multiTab.Items.Add(addButton);
        }
        private void OnAddTabClicked(object? sender, RoutedEventArgs e)
        {
            TabControl multiTab = this.FindControl<TabControl>("MultiTab");
            var newTabNumber = multiTab.Items.Count;
            AddTab("New "+newTabNumber, new TabWindow() { DataContext = new TabWindowViewModel() });
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
            if (files.Count >= 1)
            {
                await using var stream = await files[0].OpenReadAsync();
                using var streamReader = new StreamReader(stream);
                var fileContent = await streamReader.ReadToEndAsync();
                TextEditor editor = this.FindControl<TextEditor>("editor");
                editor.Document = new TextDocument(fileContent);
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