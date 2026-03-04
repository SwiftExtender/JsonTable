using ReactiveUI;

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
        public MainWindowViewModel()
        {

        }
    }
}
