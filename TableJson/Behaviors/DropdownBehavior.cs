using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System.Threading.Tasks;
using System;
using TableJson.ViewModels;

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
                //AssociatedObject.SelectionChanged += SelectionQuery;
                AssociatedObject.Focus();
                Task.Delay(100).ContinueWith(_ => Avalonia.Threading.Dispatcher.UIThread.Invoke(() => { CreatePanel(); }));
            }

            base.OnAttached();
        }

        //private void SelectionQuery(object? sender, SelectionChangedEventArgs e)
        //{
        //    AutoCompleteBox t = (AutoCompleteBox)sender;
        //    JsonQueryMenuItem v = (JsonQueryMenuItem)t.SelectedItem;
        //    AssociatedObject.SelectedItem = v.Query;
        //}

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
            //MainWindowViewModel.UpdateQueries();
            //var prop = AssociatedObject.GetType().GetProperty("TextBox", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //var tb = (TextBox?)prop?.GetValue(AssociatedObject);
        }
        private void ShowDropdown()
        {
            if (AssociatedObject is not null && !AssociatedObject.IsDropDownOpen)
            {
                typeof(AutoCompleteBox).GetMethod("PopulateDropDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(AssociatedObject, new object[] { AssociatedObject, EventArgs.Empty });
                typeof(AutoCompleteBox).GetMethod("OpeningDropDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(AssociatedObject, new object[] { false });

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
            var btn = new Button()
            {
                Content = "↓",
                Margin = new(1),
                ClickMode = ClickMode.Press
            };
            btn.Click += (s, e) => ShowDropdown();
            btn.Margin = new(0, 0, 2, 0);
            return btn;
        }
        private void CreatePanel()
        {
            if (AssociatedObject != null)
            {
                var panel = new DockPanel()
                {
                    Margin = new(1),
                };
                AssociatedObject.InnerRightContent = panel;
                panel.Children.Add(CreateDropdownButton());
                panel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            }
        }

    }
}
