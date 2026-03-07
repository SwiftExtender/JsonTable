using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TableJson.Models;

namespace TableJson.ViewModels
{
    //public class ContextCommand<T> : ICommand
    //{
    //    private readonly Action<T> _execute;
    //    private readonly Predicate<T> _canExecute;
    //    private event EventHandler? _canExecuteChanged;

    //    public ContextCommand(Action<T> execute, Predicate<T> canExecute)
    //    {
    //        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    //        _canExecute = canExecute;
    //    }
    //    public event EventHandler? CanExecuteChanged
    //    {
    //        add => _canExecuteChanged += value;
    //        remove => _canExecuteChanged -= value;
    //    }
    //    public bool CanExecute(object? parameter)
    //    {
    //        if (parameter == null)
    //        {
    //            return true;
    //        }
    //        return _canExecute((T)parameter);
    //    }
    //    public void Execute(object? parameter)
    //    {
    //        _execute((T)parameter);
    //    }
    //}
    public class MacrosMenuItem()
    {
        public string Header { get; set; }
        public ICommand Command { get; set; }
        public KeyGesture HotKey { get; set; }
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
        public void OpenFolderPathCommand(TextArea textArea)
        {
            string path = textArea.Selection.GetText();
            if (Directory.Exists(path))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start("explorer", path);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", path);
                }
                else
                {
                    Process.Start("xdg-open", path);
                }
            }
            else
            {
                StatusText = "Invalid Folder";
            }

        }
        public void OpenUrlCommand(TextArea textArea)
        {
            string url = textArea.Selection.GetText();
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            } else
            {
                StatusText = "Invalid URL";
            }
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
        private ObservableCollection<MacrosMenuItem> _MacrosContextMenu;
        public ObservableCollection<MacrosMenuItem> MacrosContextMenu
        {
            get => _MacrosContextMenu;
            set => this.RaiseAndSetIfChanged(ref _MacrosContextMenu, value);
        }
        private List<ICommand> _CommandsList;
        public List<ICommand> CommandsList { get; set; }
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

        public ObservableCollection<MacrosMenuItem> PopulateMacroMenu()
        {
            List<MacrosMenuItem> menuItems = new();
            menuItems.Add(new MacrosMenuItem { Header = "Copy", Command = ReactiveCommand.Create<TextArea>(CopyMouseCommand), HotKey=new KeyGesture(Key.C, KeyModifiers.Control) });
            menuItems.Add(new MacrosMenuItem { Header = "Cut", Command = ReactiveCommand.Create<TextArea>(CutMouseCommand), HotKey = new KeyGesture(Key.X, KeyModifiers.Control) });
            menuItems.Add(new MacrosMenuItem { Header = "Paste", Command = ReactiveCommand.Create<TextArea>(PasteMouseCommand), HotKey = new KeyGesture(Key.P, KeyModifiers.Control) });
            menuItems.Add(new MacrosMenuItem { Header = "Select All", Command = ReactiveCommand.Create<TextArea>(SelectAllMouseCommand), HotKey = new KeyGesture(Key.A, KeyModifiers.Control) });
            menuItems.Add(new MacrosMenuItem { Header = "Open as Folder", Command = ReactiveCommand.Create<TextArea>(OpenFolderPathCommand) });
            menuItems.Add(new MacrosMenuItem { Header = "Open as URL", Command = ReactiveCommand.Create<TextArea>(OpenUrlCommand) });

            using (var DataSource = new HelpContext())
            {
                List<Macros> selectedMacros = DataSource.MacrosTable.Where(i => i.IsActive == true).ToList();
                foreach (Macros macro in selectedMacros)
                {
                    //m.Add(item: new MenuItem { Header = macro.Name, Command = new RoutedCommandBinding(ApplicationCommands.Find, ExecuteFind) });

                    //menuItems.Add(item: new MacrosMenuItem { Header = macro.Name, Command = ReactiveCommand.Create<TextArea>() });
                }
            }
            return new ObservableCollection<MacrosMenuItem>(menuItems);
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
