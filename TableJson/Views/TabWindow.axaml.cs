using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit;
using System;

namespace TableJson.Views
{
    public partial class TabWindow : Window
    {
        public TabWindow()
        {
            InitializeComponent();
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
    }
}