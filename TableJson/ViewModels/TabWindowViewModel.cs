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
using System.ComponentModel;
using Avalonia;
using System.Collections;

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
    public class TabWindowViewModel : UserControl
    {
        public static readonly DirectProperty<TabWindowViewModel, string> JSONPathQueryProperty = 
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(JSONPathQuery), o => o.JSONPathQuery, (o, v) => o.JSONPathQuery = v);
        public string JSONPathQuery
        {
            get => GetValue(JSONPathQueryProperty);
            set => SetValue(JSONPathQueryProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, string> JSONPathStatusProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(JSONPathStatus), o => o.JSONPathStatus, (o, v) => o.JSONPathStatus = v);
        public string JSONPathStatus
        {
            get => GetValue(JSONPathStatusProperty);
            set => SetValue(JSONPathStatusProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, string> ResultTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(ResultText), o => o.ResultText, (o, v) => o.ResultText = v);
        public string ResultText
        {
            get => GetValue(ResultTextProperty);
            set => SetValue(ResultTextProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, bool> ShowUniqueKeysProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, bool>(nameof(ShowUniqueKeys), o => o.ShowUniqueKeys, (o, v) => o.ShowUniqueKeys = v);
        public bool ShowUniqueKeys
        {
            get => GetValue(ShowUniqueKeysProperty);
            set => SetValue(ShowUniqueKeysProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, bool> ShowUniqueValuesProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, bool>(nameof(ShowUniqueValues), o => o.ShowUniqueValues, (o, v) => o.ShowUniqueValues = v);
        public bool ShowUniqueValues
        {
            get => GetValue(ShowUniqueValuesProperty);
            set => SetValue(ShowUniqueValuesProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, TextDocument> RawTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, TextDocument>(nameof(RawText), o => o.RawText, (o, v) => o.RawText = v);
        public TextDocument RawText
        {
            get => GetValue(RawTextProperty);
            set => SetValue(RawTextProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, JsonDocument> ParsedJsonProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, JsonDocument>(nameof(ParsedJson), o => o.ParsedJson, (o, v) => o.ParsedJson = v);
        public JsonDocument ParsedJson
        {
            get => GetValue(ParsedJsonProperty);
            set
            {
                //if (_ParsedJson != null)
                //{
                //    _ParsedJson.Dispose();
                //}
                SetValue(RawTextProperty, value);
            }
        }
        public static readonly DirectProperty<TabWindowViewModel, TextDocument> JSONPathResultProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, TextDocument>(nameof(JSONPathResult), o => o.JSONPathResult, (o, v) => o.JSONPathResult = v);
        public TextDocument JSONPathResult
        {
            get => GetValue(JSONPathResultProperty);
            set => SetValue(JSONPathResultProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, string> StatusTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(StatusText), o => o.StatusText, (o, v) => o.StatusText = v);
        public string StatusText
        {
            get => GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, string> ValuesNumberTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(ValuesNumberText), o => o.ValuesNumberText, (o, v) => o.ValuesNumberText = v);

        public string ValuesNumberText
        {
            get => GetValue(ValuesNumberTextProperty);
            set => SetValue(ValuesNumberTextProperty, value);
        }
        public static readonly DirectProperty<TabWindowViewModel, string> KeysNumberTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(KeysNumberText), o => o.KeysNumberText, (o, v) => o.KeysNumberText = v);
        public string KeysNumberText
        {
            get => GetValue(KeysNumberTextProperty);
            set => SetValue(KeysNumberTextProperty, value);
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
        public void UpdateQueriesEvent(object? sender, EventArgs e)
        {
            UpdateQueries();
        }
        public TabWindowViewModel()
        {
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
