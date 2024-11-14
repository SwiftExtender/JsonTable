using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Models.TreeDataGrid;
using Newtonsoft.Json;

namespace api_corelation.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _RawText = "";
        public string RawText
        {
            get => _RawText;
            set => this.RaiseAndSetIfChanged(ref _RawText, value);
        }
        private string _JsonText;
        public string JsonText
        {
            get => _JsonText;
            set => this.RaiseAndSetIfChanged(ref _JsonText, value);
        }private string _StatusText;
        public string StatusText
        {
            get => _JsonText;
            set => this.RaiseAndSetIfChanged(ref _JsonText, value);
        }
        public ReactiveCommand<Unit, Unit> ParseCommand { get; }

        public void JsonToTable()
        {
            try {
                var t = JsonConvert.DeserializeObject(RawText);
                JsonText = t.ToString();
                StatusText = "JSON parsed successfully";
            } 

            catch (Exception e) 
            {
                StatusText = e.ToString();
            }            
        }
        public MainWindowViewModel()
        {
            ParseCommand = ReactiveCommand.Create(JsonToTable);
            //JsonRows = new ObservableCollection<HttpServerRunner>();
            //TreeDataGridInit();
        }
    }
}
