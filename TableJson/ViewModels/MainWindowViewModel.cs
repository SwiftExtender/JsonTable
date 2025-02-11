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
using Microsoft.CodeAnalysis.CSharp;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using TableJson.Views;

namespace TableJson.ViewModels
{
    public class MacrosMenuItem()
    {
        public string Header { get; set; }
        //public ReactiveCommand<string, Unit> Macros { get; set; }
        //public object? MacrosParameter { get; set; }
        public IBrush? BackgroundColor { get; set; }
        public IBrush? HeaderTextColor { get; set; }
    }
    public class JSONQueryMenuItem()
    {
        public string Query { get; set; }
        public string Description { get; set; }
    }
    public class MainWindowViewModel : ViewModelBase
    {
        private string _MacrosNameText = "";
        public string MacrosNameText
        {
            get => _MacrosNameText;
            set => this.RaiseAndSetIfChanged(ref _MacrosNameText, value);
        }
        private string _SourceCodeRunOutputText = "";
        public string SourceCodeRunOutputText
        {
            get => _SourceCodeRunOutputText;
            set => this.RaiseAndSetIfChanged(ref _SourceCodeRunOutputText, value);
        }
        private string _CompileStatusText = "Status: No code was compiled";
        public string CompileStatusText
        {
            get => _CompileStatusText;
            set => this.RaiseAndSetIfChanged(ref _CompileStatusText, value);
        }
        public void CompileSourceCode()
        {
            var m = new Macros(false);
            var options = new CSharpCompilationOptions((OutputKind)LanguageVersion.Latest);
            var syntaxTree = CSharpSyntaxTree.ParseText(SourceCode.Text);
            var compilation = CSharpCompilation.Create("DynamicAssembly")
            .AddSyntaxTrees(syntaxTree)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (result.Success == true) CompileStatusText = "Built without errors";
                m.BinaryExecutable = ms.ToArray();
                if (m.IsSaved)
                {
                    using (var DataSource = new HelpContext())
                    {
                        DataSource.MacrosTable.Attach(m);
                        DataSource.MacrosTable.Update(m);
                        DataSource.SaveChanges();
                    }
                }
                else
                {
                    using (var DataSource = new HelpContext())
                    {
                        m.IsSaved = true;
                        DataSource.MacrosTable.Attach(m);
                        DataSource.MacrosTable.Add(m);
                        DataSource.SaveChanges();
                    }
                }
            }
        }
        public List<double> EditorFontSizes { get; set; } = Enumerable.Range(9, 66).Select(t => (double)t).ToList();
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
        private TextDocument _SourceCode = new TextDocument("");
        public TextDocument SourceCode
        {
            get => _SourceCode;
            set => this.RaiseAndSetIfChanged(ref _SourceCode, value);
        }
        private JsonDocument _ParsedJson;
        public JsonDocument ParsedJson
        {
            get => _ParsedJson;
            set { 
                if (_ParsedJson != null) {
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
        private ObservableCollection<JSONQueryMenuItem> _JSONQueryContextMenu = new ObservableCollection<JSONQueryMenuItem>();
        public ObservableCollection<JSONQueryMenuItem> JSONQueryContextMenu
        {
            get => _JSONQueryContextMenu;
            set => this.RaiseAndSetIfChanged(ref _JSONQueryContextMenu, value);
        }
        private ObservableCollection<MacrosMenuItem> _MacrosContextMenu = new ObservableCollection<MacrosMenuItem>();
        public ObservableCollection<MacrosMenuItem> MacrosContextMenu { 
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
            KeysNumberText = "Keys: "+KeyEntries.Count;
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
        public void JsonToTable()
        {
            try {
                if (RawText.Text.Trim() == "")
                {
                    StatusText = "Status: Input string is empty";
                } else {
                    ParsedJson = JsonDocument.Parse(RawText.Text);
                    RawText = new TextDocument(GetFormatText(ParsedJson));
                    //JsonTable = new JsonTreeViewViewModel(parsed_json.ToString()).TreeNodes;
                    StatusText = "Status: JSON parsed successfully";
                }
            } 
            catch (Exception e) 
            {
                StatusText = e.Message.ToString();
            }            
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
        public ReactiveCommand<Unit, Unit> AddMacrosCommand { get; }
        public ReactiveCommand<Unit, Unit> ParseCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleKeysShowModeCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleValuesShowModeCommand { get; }
        public ReactiveCommand<Unit, Unit> RunJsonPathQueryCommand { get; }
        public ReactiveCommand<Unit, Unit> CompileSourceCodeCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveMacrosCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveMacrosCommand { get; }
        public ReactiveCommand<Unit, Unit> OneCycleJsonTablifyCommand { get; }
        public ReactiveCommand<Unit, Unit> RecursiveJsonTablifyCommand { get; }
        public void AddMacros() { MacrosRows.Add(new Macros(false)); }
        public void RemoveMacros(object sender, RoutedEventArgs e)
        {
            Button removeButton = (Button)sender;
            Macros removedMacros = (Macros)removeButton.DataContext;
            MacrosRows.Remove(removedMacros);
            if (removedMacros.IsSaved)
            {
                using (var DataSource = new HelpContext())
                {
                    DataSource.MacrosTable.Attach(removedMacros);
                    DataSource.MacrosTable.Remove(removedMacros);
                    DataSource.SaveChanges();
                }
            }
        }
        public void SaveMacros(object sender, RoutedEventArgs e)
        {
            Button updateButton = (Button)sender;
            Macros updateHint = (Macros)updateButton.DataContext;
            if (updateHint.IsSaved)
            {
                using (var DataSource = new HelpContext())
                {
                    DataSource.MacrosTable.Attach(updateHint);
                    DataSource.MacrosTable.Update(updateHint);
                    DataSource.SaveChanges();
                }
            }
            else
            {
                using (var DataSource = new HelpContext())
                {
                    DataSource.MacrosTable.Attach(updateHint);
                    DataSource.MacrosTable.Add(updateHint);
                    DataSource.SaveChanges();
                }
                updateHint.IsSaved = true;
            }
        }
        public void CodeEditMacros(object sender, RoutedEventArgs e)
        {
            var w1 = new MacrosCodeWindow() { DataContext = new MainWindowViewModel() };
            w1.Show();
        }
        private Button UpdateButtonInit()
        {
            var b = new Button();
            b.Background = new SolidColorBrush() { Color = new Color(255, 34, 139, 34) };
            b.Content = "Add";
            b.Click += SaveMacros;
            return b;
        }
        private Button RemoveButtonInit()
        {
            var b = new Button();
            b.Background = new SolidColorBrush() { Color = new Color(255, 80, 00, 20) };
            b.Content = "Remove";
            b.Click += RemoveMacros;
            return b;
        }
        private Button CodeEditButtonInit()
        {
            var b = new Button();
            b.Background = new SolidColorBrush() { Color = new Color(255, 80, 00, 20) };
            b.Content = "Code";
            b.Click += CodeEditMacros;
            return b;
        }
        private DockPanel ButtonsPanelInit()
        {
            var panel = new DockPanel();
            panel.Children.Add(UpdateButtonInit());
            panel.Children.Add(RemoveButtonInit());
            panel.Children.Add(CodeEditButtonInit());
            panel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            return panel;
        }
        private ObservableCollection<Macros> _MacrosRows;
        public ObservableCollection<Macros> MacrosRows
        {
            get => _MacrosRows;
            set => this.RaiseAndSetIfChanged(ref _MacrosRows, value);
        }
        private FlatTreeDataGridSource<Macros>? _MacrosGridData;
        public FlatTreeDataGridSource<Macros>? MacrosGridData
        {
            get => _MacrosGridData;
            set => this.RaiseAndSetIfChanged(ref _MacrosGridData, value);
        }
        public void TreeDataGridInit()
        {
            var TextColumnLength = new GridLength(1, GridUnitType.Star);
            var TemplateColumnLength = new GridLength(125, GridUnitType.Pixel);

            var EditOptions = new TextColumnOptions<Macros>
                {
                    BeginEditGestures = BeginEditGestures.Tap,
                    MinWidth = new GridLength(80, GridUnitType.Pixel),
                    IsTextSearchEnabled = true,

                };
                TextColumn<Macros, string> MacrosNameColumn = new TextColumn<Macros, string>("Name", x => x.Name, options: EditOptions, width: TextColumnLength);
                MacrosGridData = new FlatTreeDataGridSource<Macros>(MacrosRows)
                {
                    Columns =
                    {
                        MacrosNameColumn,
                        new TemplateColumn<Macros>("Actions", new FuncDataTemplate<Macros>((a, e) => ButtonsPanelInit(), supportsRecycling: true), width: TemplateColumnLength),
                    },
                };

            MacrosGridData.Selection = new TreeDataGridCellSelectionModel<Macros>(MacrosGridData);
        }
        public MainWindowViewModel()
        {
            JSONQueryContextMenu = new ObservableCollection<JSONQueryMenuItem> { new JSONQueryMenuItem() { Description="lol",Query="kek"} };
            MacrosContextMenu = new ObservableCollection<MacrosMenuItem> { new MacrosMenuItem()
            { Header = "Copy", HeaderTextColor = Brushes.Green, BackgroundColor = Brushes.Honeydew } };
            //Macros = ReactiveCommand.Create<string>(CopyText)}
            AddMacrosCommand = ReactiveCommand.Create(AddMacros);
            ParseCommand = ReactiveCommand.Create(JsonToTable);
            ToggleKeysShowModeCommand = ReactiveCommand.Create(ToggleKeysShowMode);
            ToggleValuesShowModeCommand = ReactiveCommand.Create(ToggleValuesShowMode);
            RunJsonPathQueryCommand = ReactiveCommand.Create(RunJsonPathQuery);
            CompileSourceCodeCommand = ReactiveCommand.Create(CompileSourceCode);
            OneCycleJsonTablifyCommand = ReactiveCommand.Create(OneCycleJsonTablify);
            RecursiveJsonTablifyCommand = ReactiveCommand.Create(RecursiveJsonTablify);
            using (var DataSource = new HelpContext())
            {
                List<Macros> selectedMacros = DataSource.MacrosTable.ToList();
                MacrosRows = new ObservableCollection<Macros>(selectedMacros);
            }
            TreeDataGridInit();
        }
    }
}
