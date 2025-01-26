using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using System.Threading.Tasks;
using System;

namespace TableJson.Behaviors
{
    public class DropdownBehavior : Behavior<AutoCompleteBox>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.KeyUp += OnKeyUp;
                AssociatedObject.DropDownOpening += DropDownOpening;
                AssociatedObject.Focus();
                Task.Delay(100).ContinueWith(_ => Avalonia.Threading.Dispatcher.UIThread.Invoke(() => { CreatePanel(); }));
            }

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.KeyUp -= OnKeyUp;
                AssociatedObject.DropDownOpening -= DropDownOpening;
            }

            base.OnDetaching();
        }

        private void OnKeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Down)
            {
                ShowDropdown();
            }
        }

        private void DropDownOpening(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            //var prop = AssociatedObject.GetType().GetProperty("TextBox", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //var tb = (TextBox?)prop?.GetValue(AssociatedObject);
        }

        private void ShowDropdown()
        {
            if (AssociatedObject is not null && !AssociatedObject.IsDropDownOpen)
            {
                //typeof(AutoCompleteBox).GetMethod("PopulateDropDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(AssociatedObject, new object[] { AssociatedObject, EventArgs.Empty });
                //typeof(AutoCompleteBox).GetMethod("OpeningDropDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(AssociatedObject, new object[] { false });

                //if (!AssociatedObject.IsDropDownOpen)
                {
                    //We *must* set the field and not the property as we need to avoid the changed event being raised (which prevents the dropdown opening).
                    var ipc = typeof(AutoCompleteBox).GetField("_ignorePropertyChange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if ((bool)ipc?.GetValue(AssociatedObject) == false)
                        ipc?.SetValue(AssociatedObject, true);

                    AssociatedObject.SetCurrentValue<bool>(AutoCompleteBox.IsDropDownOpenProperty, true);
                }
            }
        }

        private Button CreateDropdownButton()
        {
            //var prop = AssociatedObject.GetType().GetProperty("TextBox", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //var tb = (TextBox?)prop?.GetValue(AssociatedObject);

            var btn = new Button()
            {
                Content = "↓",
                Margin = new(1),
                ClickMode = ClickMode.Press
            };
            btn.Click += (s, e) => ShowDropdown();

            //tb.InnerRightContent = btn;
            return btn;
        }
        private Button CreateSaveQueryButton()
        {
            var btn = new Button()
            {
                Content = "Save",
                //Margin = new(1),
                ClickMode = ClickMode.Press,
                Width = 50,
                Height = 28
            };
            btn.Background = new SolidColorBrush() { Color = new Color(255, 16, 157, 62) };
            btn.Margin = new(0, 0, 4, 0);
            //btn.Click += (s, e) => CleanTextBox();
            return btn;

        }
        private void CreatePanel()
        {
            if (AssociatedObject != null)
            {
                var panel = new DockPanel()
                {
                    //Content = "Ⅹ",
                    Margin = new(1),
                    //ClickMode = ClickMode.Press
                };
                AssociatedObject.InnerRightContent = panel;
                panel.Children.Add(CreateSaveQueryButton());
                panel.Children.Add(CreateDropdownButton());
                panel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            }
        }

    }
}
