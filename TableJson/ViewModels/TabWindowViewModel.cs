using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TableJson.Models;

namespace TableJson.ViewModels
{
    public class MacrosMenuItem()
    {
        public string Header { get; set; }
        public ReactiveCommand<string, Unit> Macros { get; set; }
        //public object? MacrosParameter { get; set; }
        //public IBrush? BackgroundColor { get; set; }
        //public IBrush? HeaderTextColor { get; set; }
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
        public void CopyMouseCommand(TextArea textArea)
        {
            ApplicationCommands.Copy.Execute(null, textArea);
        }
        public void CutMouseCommand(TextArea textArea)
        {
            ApplicationCommands.Cut.Execute(null, textArea);
        }
        public void PasteMouseCommand(TextArea textArea)
        {
            ApplicationCommands.Paste.Execute(null, textArea);
        }
        public void SelectAllMouseCommand(TextArea textArea)
        {
            ApplicationCommands.SelectAll.Execute(null, textArea);
        }
        public static void EmptyCommand()
        {

        }
        private string _FileFullPath = "";
        public string FileFullPath
        {
            get => _FileFullPath;
            set => this.RaiseAndSetIfChanged(ref _FileFullPath, value);
        }

        private string _ResultText = "";
        public string ResultText
        {
            get => _ResultText;
            set => this.RaiseAndSetIfChanged(ref _ResultText, value);
        }

        private TextDocument _RawText = new TextDocument("");
        public TextDocument RawText
        {
            get => _RawText;
            set => this.RaiseAndSetIfChanged(ref _RawText, value);
        }

        private string _StatusText = "No macros launched";
        public string StatusText
        {
            get => _StatusText;
            set => this.RaiseAndSetIfChanged(ref _StatusText, value);
        }
        private ObservableCollection<MenuItem> _MacrosContextMenu;
        public ObservableCollection<MenuItem> MacrosContextMenu
        {
            get => _MacrosContextMenu;
            set => this.RaiseAndSetIfChanged(ref _MacrosContextMenu, value);
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
            using var fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);

            using var reader = new StreamReader(fileStream);
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
                    RawText = new TextDocument(sb.ToString());
                }
                catch (Exception e)
                {
                    RawText = new TextDocument(e.ToString());
                }
            }, DispatcherPriority.Background);
        }

        public ObservableCollection<MenuItem> PopulateMacroMenu()
        {

            List<MenuItem> m = new();
            m.Add(new MenuItem { Header = "Copy", Command = ApplicationCommands.Copy });
            m.Add(new MenuItem { Header = "Cut", Command = ApplicationCommands.Cut });
            m.Add(new MenuItem { Header = "Paste", Command = ApplicationCommands.Paste });
            //m.Add(new MenuItem { Header = "-" });
            m.Add(new MenuItem { Header = "Select All", Command = ApplicationCommands.SelectAll });
            using (var DataSource = new HelpContext())
            {
                List<Macros> selectedMacros = DataSource.MacrosTable.Where(i => i.IsActive == true).ToList();
                foreach (Macros macro in selectedMacros)
                {
                    //m.Add(item: new MenuItem { Header = macro.Name, Command = new RoutedCommandBinding(ApplicationCommands.Find, ExecuteFind) });
                    //m.Add(item: new MenuItem { Header = macro.Name, Command = c1, CommandParameter= "test" });
                }
            }
            return new ObservableCollection<MenuItem>(m);
        }
        public TabWindowViewModel()
        {
            //ToggleKeysShowModeCommand = ReactiveCommand.Create(ToggleKeysShowMode);
            //ToggleValuesShowModeCommand = ReactiveCommand.Create(ToggleValuesShowMode);
            //RunJsonPathQueryCommand = ReactiveCommand.Create(RunJsonPathQuery);

            //OneCycleJsonTablifyCommand = ReactiveCommand.Create(OneCycleJsonTablify);
            //RecursiveJsonTablifyCommand = ReactiveCommand.Create(RecursiveJsonTablify);
            //SaveQueryFastWindowCommand = ReactiveCommand.Create(SaveQueryFastWindow);
            MacrosContextMenu = PopulateMacroMenu();
        }
        public TabWindowViewModel(IStorageFile file)
        {
            LoadFileAsync(file.TryGetLocalPath());
            FileFullPath = file.TryGetLocalPath();

            //ToggleKeysShowModeCommand = ReactiveCommand.Create(ToggleKeysShowMode);
            //ToggleValuesShowModeCommand = ReactiveCommand.Create(ToggleValuesShowMode);
            //RunJsonPathQueryCommand = ReactiveCommand.Create(RunJsonPathQuery);

            //OneCycleJsonTablifyCommand = ReactiveCommand.Create(OneCycleJsonTablify);
            //RecursiveJsonTablifyCommand = ReactiveCommand.Create(RecursiveJsonTablify);
            //SaveQueryFastWindowCommand = ReactiveCommand.Create(SaveQueryFastWindow);
            MacrosContextMenu = PopulateMacroMenu();
            //UpdateQueries();
        }
    }
}
