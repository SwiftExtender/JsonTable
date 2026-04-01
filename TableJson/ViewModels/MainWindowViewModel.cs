using Avalonia;
using ReactiveUI;
using System;

namespace TableJson.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _IsPinnedWindow = false;
        public bool IsPinnedWindow
        {
            get => _IsPinnedWindow;
            set => this.RaiseAndSetIfChanged(ref _IsPinnedWindow, value);
        }
        private string _MainWindowColor = "";
        public string MainWindowColor
        {
            get => _MainWindowColor;
            set => this.RaiseAndSetIfChanged(ref _MainWindowColor, value);
        }
        public MainWindowViewModel()
        {
            (Application.Current as App).Settings.
                WhenAnyValue(x => x.MainWindowColor).
                Subscribe<string>(onNext: s =>
                {
                    this.MainWindowColor = s;
                });
        }
    }
}
