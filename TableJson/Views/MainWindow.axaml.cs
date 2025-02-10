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
            this.Opened += StartFocusing;
            InitializeComponent();
        }
        public void HotKeyPointerWheelHandler(object sender, PointerWheelEventArgs args)
        {
            if ((args.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control)
            {
                TextEditor editor = this.FindControl<TextEditor>("editor");
                if (args.Delta.Y > 0 && editor.FontSize < 74)
                {
                    editor.FontSize += 1; args.Handled = true;
                }
                if (args.Delta.Y < 0 && editor.FontSize > 9)
                {
                    editor.FontSize -= 1; args.Handled = true;
                }
            }
        }
        private void StartFocusing(object sender, EventArgs arg)
        {
            TextEditor focused = this.FindControl<TextEditor>("editor");
            focused.FontSize = 13;
            if (focused != null)
            {
                focused.Loaded += (s, e) => focused.Focus();
            }
        }
        public async void CopyToClipboardFromListbox(object sender, TappedEventArgs e)
        {
            if (e.Source.GetType() == typeof(ListBox))
            {
                TextBox c = (TextBox)sender;
                var clipboard = GetTopLevel(c).Clipboard;
                var dataObject = new DataObject();

                dataObject.Set(DataFormats.Text, ((TextBlock)e.Source).Text);
                await clipboard.SetDataObjectAsync(dataObject);
            }
        }
        public async void CopyToClipboardFromJSONPathResult(object sender, RoutedEventArgs e)
        {
            TextEditor JSONPathResult = this.FindControl<TextEditor>("JSONPathResultEditor");
            var clipboard = GetTopLevel(JSONPathResult).Clipboard;
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, JSONPathResult.Document.Text);
            await clipboard.SetDataObjectAsync(dataObject);
        }
        public async void CopyToClipboardFromJSONKeys(object sender, RoutedEventArgs e)
        {
            ListBox JSONKeysList = this.FindControl<ListBox>("JSONKeysList");
            var clipboard = GetTopLevel(JSONKeysList).Clipboard;
            var dataObject = new DataObject();
            string keys = "";
            foreach (var item in JSONKeysList.ItemsSource)
            {
                keys += item + Environment.NewLine;
            }
            dataObject.Set(DataFormats.Text, keys);
            await clipboard.SetDataObjectAsync(dataObject);
        } 
        public async void CopyToClipboardFromJSONValues(object sender, RoutedEventArgs e)
        {
            ListBox JSONValuesList = this.FindControl<ListBox>("JSONValuesList");
            var clipboard = GetTopLevel(JSONValuesList).Clipboard;
            var dataObject = new DataObject();
            string values = "";
            foreach (var item in JSONValuesList.ItemsSource)
            {
                values += item + Environment.NewLine;
            }
            dataObject.Set(DataFormats.Text, values);
            await clipboard.SetDataObjectAsync(dataObject);
        }
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