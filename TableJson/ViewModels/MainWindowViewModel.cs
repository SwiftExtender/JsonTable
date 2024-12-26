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
        public List<string> JsonKeys { get; }
        public List<string> JsonValues { get; }
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
        public MainWindowViewModel()
        {
            ParseCommand = ReactiveCommand.Create(JsonToTable);
        }
    }
}
