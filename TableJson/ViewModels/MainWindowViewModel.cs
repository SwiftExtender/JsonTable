using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reactive;
//using Newtonsoft.Json;
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
        public Dictionary<string, List<string>> StatsDictionary { get; set; }
        //public List<string> GetAllKeys(JToken token)
        //{
        //    var keys = new List<string>();
        //    foreach (var item in token.)
        //    //void Traverse(JToken jtoken, int depth = 0)
        //    //{
        //    //    switch (jtoken.Type)
        //    //    {
        //    //        case JTokenType.Object:
        //    //            foreach (var child in jtoken.Children())
        //    //            {
        //    //                if (child is JObject obj)
        //    //                {
        //    //                    keys.AddRange(GetAllKeys(obj));
        //    //                }
        //    //                else if (child is JProperty prop)
        //    //                {
        //    //                    keys.Add(prop.Name);
        //    //                    keys.AddRange(GetAllKeys(prop));
        //    //                }
        //    //            }
        //    //            break;
        //    //        case JTokenType.Array:
        //    //            for (int i = 0; i < jtoken.Count(); i++)
        //    //            {
        //    //                Traverse(jtoken[i], depth + 1);
        //    //            }
        //    //            break;
        //    //        case JTokenType.Property:
        //    //            jtoken.
        //    //        default:
        //    //            //keys.Add(jtoken.ToString());
        //    //            break;
        //    //    }
        //    //}
        //    //Traverse(token);
        //    return keys;
        //}
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
                JsonDocument parsed_json = JsonDocument.Parse(RawText.Text);
                var keys = new List<string>();
                foreach (JsonProperty property in parsed_json.RootElement.EnumerateObject())
                {
                    keys.Add(property.Name);
                    keys.Add(property.Value.ToString());
                }
                if (parsed_json is not null)
                {
                    RawText = new TextDocument(GetFormatText(parsed_json));
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
