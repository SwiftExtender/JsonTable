using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reactive;
using Newtonsoft.Json;
using AvaloniaEdit.Document;
using System.Linq;
using Newtonsoft.Json.Linq;
using DynamicData;
using System.Text.Json;
using System.Xml.Linq;
using static TableJson.ViewModels.MainWindowViewModel;
using Splat.ModeDetection;

namespace TableJson.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TreeNode> _treeNodes = new ObservableCollection<TreeNode>();
        public ObservableCollection<TreeNode> TreeNodes => _treeNodes;
        public class TreeNode
        {
            public string Name { get; set; }
            public TreeNode? Parent { get; set; }
            public ObservableCollection<TreeNode> Children { get; set; } = new ObservableCollection<TreeNode>();
            public string? Value { get; set; }
            public TreeNode(string name, TreeNode? parent)
            {
                Name = name;
                Parent = parent;
            }
        }
        public class TASK_O_ITEM
        {
            public string CODE { get; set; }
            public string NAME { get; set; }
        }
        public class TASK_O
        {
            public List<TASK_O_ITEM> TASK_O_ITEM { get; set; }
        }
        public class Outputdata
        {
            public TASK_O TASK_O { get; set; }
            public string ERROR_O { get; set; }
        }
        public class OutputParameters
        {
            public Outputdata Outputdata { get; set; }
        }
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
        private string _StatusText = "Status: No input was parsed";
        public string StatusText
        {
            get => _StatusText;
            set => this.RaiseAndSetIfChanged(ref _StatusText, value);
        }
        public ReactiveCommand<Unit, Unit> ParseCommand { get; }
        public Dictionary<string, List<string>> StatsDictionary { get; set; }
        public void JsonToTable()
        {
            try {
                var parsed_json = JsonConvert.DeserializeObject(RawText.Text);
                //var stats_json = JsonConvert.DeserializeObject<OutputParameters>(RawText.Text);
                //foreach (var item in stats_json.Outputdata.TASK_O.TASK_O_ITEM)
                //{
                //    if (StatsDictionary.ContainsKey(item.CODE)) {
                //        StatsDictionary[item.CODE].Add(item.NAME);
                //    }
                //    else
                //    {
                //        StatsDictionary.Add(item.CODE, new List<string> { item.NAME });
                //    }
                //}
                if (parsed_json is not null)
                {
                    RawText = new TextDocument(parsed_json.ToString());
                    //JsonTable = new JsonTreeViewViewModel(parsed_json.ToString()).TreeNodes;
                    StatusText = "Status: JSON parsed successfully";
                } else
                {
                    StatusText = "Status: Input string is empty";
                }
            } 
            catch (Exception e) 
            {
                StatusText = e.Message.ToString();
            }            
        }
        public MainWindowViewModel()
        {
            ParseCommand = ReactiveCommand.Create(JsonToTable);
            //JsonTable = new ObservableCollection<TreeNode>{};
            //JsonRows = new ObservableCollection<HttpServerRunner>();
            //TreeDataGridInit();
        }
    }
}
