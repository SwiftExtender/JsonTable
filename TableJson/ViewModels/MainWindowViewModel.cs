using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reactive;
using Newtonsoft.Json;
using AvaloniaEdit.Document;
using System.Text.Json;

namespace TableJson.ViewModels
{
    public class JsonTreeViewViewModel
    {
        private ObservableCollection<TreeNode> _treeNodes = new ObservableCollection<TreeNode>();
        public ObservableCollection<TreeNode> TreeNodes => _treeNodes;
        public JsonTreeViewViewModel(string jsonString)
        {
            var jsonDocument = JsonDocument.Parse(jsonString);
            BuildTree(jsonDocument.RootElement, null);
        }
        private void BuildTree(JsonElement element, TreeNode? parent)
        {
            TreeNode node = new TreeNode(element.ToString(), parent);

            if (element.ValueKind == JsonValueKind.Object || element.ValueKind == JsonValueKind.Array)
            {
                foreach (var property in element.EnumerateObject())
                {
                    BuildTree(property.Value, node);
                }
            }
            else if (element.ValueKind == JsonValueKind.Number || element.ValueKind == JsonValueKind.String ||
                     element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False)
            {
                // Handle primitive values (you might want to display these differently)
                node.Value = element.ToString();
            }

            _treeNodes.Add(node);
        }
    }
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
        }
        private string _StatusText = "Status: No input was parsed";
        public string StatusText
        {
            get => _StatusText;
            set => this.RaiseAndSetIfChanged(ref _StatusText, value);
        }
        public ReactiveCommand<Unit, Unit> ParseCommand { get; }
        private ObservableCollection<TreeNode> _JsonTable;
        public ObservableCollection<TreeNode> JsonTable {
            get => _JsonTable;
            set => this.RaiseAndSetIfChanged(ref _JsonTable, value);
        }
        public void JsonToTable()
        {
            try {
                var parsed_json = JsonConvert.DeserializeObject(RawText.Text);
                if (parsed_json is not null)
                {
                    JsonText = new TextDocument(parsed_json.ToString());
                    JsonTable = new JsonTreeViewViewModel(parsed_json.ToString()).TreeNodes;
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
            JsonTable = new ObservableCollection<TreeNode>{};
            //JsonRows = new ObservableCollection<HttpServerRunner>();
            //TreeDataGridInit();
        }
    }
}
