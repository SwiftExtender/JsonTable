using Avalonia;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using ReactiveUI;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using TableJson.Services;

namespace TableJson.ViewModels
{
    public class TabWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ScriptMenuItem> TabMenu
        {
            get => (Application.Current as App).ContextMenuService.ScriptsContextMenu;
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
        private TextDocument _RawText = new TextDocument();
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
        public TabWindowViewModel()
        {
            //MacrosContextMenu = PopulateMacroMenu();
        }
        public TabWindowViewModel(IStorageFile file)
        {
            LoadFileAsync(file.TryGetLocalPath());
            FileFullPath = file.TryGetLocalPath();
            //MacrosContextMenu = PopulateMacroMenu();
        }
    }
}
