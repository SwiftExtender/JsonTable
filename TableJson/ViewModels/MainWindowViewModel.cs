using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reactive;
using AvaloniaEdit.Document;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.Text;
using TableJson.Models;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace TableJson.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //private ObservableCollection<TreeNode> _treeNodes = new ObservableCollection<TreeNode>();
        //public ObservableCollection<TreeNode> TreeNodes => _treeNodes;
        //public class TreeNode
        //{
        //    public string Name { get; set; }
        //    public TreeNode? Parent { get; set; }
        //    public ObservableCollection<TreeNode> Children { get; set; } = new ObservableCollection<TreeNode>();
        //    public string? Value { get; set; }
        //    public TreeNode(string name, TreeNode? parent)
        //    {
        //        Name = name;
        //        Parent = parent;
        //    }
        //}
        public List<double> EditorFontSizes {  get; set; } = Enumerable.Range(9, 66).Select(t => (double)t).ToList();
        private string _JSONPathQuery = "";
        public string JSONPathQuery
        {
            get => _JSONPathQuery;
            set => this.RaiseAndSetIfChanged(ref _JSONPathQuery, value);
        }
        private string _JSONPathStatus = "";
        public string JSONPathStatus
        {
            get => _JSONPathStatus;
            set => this.RaiseAndSetIfChanged(ref _JSONPathStatus, value);
        }
        private TextDocument _JSONPathResult = new TextDocument("");
        public TextDocument JSONPathResult
        {
            get => _JSONPathResult;
            set => this.RaiseAndSetIfChanged(ref _JSONPathResult, value);
        }
        private string _ResultText = "";
        public string ResultText
        {
            get => _ResultText;
            set => this.RaiseAndSetIfChanged(ref _ResultText, value);
        }
        private bool _ShowUniqueKeys = false;
        public bool ShowUniqueKeys
        {
            get => _ShowUniqueKeys;
            set => this.RaiseAndSetIfChanged(ref _ShowUniqueKeys, value);
        }
        private bool _ShowUniqueValues = false;
        public bool ShowUniqueValues
        {
            get => _ShowUniqueValues;
            set => this.RaiseAndSetIfChanged(ref _ShowUniqueValues, value);
        }
        private ObservableCollection<Macros> _MacrosGrid = new ObservableCollection<Macros> { };
        public ObservableCollection<Macros> MacrosGrid { get => _MacrosGrid; set => this.RaiseAndSetIfChanged(ref _MacrosGrid, value); }
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
        public ReactiveCommand<Unit, Unit> ToggleKeysShowModeCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleValuesShowModeCommand { get; }
        public ReactiveCommand<Unit, Unit> RunJsonPathQueryCommand { get; }
        public ReactiveCommand<Unit, Unit> AddMacrosCommand { get; }
        public List<string> JsonKeys { get; }
        public List<string> UniqueJsonKeys { get; }
        public List<string> JsonValues { get; }
        public List<string> UniqueJsonValues { get; }
        private ObservableCollection<string> _KeyEntries;
        public ObservableCollection<string> KeyEntries { get => _KeyEntries; set => this.RaiseAndSetIfChanged(ref _KeyEntries, value); }
        private ObservableCollection<string> _ValueEntries;
        public ObservableCollection<string> ValueEntries { get => _ValueEntries; set => this.RaiseAndSetIfChanged(ref _ValueEntries, value); }
        public ObservableCollection<string> GetAllValues(JsonDocument jsonDoc)
        {
            var keys = new ObservableCollection<string>();
            void Traverse(JsonElement token)
            {
                if (token.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement item in token.EnumerateArray())
                    {
                        Traverse(item);
                    }
                }
                else if (token.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty item in token.EnumerateObject())
                    {
                        Traverse(item.Value);
                    }
                } 
                else
                {
                    keys.Add(token.ToString());
                }   
            }
            Traverse(jsonDoc.RootElement);
            return keys;
        }
        public ObservableCollection<string> GetAllKeys(JsonDocument jsonDoc)
        {
            var keys = new ObservableCollection<string>();
            void Traverse(JsonElement token)
            {
                if (token.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement item in token.EnumerateArray())
                    {
                        Traverse(item);
                    }
                }
                else if (token.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty item in token.EnumerateObject())
                    {
                        keys.Add(item.Name);
                        Traverse(item.Value);
                    }
                }
            }
            Traverse(jsonDoc.RootElement);
            return keys;
        }
        public string GetFormatText(JsonDocument jdoc)
        {
            using (var stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                jdoc.WriteTo(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        public void AddMacros()
        {
            MacrosGrid.Add(new Macros(false));
        }
        public void ToggleKeysShowMode()
        {
            if (!ShowUniqueKeys)
            {
                ShowUniqueKeys = true;
            }
            else
            {
                ShowUniqueKeys = false;
            }
        }
        public void ToggleValuesShowMode()
        {
            if (!ShowUniqueValues)
            {
                ShowUniqueValues = true;
            } 
            else
            {
                ShowUniqueValues = false;
            }
        }
        public void JsonToTable()
        {
            try {
                if (RawText.Text.Trim() == "")
                {
                    StatusText = "Status: Input string is empty";
                } else {
                    using (JsonDocument parsed_json = JsonDocument.Parse(RawText.Text))
                    {
                        ValueEntries = GetAllValues(parsed_json);
                        KeyEntries = GetAllKeys(parsed_json);
                        RawText = new TextDocument(GetFormatText(parsed_json));
                        //JsonTable = new JsonTreeViewViewModel(parsed_json.ToString()).TreeNodes;
                        StatusText = "Status: JSON parsed successfully";
                    }
                }
            } 
            catch (Exception e) 
            {
                StatusText = e.Message.ToString();
            }            
        }     
        public void RunJsonPathQuery()
        {
            try
            {
                if (RawText.Text.Trim() == "")
                {
                    JSONPathStatus = "Status: JSON is empty";
                }
                if (JSONPathQuery == "")
                {
                    JSONPathStatus = "Status: JSONPath query is empty";
                }
                else
                {
                    JObject JsonObject = JObject.Parse(RawText.Text);
                    var t = JsonObject.SelectToken(JSONPathQuery).ToString(); ;
                    JSONPathResult =  new TextDocument(t);
                    JSONPathStatus = "Status: JSONPath query completed successfully";
                }
            }
            catch (Exception e)
            {
                JSONPathStatus = e.Message.ToString();
            }
        }
        public MainWindowViewModel()
        {
            ParseCommand = ReactiveCommand.Create(JsonToTable);
            ToggleKeysShowModeCommand = ReactiveCommand.Create(ToggleKeysShowMode);
            ToggleValuesShowModeCommand = ReactiveCommand.Create(ToggleValuesShowMode);
            RunJsonPathQueryCommand = ReactiveCommand.Create(RunJsonPathQuery);
            AddMacrosCommand = ReactiveCommand.Create(AddMacros);
            MacrosGrid = new ObservableCollection<Macros> { };
        }
    }
}
