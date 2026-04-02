using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using System;
using TableJson.ViewModels;

namespace TableJson.Views
{
    public partial class MacrosCodeWindow : Window, IActivatableView
    {
        public MacrosCodeWindow()
        {
            InitializeComponent();
            DataContext = new MacrosWindowViewModel();
            DataGrid grid = GetGrid();
            this.WhenActivated(disposables =>
            {
                (Application.Current as App).Settings.
                WhenAnyValue(x => x.MacrosWindowColor).
                Subscribe<string>(onNext: s =>
                {
                    this.Background = Brush.Parse(s);
                });
                (Application.Current as App).Settings.
                WhenAnyValue(x => x.MacrosEditorColor).
                Subscribe<string>(onNext: s =>
                {
                    Editor.Background = Brush.Parse(s);
                });
                (Application.Current as App).Settings.
                    WhenAnyValue(x => x.MacrosEditorTextColor).
                    Subscribe<string>(onNext: s =>
                    {
                        Editor.Foreground = Brush.Parse(s);
                    });
            });

        }
        private DataGrid GetGrid()
        {
            DataGrid grid = this.FindControl<DataGrid>("mgrid");
            grid.SelectionMode = DataGridSelectionMode.Single;
            return grid;
        }
    }
}
