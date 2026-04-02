using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ReactiveUI;
using System;
using TableJson.ViewModels;

namespace TableJson.Views
{
    public partial class TabWindow : UserControl, IActivatableView
    {
        public TabWindow()
        {
            InitializeComponent();
            DataContext = new TabWindowViewModel();
            RowTip.Text = "-";
            ColumnTip.Text = "-";
            FontSize = Double.Parse((Application.Current as App).Settings.NewTabFontSize);
            AddHandler(PointerWheelChangedEvent, MouseWheelFontSizer, RoutingStrategies.Tunnel, true);
            AddHandler(KeyDownEvent, KeyboardFontSizer, RoutingStrategies.Tunnel, true);
            TextEditingControl.TextArea.Caret.PositionChanged += CaretPositionChanged;
            TabColorsUpdate();
        }
        public TabWindow(IStorageFile file)
        {
            InitializeComponent();
            DataContext = new TabWindowViewModel(file);
            RowTip.Text = "-";
            ColumnTip.Text = "-";
            AddHandler(PointerWheelChangedEvent, MouseWheelFontSizer, RoutingStrategies.Tunnel, true);
            AddHandler(KeyDownEvent, KeyboardFontSizer, RoutingStrategies.Tunnel, true);
            TextEditingControl.TextArea.Caret.PositionChanged += CaretPositionChanged;
            TabColorsUpdate();
        }
        private void TabColorsUpdate()
        {
            this.WhenActivated(disposables =>
            {
                (Application.Current as App).Settings.
                WhenAnyValue(x => x.TabWindowColor).
                Subscribe<string>(onNext: s =>
                {
                    TextEditingControl.Background = Brush.Parse(s);
                });
            (Application.Current as App).Settings.
                WhenAnyValue(x => x.TabWindowTextColor).
                Subscribe<string>(onNext: s =>
                {
                    TextEditingControl.Foreground = Brush.Parse(s);
                });
            });
        }
        private void MouseWheelFontSizer(object? sender, PointerWheelEventArgs e)
        {
            if (e.KeyModifiers != KeyModifiers.Control) return;
            if (e.Delta.Y > 0) FontSize = FontSize < 74 ? FontSize + 1 : 74;
            else FontSize = FontSize > 7 ? FontSize - 1 : 7;
            e.Handled = true;
        }
        private void KeyboardFontSizer(object sender, KeyEventArgs e)
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                if (e.PhysicalKey == PhysicalKey.Equal || e.PhysicalKey == PhysicalKey.NumPadAdd)
                {
                    FontSize = FontSize < 74 ? FontSize + 1 : 74;
                }
                else if (e.PhysicalKey == PhysicalKey.Minus || e.PhysicalKey == PhysicalKey.NumPadSubtract)
                {
                    FontSize = FontSize > 7 ? FontSize - 1 : 7;
                }
            }
        }
        private void CaretPositionChanged(object sender, EventArgs e)
        {
            RowTip.Text = TextEditingControl.TextArea.Caret.Line.ToString();
            ColumnTip.Text = TextEditingControl.TextArea.Caret.Column.ToString();
        }
    }
}