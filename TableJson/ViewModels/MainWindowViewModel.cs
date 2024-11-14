using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using AvaloniaEdit;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Models.TreeDataGrid;
using Newtonsoft.Json;
using AvaloniaEdit.Document;
using System.Reflection.Metadata;

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
        private TextDocument _RawText = new TextDocument("");
        public TextDocument RawText
        {
            get => _RawText;
            set => this.RaiseAndSetIfChanged(ref _RawText, value);
        }
        private TextDocument _JsonText = new TextDocument("");
        public TextDocument JsonText
        {
            get => _JsonText;
            set => this.RaiseAndSetIfChanged(ref _JsonText, value);
        }private string _StatusText = "Status: No input was parsed";
        public string StatusText
        {
            get => _StatusText;
            set => this.RaiseAndSetIfChanged(ref _StatusText, value);
        }
        public ReactiveCommand<Unit, Unit> ParseCommand { get; }

        public void JsonToTable()
        {
            try {
                var t = JsonConvert.DeserializeObject(RawText.Text);
                if (t is not null)
                {
                    JsonText = new TextDocument(t.ToString());
                    StatusText = "Status: JSON parsed successfully";
                } else
                {
                    StatusText = "Status: Input string is empty";
                }
                
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
