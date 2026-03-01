using AvaloniaEdit.Document;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using TableJson.Models;

namespace TableJson.ViewModels
{
    public class MacrosWindowViewModel : ViewModelBase
    {
        public void SaveCode()
        {
            SelectedRow.SourceCode = SourceCode.Text;
            SaveMacros(SelectedRow);
            if (SelectedRow.Name != "") {
                CompileStatusText = "Macros " + SelectedRow.Name + " saved";
            } else
            {
                CompileStatusText = "Macros saved";
            }
        }
        public void RemoveMacros(Macros remHint)
        {
            MacrosGridData.Remove(remHint);
            if (remHint.IsSaved)
            {
                using (var DataSource = new HelpContext())
                {
                    DataSource.MacrosTable.Attach(remHint);
                    DataSource.MacrosTable.Remove(remHint);
                    DataSource.SaveChanges();
                }
            }
        }
        public void SaveMacros(Macros updateHint)
        {
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
        private ObservableCollection<Macros>? _MacrosGridData;
        public ObservableCollection<Macros>? MacrosGridData
        {
            get => _MacrosGridData;
            set => this.RaiseAndSetIfChanged(ref _MacrosGridData, value);
        }
        public void AddMacros()
        {
            try
            {
                MacrosGridData.Add(new Macros(false));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static int RandomNumber()
        {
            Random random = new Random();
            return random.Next();
        }
        public static string GenerateChecksum(MemoryStream ms)
        {
            ms.Position = 0;
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] hash = sha256.ComputeHash(ms);
            return BitConverter.ToString(hash).Replace("-","");
        }
        public void CompileSourceCode()
        {
            CSharpCompilationOptions options = new CSharpCompilationOptions((OutputKind)LanguageVersion.Latest, deterministic: true, platform: Platform.AnyCpu, optimizationLevel: OptimizationLevel.Release);
            var syntaxTree = CSharpSyntaxTree.ParseText(SourceCode.Text);
            CSharpCompilation compilation = CSharpCompilation.Create(SelectedRow.Name+"__"+ RandomNumber().ToString())
                .AddSyntaxTrees(syntaxTree)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            SaveSourceCode(compilation);
        }
        public void SaveSourceCode(CSharpCompilation compilation)
        {
            //SelectedRow;
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (result.Success == true) CompileStatusText = "Compiled successfully";
                SelectedRow.Checksum = GenerateChecksum(ms);
                SelectedRow.BinaryExecutable = ms.ToArray();
                if (SelectedRow.IsSaved)
                {
                    using (var DataSource = new HelpContext())
                    {
                        DataSource.MacrosTable.Attach(SelectedRow);
                        DataSource.MacrosTable.Update(SelectedRow);
                        DataSource.SaveChanges();
                    }
                }
                else
                {
                    using (var DataSource = new HelpContext())
                    {
                        SelectedRow.IsSaved = true;
                        DataSource.MacrosTable.Attach(SelectedRow);
                        DataSource.MacrosTable.Add(SelectedRow);
                        DataSource.SaveChanges();
                    }
                }
            }
        }
        public ReactiveCommand<Unit, Unit> CompileSourceCodeCommand { get; }
        public ReactiveCommand<Macros, Unit> SaveMacrosCommand { get; }
        public ReactiveCommand<Macros, Unit> RemoveMacrosCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCodeCommand { get; }
        public ReactiveCommand<Unit, Unit> AddMacrosCommand { get; }
        private Macros _SelectedRow;
        public Macros SelectedRow
        {
            get => _SelectedRow;
            set
            {
                this.RaiseAndSetIfChanged(ref _SelectedRow, value);
                SourceCode.Text = _SelectedRow.SourceCode;
            }
        }
        public MacrosWindowViewModel()
        {
            CompileSourceCodeCommand = ReactiveCommand.Create(CompileSourceCode);
            RemoveMacrosCommand = ReactiveCommand.Create<Macros>(RemoveMacros);
            SaveMacrosCommand = ReactiveCommand.Create<Macros>(SaveMacros);
            SaveCodeCommand = ReactiveCommand.Create(SaveCode);
            AddMacrosCommand = ReactiveCommand.Create(AddMacros);
            using (var DataSource = new HelpContext())
            {
                List<Macros> selectedMacros = DataSource.MacrosTable.ToList();
                MacrosGridData = new ObservableCollection<Macros>(selectedMacros);
            }
        }
    }
}
