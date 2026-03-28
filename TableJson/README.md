Snippets for context menu

1) Copy SHA256 hash of highlighted string in clipboard

```
using Avalonia.Controls;
using Avalonia.VisualTree;
using AvaloniaEdit.Editing;
using System.Security.Cryptography;
using System.Text;

namespace ContextItemPlugin
{
    class Plugin
    {
        public static void Handler(TextArea textarea)
        {
            var selection = textarea.Selection;
            string text = selection.GetText();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                var clipboard = TopLevel.GetTopLevel(textarea.GetVisualParent())?.Clipboard;
                if (clipboard is not null)
                {
                    clipboard.SetTextAsync(builder.ToString());
                }
            }
        }
    }
}
```

2) Show lines with specified substring in table

```
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaEdit.Editing;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Input;
using Avalonia.Themes.Fluent;
using Avalonia.Interactivity;

namespace ContextItemPlugin
{
    class Plugin
    {
        public static void ShowDataGridResults(List<string> result)
        {
            var w1 = new Window();
            var panel = new Panel();
           var grid = new ListBox();
		
            var t_result = new ObservableCollection<string>(result);
            grid.ItemTemplate = new FuncDataTemplate<string>((s, ns) => 
    		new SelectableTextBlock { Text = s });
            grid.ItemsSource = t_result;
            grid.Focusable=false;
            panel.Children.Add(grid);
            w1.Content = panel;
            w1.Show();
        }

        public static void Handler(TextArea textarea)
        {
            var selection = textarea.Selection;
	    string text = selection.GetText();
            //string text = textarea.Document.Text;

            string[] lines = text
                .Split(
                    new[] { Environment.NewLine, "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );

            var result = new List<string>();

            foreach (string line in lines)
            {
                if (line.Contains("OperationBody"))
                {
                    string prep_line = line.TrimEnd('\r', '\n');
                    if (!result.Contains(prep_line))
                    {
                        result.Add(prep_line.Replace("\"OperationBody\" :","")); 
                    }
                }
            }

            ShowDataGridResults(result);
        }
    }
}
```