using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit;
using System;

namespace TableJson.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Opened += StartFocusing;
            InitializeComponent();
            //HotKeyManager.SetHotKey(saveMenuItem, new KeyGesture(Key.S, KeyModifiers.Control));
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
            if (focused != null)
            {
                focused.Loaded += (s, e) => focused.Focus();
            }
        }
    }
}