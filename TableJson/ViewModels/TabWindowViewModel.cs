using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TableJson.Models;
using TableJson.Views;
using Tmds.DBus.Protocol;
using static System.Net.Mime.MediaTypeNames;

namespace TableJson.ViewModels
{
    public class MacrosMenuItem()
    {
        public string Header { get; set; }
        public ReactiveCommand<string, Unit> Macros { get; set; }
        //public object? MacrosParameter { get; set; }
        public IBrush? BackgroundColor { get; set; }
        public IBrush? HeaderTextColor { get; set; }
    }
    public class JsonQueryMenuItem
    {
        public string? Query { get; set; }
        public string? Description { get; set; }
        public JsonQueryMenuItem(string query, string desc)
        {
            Query = query;
            Description = desc;
        }
    }
    public class TabWindowViewModel : ViewModelBase
    {

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
        private TextDocument _RawText = new TextDocument("");
        public TextDocument RawText
        {
            get => _RawText;
            set => this.RaiseAndSetIfChanged(ref _RawText, value);
        }
        private JsonDocument _ParsedJson;
        public JsonDocument ParsedJson
        {
            get => _ParsedJson;
            set
            {
                if (_ParsedJson != null)
                {
                    _ParsedJson.Dispose();
                }
                this.RaiseAndSetIfChanged(ref _ParsedJson, value);
            }
        }
        private TextDocument _JSONPathResult = new TextDocument("");
        public TextDocument JSONPathResult
        {
            get => _JSONPathResult;
            set => this.RaiseAndSetIfChanged(ref _JSONPathResult, value);
        }
        private string _StatusText = "Status: No input was parsed";
        public string StatusText
        {
            get => _StatusText;
            set => this.RaiseAndSetIfChanged(ref _StatusText, value);
        }
        private string _ValuesNumberText = "";
        public string ValuesNumberText
        {
            get => _ValuesNumberText;
            set => this.RaiseAndSetIfChanged(ref _ValuesNumberText, value);
        }
        private string _KeysNumberText = "";
        public string KeysNumberText
        {
            get => _KeysNumberText;
            set => this.RaiseAndSetIfChanged(ref _KeysNumberText, value);
        }
        private ObservableCollection<JsonQueryMenuItem>? _JSONQueryContextMenu = new ObservableCollection<JsonQueryMenuItem>();
        public ObservableCollection<JsonQueryMenuItem> JSONQueryContextMenu
        {
            get => _JSONQueryContextMenu;
            set => this.RaiseAndSetIfChanged(ref _JSONQueryContextMenu, value);
        }
        private ObservableCollection<MacrosMenuItem> _MacrosContextMenu = new ObservableCollection<MacrosMenuItem>();
        public ObservableCollection<MacrosMenuItem> MacrosContextMenu
        {
            get => _MacrosContextMenu;
            set => this.RaiseAndSetIfChanged(ref _MacrosContextMenu, value);
        }
        private ObservableCollection<string>? _JsonQuerys;
        public ObservableCollection<string>? JsonQuerys
        {
            get => _JsonQuerys;
            set => this.RaiseAndSetIfChanged(ref _JsonQuerys, value);
        }
        private ObservableCollection<string> _TempJsonKeys;
        private ObservableCollection<string> _TempJsonValues;
        private ObservableCollection<string> _OneCycleJsonKeys;
        public ObservableCollection<string> OneCycleJsonKeys { get => _OneCycleJsonKeys; set => this.RaiseAndSetIfChanged(ref _OneCycleJsonKeys, value); }
        private ObservableCollection<string> _OneCycleJsonValues;
        public ObservableCollection<string> OneCycleJsonValues { get => _OneCycleJsonValues; set => this.RaiseAndSetIfChanged(ref _OneCycleJsonValues, value); }
        private ObservableCollection<string> _JsonKeys;
        public ObservableCollection<string> JsonKeys { get => _JsonKeys; set => this.RaiseAndSetIfChanged(ref _JsonKeys, value); }
        private ObservableCollection<string> _UniqueJsonKeys;
        public ObservableCollection<string> UniqueJsonKeys { get => _UniqueJsonKeys; set => this.RaiseAndSetIfChanged(ref _UniqueJsonKeys, value); }
        private ObservableCollection<string> _JsonValues;
        public ObservableCollection<string> JsonValues { get => _JsonValues; set => this.RaiseAndSetIfChanged(ref _JsonValues, value); }
        private ObservableCollection<string> _UniqueJsonValues;
        public ObservableCollection<string> UniqueJsonValues { get => _UniqueJsonValues; set => this.RaiseAndSetIfChanged(ref _UniqueJsonValues, value); }
        private ObservableCollection<string> _KeyEntries;
        public ObservableCollection<string> KeyEntries { get => _KeyEntries; set => this.RaiseAndSetIfChanged(ref _KeyEntries, value); }
        private ObservableCollection<string> _ValueEntries;
        public ObservableCollection<string> ValueEntries { get => _ValueEntries; set => this.RaiseAndSetIfChanged(ref _ValueEntries, value); }
        public ObservableCollection<string> GetAllValuesRecursively(JsonDocument jsonDoc)
        {
            var values = new ObservableCollection<string>();
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
                    values.Add(token.ToString());
                }
            }
            Traverse(jsonDoc.RootElement);
            return values;
        }
        public ObservableCollection<string> GetAllKeysRecursively(JsonDocument jsonDoc)
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
        public ObservableCollection<string> GetAllKeysOneCycle(JsonDocument jsonDoc)
        {
            var keys = new ObservableCollection<string>();
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty item in jsonDoc.RootElement.EnumerateObject())
                {
                    keys.Add(item.Name);
                }
            }
            return keys;
        }
        public ObservableCollection<string> GetAllValuesOneCycle(JsonDocument jsonDoc)
        {
            var values = new ObservableCollection<string>();
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in jsonDoc.RootElement.EnumerateArray())
                {
                    values.Add(item.ToString());
                }
            }
            else if (jsonDoc.RootElement.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty item in jsonDoc.RootElement.EnumerateObject())
                {
                    values.Add(item.Value.ToString());
                }
            }
            return values;
        }
        public void ToggleKeysShowMode()
        {
            if (!ShowUniqueKeys)
            {
                _TempJsonKeys = KeyEntries;
                KeyEntries = new ObservableCollection<string>(KeyEntries.Distinct());
                ShowUniqueKeys = true;
            }
            else
            {
                KeyEntries = new ObservableCollection<string>(_TempJsonKeys);
                ShowUniqueKeys = false;
            }
            KeysNumberText = "Keys: " + KeyEntries.Count;
        }
        public void ToggleValuesShowMode()
        {
            if (!ShowUniqueValues)
            {
                _TempJsonValues = ValueEntries;
                ValueEntries = new ObservableCollection<string>(ValueEntries.Distinct());
                ShowUniqueValues = true;
            }
            else
            {
                ValueEntries = new ObservableCollection<string>(_TempJsonValues);
                ShowUniqueValues = false;
            }
            ValuesNumberText = "Values: " + ValueEntries.Count;
        }

        public void OneCycleJsonTablify()
        {
            KeyEntries = GetAllKeysOneCycle(ParsedJson);
            ValueEntries = GetAllValuesOneCycle(ParsedJson);
            KeysNumberText = "Keys: " + KeyEntries.Count;
            ValuesNumberText = "Values: " + ValueEntries.Count;
        }
        public void RecursiveJsonTablify()
        {
            KeyEntries = GetAllKeysRecursively(ParsedJson);
            ValueEntries = GetAllValuesRecursively(ParsedJson);
            KeysNumberText = "Keys: " + KeyEntries.Count;
            ValuesNumberText = "Values: " + ValueEntries.Count;
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
                    JSONPathResult = new TextDocument(t);
                    JSONPathStatus = "Status: JSONPath query completed successfully";
                }
            }
            catch (Exception e)
            {
                JSONPathStatus = e.Message.ToString();
            }
        }
        public ReactiveCommand<Unit, Unit> ToggleKeysShowModeCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleValuesShowModeCommand { get; }
        public ReactiveCommand<Unit, Unit> RunJsonPathQueryCommand { get; }
        public ReactiveCommand<Unit, Unit> OneCycleJsonTablifyCommand { get; }
        public ReactiveCommand<Unit, Unit> RecursiveJsonTablifyCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveQueryFastWindowCommand { get; }
        public void SaveQueryFastWindow()
        {
            var w1 = new SaveWindow() { DataContext = new SaveWindowViewModel(JSONPathQuery) };
            w1.Closed += UpdateQueriesEvent;
            w1.Show();
        }
        private void UpdateQueries()
        {
            using (var DataSource = new HelpContext())
            {
                JSONQueryContextMenu = new ObservableCollection<JsonQueryMenuItem>();
                List<JsonQuery> querylist = DataSource.JsonQueryTable.ToList();
                foreach (JsonQuery item in querylist)
                {
                    JSONQueryContextMenu.Add(new JsonQueryMenuItem(item.Query, item.Description));
                }
            }
        }

        //public async Task LoadFileAs(IStorageFile file) {
        //    await using var stream = await file.OpenReadAsync();
        //    using var streamReader = new StreamReader(stream);
        //    var fileContent = await streamReader.ReadToEndAsync();
        //    await Dispatcher.UIThread.InvokeAsync(() =>
        //    {
        //        RawText = new TextDocument(fileContent);
        //    }, DispatcherPriority.Background);
        //}
        //public static async Task<string> ReadFileToStringMappedAsync(
        //    string filePath,
        //    Encoding encoding = null,
        //    int bufferSize = 4096)
        //{
        //    if (!File.Exists(filePath))
        //        throw new FileNotFoundException($"File not found: {filePath}");

        //    encoding ??= Encoding.UTF8;

        //    var fileInfo = new FileInfo(filePath);
        //    long fileLength = fileInfo.Length;

        //    using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
        //    {
        //        using (var stream = mmf.CreateViewStream(0, fileLength, MemoryMappedFileAccess.Read))
        //        {
        //            using (var reader = new StreamReader(stream, encoding, false, bufferSize))
        //            {
        //                // Convert all text content to string
        //                return await reader.ReadToEndAsync();
        //            }
        //        }
        //    }
        //}

        public async Task LoadFileAsync(string filePath)
        {
            using var reader = new StreamReader(filePath);
            StringBuilder sb = new StringBuilder();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                   try
                   {
                        RawText = new TextDocument(sb.ToString()) ;
                   } catch (Exception e) {
                        RawText = new TextDocument(e.ToString());
                   }        
            }, DispatcherPriority.Background);
            GC.Collect(2);
        }
        public void UpdateQueriesEvent(object? sender, EventArgs e)
        {
            UpdateQueries();
        }
        public TabWindowViewModel(IStorageFile file)
        {
            LoadFileAsync(file.TryGetLocalPath());
            //ProcessLargeFile(file.TryGetLocalPath());
            //LoadFileAsync(file.TryGetLocalPath());

            MacrosContextMenu = new ObservableCollection<MacrosMenuItem> { new MacrosMenuItem()
            { Header = "Copy", HeaderTextColor = Brushes.Green, BackgroundColor = Brushes.Honeydew } };
            //Macros = ReactiveCommand.Create<string>(CopyText)}


            ToggleKeysShowModeCommand = ReactiveCommand.Create(ToggleKeysShowMode);
            ToggleValuesShowModeCommand = ReactiveCommand.Create(ToggleValuesShowMode);
            RunJsonPathQueryCommand = ReactiveCommand.Create(RunJsonPathQuery);

            OneCycleJsonTablifyCommand = ReactiveCommand.Create(OneCycleJsonTablify);
            RecursiveJsonTablifyCommand = ReactiveCommand.Create(RecursiveJsonTablify);
            SaveQueryFastWindowCommand = ReactiveCommand.Create(SaveQueryFastWindow);

            UpdateQueries();

        }
        public TabWindowViewModel()
        {
            //MainWindowViewModel.DataRequested += OnDataRequested;

            MacrosContextMenu = new ObservableCollection<MacrosMenuItem> { new MacrosMenuItem()
            { Header = "Copy", HeaderTextColor = Brushes.Green, BackgroundColor = Brushes.Honeydew } };
            //Macros = ReactiveCommand.Create<string>(CopyText)}


            ToggleKeysShowModeCommand = ReactiveCommand.Create(ToggleKeysShowMode);
            ToggleValuesShowModeCommand = ReactiveCommand.Create(ToggleValuesShowMode);
            RunJsonPathQueryCommand = ReactiveCommand.Create(RunJsonPathQuery);

            OneCycleJsonTablifyCommand = ReactiveCommand.Create(OneCycleJsonTablify);
            RecursiveJsonTablifyCommand = ReactiveCommand.Create(RecursiveJsonTablify);
            SaveQueryFastWindowCommand = ReactiveCommand.Create(SaveQueryFastWindow);

            UpdateQueries();

        }
    }
}
