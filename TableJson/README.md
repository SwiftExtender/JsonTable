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
    static class Plugin
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