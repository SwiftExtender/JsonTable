using Avalonia.Controls;
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