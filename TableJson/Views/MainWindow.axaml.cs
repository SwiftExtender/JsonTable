using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using System;
using System.IO;

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
        private async void ImportJsonFile_Clicked(object sender, RoutedEventArgs args)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this);

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File with JSON",
                AllowMultiple = false
            });

            if (files.Count >= 1)
            {
                // Open reading stream from the first file.
                await using var stream = await files[0].OpenReadAsync();
                using var streamReader = new StreamReader(stream);
                // Reads all the content of file as a text.
                var fileContent = await streamReader.ReadToEndAsync();
                TextEditor editor = this.FindControl<TextEditor>("editor");
                editor.Document = new TextDocument(fileContent);
            }     
        }
    }
}