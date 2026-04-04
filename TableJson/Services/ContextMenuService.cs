using Avalonia;
using Avalonia.Input;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using TableJson.Models;

namespace TableJson.Services
{
    public class ScriptMenuItem()
    {
        public string Header { get; set; }
        public ICommand Command { get; set; }
        public KeyGesture HotKey { get; set; }
        public bool IsArgsRequired { get; set; }
        public string ItemColor { get; set; }
        public string TextColor { get; set; }
        public string HotkeyTextColor { get; set; }
    }
    public class ContextMenuService : ReactiveObject
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
        public string _ContextMenuItemColor = "";
        public string ContextMenuItemColor
        {
            get { return _ContextMenuItemColor; }
            set => this.RaiseAndSetIfChanged(ref _ContextMenuItemColor, value);
        }
        private string _ContextMenuTextColor = "";
        public string ContextMenuTextColor
        {
            get { return _ContextMenuTextColor; }
            set => this.RaiseAndSetIfChanged(ref _ContextMenuTextColor, value);
        }
        private string _ContextMenuHotkeyTextColor = "";
        public string ContextMenuHotkeyTextColor
        {
            get { return _ContextMenuHotkeyTextColor; }
            set => this.RaiseAndSetIfChanged(ref _ContextMenuHotkeyTextColor, value);
        }
        private ObservableCollection<ScriptMenuItem> _ScriptsContextMenu;
        public ObservableCollection<ScriptMenuItem> ScriptsContextMenu
        {
            get => _ScriptsContextMenu;
            set => this.RaiseAndSetIfChanged(ref _ScriptsContextMenu, value);
        }
        public KeyGesture GetValidatedHotkey(string rawHotKey)
        {
            if (rawHotKey != null)
            {
                return KeyGesture.Parse(rawHotKey);
            }
            else
            {
                return null;
            }
        }
        public ObservableCollection<ScriptMenuItem> PopulateScriptsMenu()
        {
            List<ScriptMenuItem> menuItems = new();
            string defaultMenuItemColor = ContextMenuItemColor;
            string defaultMenuTextColor = ContextMenuTextColor;
            string defaultMenuHotkeyTextColor = ContextMenuHotkeyTextColor;
            menuItems.Add(new ScriptMenuItem { Header = "Copy", Command = ReactiveCommand.Create<TextArea>(CopyMouseCommand), HotKey = new KeyGesture(Key.C, KeyModifiers.Control), ItemColor = defaultMenuItemColor, TextColor = defaultMenuTextColor });
            menuItems.Add(new ScriptMenuItem { Header = "Cut", Command = ReactiveCommand.Create<TextArea>(CutMouseCommand), HotKey = new KeyGesture(Key.X, KeyModifiers.Control), ItemColor = defaultMenuItemColor, TextColor = defaultMenuTextColor });
            menuItems.Add(new ScriptMenuItem { Header = "Paste", Command = ReactiveCommand.Create<TextArea>(PasteMouseCommand), HotKey = new KeyGesture(Key.V, KeyModifiers.Control), ItemColor = defaultMenuItemColor, TextColor = defaultMenuTextColor });
            menuItems.Add(new ScriptMenuItem { Header = "Select All", Command = ReactiveCommand.Create<TextArea>(SelectAllMouseCommand), HotKey = new KeyGesture(Key.A, KeyModifiers.Control), ItemColor = defaultMenuItemColor, TextColor = defaultMenuTextColor });

            using (var DataSource = new HelpContext())
            {
                List<Macros> selectedMacros = DataSource.MacrosTable.Where(i => i.IsActive == true).Where(i => i.BinaryExecutable != null).ToList();
                foreach (Macros macro in selectedMacros)
                {
                    Action<TextArea> customMethod = ExtractHandler(macro.BinaryExecutable);
                    if (customMethod != null)
                    {
                        ScriptMenuItem t = new ScriptMenuItem
                        {
                            Header = macro.Name,
                            Command = ReactiveCommand.Create<TextArea>(customMethod),
                            HotKey = GetValidatedHotkey(macro.HotKey),
                            ItemColor = macro.MenuItemColor,
                            TextColor = macro.MenuTextColor
                        };
                        menuItems.Add(item: t);
                    }
                }
            }
            return new ObservableCollection<ScriptMenuItem>(menuItems);
        }
        public Action<TextArea> ExtractHandler(byte[] dllArray)
        {
            try
            {
                Assembly asm = Assembly.Load(dllArray);
                Type type = asm.GetType("ContextItemPlugin.Plugin");
                MethodInfo entrypoint = type.GetMethod("Handler");
                if (entrypoint != null)
                {
                    return (Action<TextArea>)Delegate.CreateDelegate(typeof(Action<TextArea>), entrypoint);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public ContextMenuService()
        {
            ContextMenuItemColor = (Application.Current as App).Settings.ContextMenuItemColor;
            ContextMenuTextColor = (Application.Current as App).Settings.ContextMenuItemColor;
            ContextMenuHotkeyTextColor = (Application.Current as App).Settings.ContextMenuItemColor;
            ScriptsContextMenu = PopulateScriptsMenu();
        }
    }
}
