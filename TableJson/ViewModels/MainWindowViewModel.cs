using ReactiveUI;

namespace TableJson.ViewModels
{
    //public class TabItemViewModel
    //{
    //    public string Header { get; set; }
    //    public string Content { get; set; }
    //}
    //public class CustomTabItem : TabItem
    //{
    //    public CustomTabItem() {
    //        Header = "1";
    //        Item
    //    }
    //}
    public class MainWindowViewModel : ViewModelBase
    {
        //private ObservableCollection<TabWindow> _Tabs = new ObservableCollection<TabWindow>();// { new TabWindow() {DataContext=new TabWindowViewModel() }, new TabWindow() { DataContext = new TabWindowViewModel() }, new TabWindow() { DataContext = new TabWindowViewModel() } };
        //public ObservableCollection<TabWindow> Tabs {
        //    get => _Tabs;
        //    set => this.RaiseAndSetIfChanged(ref _Tabs, value);
        //}
        //private ObservableCollection<TabWindowViewModel> _Tabs = new ObservableCollection<TabWindowViewModel>() { new TabWindowViewModel() {} };
        //public ObservableCollection<TabWindowViewModel> Tabs {
        //    get => _Tabs;
        //    set => this.RaiseAndSetIfChanged(ref _Tabs, value);
        //}

        private bool _IsPinnedWindow = false;
        public bool IsPinnedWindow
        {
            get => _IsPinnedWindow;
            set => this.RaiseAndSetIfChanged(ref _IsPinnedWindow, value);
        }
        private string _StatusText = "Status: No input was parsed";
        public string StatusText
        {
            get => _StatusText;
            set => this.RaiseAndSetIfChanged(ref _StatusText, value);
        }
        public MainWindowViewModel()
        {

        }
    }
}
