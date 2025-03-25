using ReactiveUI;
using AvaloniaEdit.Document;
using System.IO;
using TableJson.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using System.Collections.ObjectModel;
using Avalonia.Media;
using System.Collections.Generic;
using System.Linq;
using TableJson.Views;

namespace TableJson.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TabWindow> _Tabs = new ObservableCollection<TabWindow>() { new TabWindow() };
        public ObservableCollection<TabWindow> Tabs {
            get => _Tabs;
            set => this.RaiseAndSetIfChanged(ref _Tabs, value);
        }
        public List<double> EditorFontSizes { get; set; } = Enumerable.Range(9, 66).Select(t => (double)t).ToList();
        private bool _IsPinnedWindow = false;
        public bool IsPinnedWindow
        {
            get => _IsPinnedWindow;
            set => this.RaiseAndSetIfChanged(ref _IsPinnedWindow, value);
        }
        private string _StatusText = "Status: No input was parsed";
        public string StatusText
        {
            get => _StatusText;
            set => this.RaiseAndSetIfChanged(ref _StatusText, value);
        }
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
        
        private TextDocument _SourceCode = new TextDocument("");
        public TextDocument SourceCode
        {
            get => _SourceCode;
            set => this.RaiseAndSetIfChanged(ref _SourceCode, value);
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
        public void AddTab() { }
        public void RemoveTab() { }
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
        private DockPanel ButtonsPanelInit()
        {
            var panel = new DockPanel();
            panel.Children.Add(UpdateButtonInit());
            panel.Children.Add(RemoveButtonInit());
            panel.Children.Add(CodeEditButtonInit());
            panel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            return panel;
        }
        private Button CodeEditButtonInit()
        {
            var b = new Button();
            b.Background = new SolidColorBrush() { Color = new Color(255, 80, 00, 20) };
            b.Content = "Code";
            b.Click += CodeEditMacros;
            return b;
        }
        public void CodeEditMacros(object sender, RoutedEventArgs e)
        {
            var w1 = new MacrosCodeWindow() { DataContext = this };
            w1.Show();
        }
        public ReactiveCommand<Unit, Unit> CompileSourceCodeCommand { get; }
        public ReactiveCommand<Unit, Unit> AddMacrosCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveMacrosCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveMacrosCommand { get; }
        public MainWindowViewModel()
        {
            CompileSourceCodeCommand = ReactiveCommand.Create(CompileSourceCode);
            AddMacrosCommand = ReactiveCommand.Create(AddMacros);
            using (var DataSource = new HelpContext())
            {
                List<Macros> selectedMacros = DataSource.MacrosTable.ToList();
                MacrosRows = new ObservableCollection<Macros>(selectedMacros);
            }
            TreeDataGridInit();
        }
    }
}
