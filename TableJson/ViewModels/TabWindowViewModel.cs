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
        public static DirectProperty<TabWindowViewModel, string> JSONPathQueryProperty = 
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(JSONPathQuery), o => o.JSONPathQuery, (o, v) => o.JSONPathQuery = v);
        public string JSONPathQuery
        {
            get => GetValue(JSONPathQueryProperty);
            set => SetValue(JSONPathQueryProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, string> JSONPathStatusProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(JSONPathStatus), o => o.JSONPathStatus, (o, v) => o.JSONPathStatus = v);
        public string JSONPathStatus
        {
            get => GetValue(JSONPathStatusProperty);
            set => SetValue(JSONPathStatusProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, string> ResultTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(ResultText), o => o.ResultText, (o, v) => o.ResultText = v);
        public string ResultText
        {
            get => GetValue(ResultTextProperty);
            set => SetValue(ResultTextProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, bool> ShowUniqueKeysProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, bool>(nameof(ShowUniqueKeys), o => o.ShowUniqueKeys, (o, v) => o.ShowUniqueKeys = v);
        public bool ShowUniqueKeys
        {
            get => GetValue(ShowUniqueKeysProperty);
            set => SetValue(ShowUniqueKeysProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, bool> ShowUniqueValuesProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, bool>(nameof(ShowUniqueValues), o => o.ShowUniqueValues, (o, v) => o.ShowUniqueValues = v);
        public bool ShowUniqueValues
        {
            get => GetValue(ShowUniqueValuesProperty);
            set => SetValue(ShowUniqueValuesProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, TextDocument> RawTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, TextDocument>(nameof(RawText), o => o.RawText, (o, v) => o.RawText = v);
        public TextDocument RawText
        {
            get => GetValue(RawTextProperty);
            set => SetValue(RawTextProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, JsonDocument> ParsedJsonProperty =
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
        public static DirectProperty<TabWindowViewModel, TextDocument> JSONPathResultProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, TextDocument>(nameof(JSONPathResult), o => o.JSONPathResult, (o, v) => o.JSONPathResult = v);
        public TextDocument JSONPathResult
        {
            get => GetValue(JSONPathResultProperty);
            set => SetValue(JSONPathResultProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, string> StatusTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(StatusText), o => o.StatusText, (o, v) => o.StatusText = v);
        public string StatusText
        {
            get => GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, string> ValuesNumberTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(ValuesNumberText), o => o.ValuesNumberText, (o, v) => o.ValuesNumberText = v);

        public string ValuesNumberText
        {
            get => GetValue(ValuesNumberTextProperty);
            set => SetValue(ValuesNumberTextProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, string> KeysNumberTextProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, string>(nameof(KeysNumberText), o => o.KeysNumberText, (o, v) => o.KeysNumberText = v);
        public string KeysNumberText
        {
            get => GetValue(KeysNumberTextProperty);
            set => SetValue(KeysNumberTextProperty, value);
        }
        private ObservableCollection<JsonQueryMenuItem>? _JSONQueryContextMenu = new ObservableCollection<JsonQueryMenuItem>();
        public static DirectProperty<TabWindowViewModel, ObservableCollection<JsonQueryMenuItem>> JSONQueryContextMenuProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<JsonQueryMenuItem>>(
                nameof(JSONQueryContextMenu), o => o._JSONQueryContextMenu, (o, v) => o._JSONQueryContextMenu = v);
        public ObservableCollection<JsonQueryMenuItem> JSONQueryContextMenu
        {
            get => GetValue(JSONQueryContextMenuProperty);
            set => SetValue(JSONQueryContextMenuProperty, value);
        }
        private ObservableCollection<MacrosMenuItem>? _MacrosContextMenu = new ObservableCollection<MacrosMenuItem>();
        public static DirectProperty<TabWindowViewModel, ObservableCollection<MacrosMenuItem>> MacrosContextMenuProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<MacrosMenuItem>>(nameof(MacrosContextMenu), o => o._MacrosContextMenu, (o, v) => o._MacrosContextMenu = v);
        public ObservableCollection<MacrosMenuItem> MacrosContextMenu
        {
            get => GetValue(MacrosContextMenuProperty);
            set => SetValue(MacrosContextMenuProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string> > JsonQuerysProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string> >(nameof(JsonQuerys), o => o.JsonQuerys, (o, v) => o.JsonQuerys = v);
        public ObservableCollection<string>? JsonQuerys
        {
            get => GetValue(JsonQuerysProperty);
            set => SetValue(JsonQuerysProperty, value);
        }
        private ObservableCollection<string> _TempJsonKeys;
        private ObservableCollection<string> _TempJsonValues;
        private ObservableCollection<string> _OneCycleJsonKeys;
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string>> OneCycleJsonKeysProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string>>(nameof(OneCycleJsonKeys), o => o.OneCycleJsonKeys, (o, v) => o.OneCycleJsonKeys = v);
        public ObservableCollection<string> OneCycleJsonKeys { 
            get => GetValue(OneCycleJsonKeysProperty); 
            set => SetValue(OneCycleJsonKeysProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string>> OneCycleJsonValuesProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string>>(nameof(OneCycleJsonValues), o => o.OneCycleJsonValues, (o, v) => o.OneCycleJsonValues = v);
        public ObservableCollection<string> OneCycleJsonValues { 
            get => GetValue(OneCycleJsonValuesProperty); 
            set => SetValue(OneCycleJsonValuesProperty, value); 
        }
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string>> JsonKeysProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string>>(nameof(JsonKeys), o => o.JsonKeys, (o, v) => o.JsonKeys = v);
        public ObservableCollection<string> JsonKeys { 
            get => GetValue(JsonKeysProperty); 
            set => SetValue(JsonKeysProperty, value); 
        }
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string>> UniqueJsonKeysProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string>>(nameof(UniqueJsonKeys), o => o.UniqueJsonKeys, (o, v) => o.UniqueJsonKeys = v);
        public ObservableCollection<string> UniqueJsonKeys { 
            get => GetValue(UniqueJsonKeysProperty); 
            set => SetValue(UniqueJsonKeysProperty, value); 
        }
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string>> JsonValuesProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string>>(nameof(JsonValues), o => o.JsonValues, (o, v) => o.JsonValues = v);
        public ObservableCollection<string> JsonValues { 
            get => GetValue(JsonValuesProperty);
            set => SetValue(JsonValuesProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string>> UniqueJsonValuesProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string>>(nameof(UniqueJsonValues), o => o.UniqueJsonValues, (o, v) => o.UniqueJsonValues = v);
        public ObservableCollection<string> UniqueJsonValues { 
            get => GetValue(UniqueJsonValuesProperty);
            set => SetValue(UniqueJsonValuesProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string>> KeyEntriesProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string>>(nameof(KeyEntries), o => o.KeyEntries, (o, v) => o.KeyEntries = v);
        public ObservableCollection<string> KeyEntries { 
            get => GetValue(KeyEntriesProperty);
            set => SetValue(KeyEntriesProperty, value);
        }
        public static DirectProperty<TabWindowViewModel, ObservableCollection<string>> ValueEntriesProperty =
            AvaloniaProperty.RegisterDirect<TabWindowViewModel, ObservableCollection<string>>(nameof(ValueEntries), o => o.ValueEntries, (o, v) => o.ValueEntries = v);
        public ObservableCollection<string> ValueEntries { 
            get => GetValue(ValueEntriesProperty);
            set => SetValue(ValueEntriesProperty, value);
        }
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
            //MacrosContextMenu = new ObservableCollection<MacrosMenuItem> { new MacrosMenuItem()
            //{ Header = "Copy", HeaderTextColor = Brushes.Green, BackgroundColor = Brushes.Honeydew } };
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
