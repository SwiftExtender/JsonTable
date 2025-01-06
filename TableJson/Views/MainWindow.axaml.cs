using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit;
using System;
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
        public void MacrosCreationWindowClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var w1 = new MacrosCodeWindow() {DataContext = new MacrosCodeWindowViewModel(btn.DataContext)};
            w1.Show();
        }
        //public void RemoveMacrosClick(object sender, RoutedEventArgs e)
        //{
        //    var btn = (Button)sender;
        //    try
        //    {
        //        TableJson.ViewModels.MainWindowViewModel. RemoveMacros(btn.DataContext);
        //    } catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}public void SaveMacrosClick(object sender, RoutedEventArgs e)
        //{
        //    var btn = (Button)sender;
        //    try
        //    {
        //        SaveMacros(btn.DataContext);
        //    } catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        private void StartFocusing(object sender, EventArgs arg)
        {
            TextEditor focused = this.FindControl<TextEditor>("editor");
            focused.FontSize = 13;
            if (focused != null)
            {
                focused.Loaded += (s, e) => focused.Focus();
            }
        }
    }
}