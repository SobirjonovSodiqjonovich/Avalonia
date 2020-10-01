using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.UnitTests;
using Avalonia.VisualTree;
using ReactiveUI;
using Xunit;

namespace Avalonia.Controls.UnitTests
{
    public class ListBoxTests_Single
    {
        MouseTestHelper _mouse = new MouseTestHelper();
        
        [Fact]
        public void Focusing_Item_With_Tab_Should_Not_Select_It()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            ApplyTemplate(target);

            target.Presenter.Panel.Children[0].RaiseEvent(new GotFocusEventArgs
            {
                RoutedEvent = InputElement.GotFocusEvent,
                NavigationMethod = NavigationMethod.Tab,
            });

            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Focusing_Item_With_Arrow_Key_Should_Select_It()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            ApplyTemplate(target);

            target.Presenter.Panel.Children[0].RaiseEvent(new GotFocusEventArgs
            {
                RoutedEvent = InputElement.GotFocusEvent,
                NavigationMethod = NavigationMethod.Directional,
            });

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Item_Should_Select_It()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            ApplyTemplate(target);
            _mouse.Click(target.Presenter.Panel.Children[0]);

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Selected_Item_Should_Not_Deselect_It()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            ApplyTemplate(target);
            target.SelectedIndex = 0;

            _mouse.Click(target.Presenter.Panel.Children[0]);

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Item_Should_Select_It_When_SelectionMode_Toggle()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Single | SelectionMode.Toggle,
            };

            ApplyTemplate(target);

            _mouse.Click(target.Presenter.Panel.Children[0]);

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Selected_Item_Should_Deselect_It_When_SelectionMode_Toggle()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Toggle,
            };

            ApplyTemplate(target);
            target.SelectedIndex = 0;

            _mouse.Click(target.Presenter.Panel.Children[0]);

            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Selected_Item_Should_Not_Deselect_It_When_SelectionMode_ToggleAlwaysSelected()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Toggle | SelectionMode.AlwaysSelected,
            };

            ApplyTemplate(target);
            target.SelectedIndex = 0;

            _mouse.Click(target.Presenter.Panel.Children[0]);

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Clicking_Another_Item_Should_Select_It_When_SelectionMode_Toggle()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Single | SelectionMode.Toggle,
            };

            ApplyTemplate(target);
            target.SelectedIndex = 1;

            _mouse.Click(target.Presenter.Panel.Children[0]);

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Setting_Item_IsSelected_Sets_ListBox_Selection()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            ApplyTemplate(target);

            ((ListBoxItem)target.GetLogicalChildren().ElementAt(1)).IsSelected = true;

            Assert.Equal("Bar", target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);
        }

        private class TestDataContext : ReactiveObject
        {
            private int _counter;
            private string _selectedItem;

            public TestDataContext()
            {
                Items1 = new ObservableCollection<string>(Enumerable.Range(1, 5).Select(i => GenerateItem()));
                Items2 = new ObservableCollection<string>(Enumerable.Range(1, 5).Select(i => GenerateItem()));
            }

            private string GenerateItem() => $"Item {_counter++.ToString()}";

            public ObservableCollection<string> Items1 { get; }

            public ObservableCollection<string> Items2 { get; }

            public string SelectedItem
            {
                get { return _selectedItem; }
                set
                {
                    this.RaiseAndSetIfChanged(ref _selectedItem, value);
                }
            }
        }

        [Fact]
        public void Two_ListBoxes_With_Seperate_Source_Lists_And_Bound_To_Common_Selected_Items()
        {
            var viewModel = new TestDataContext();

            var target1 = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                DataContext = viewModel
            };

            var target2 = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                DataContext = viewModel,
            };

            ApplyTemplate(target1);
            ApplyTemplate(target2);

            target1.Bind(ListBox.ItemsProperty, new Binding("Items1"));
            target1.Bind(ListBox.SelectedItemProperty, new Binding("SelectedItem"));

            target2.Bind(ListBox.ItemsProperty, new Binding("Items2"));
            target2.Bind(ListBox.SelectedItemProperty, new Binding("SelectedItem"));

            var selectedValues = new List<object>();

            viewModel.WhenAnyValue(x => x.SelectedItem)
                .Subscribe(x => selectedValues.Add(x));

            _mouse.Click(target1.Presenter.Panel.Children[0]);
            _mouse.Click(target2.Presenter.Panel.Children[0]);

            Assert.Equal(3, selectedValues.Count);
            Assert.Equal(new[] { null, "Item 0", "Item 5" }, selectedValues.ToArray());
        }

        [Fact]
        public void SelectedItem_Should_Not_Cause_StackOverflow()
        {
            var viewModel = new TestStackOverflowViewModel()
            {
                Items = new List<string> { "foo", "bar", "baz" }
            };

            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                DataContext = viewModel,
                Items = viewModel.Items
            };

            target.Bind(ListBox.SelectedItemProperty,
                new Binding("SelectedItem") { Mode = BindingMode.TwoWay });

            Assert.Equal(0, viewModel.SetterInvokedCount);

            // In Issue #855, a Stackoverflow occured here.
            target.SelectedItem = viewModel.Items[2];

            Assert.Equal(viewModel.Items[1], target.SelectedItem);
            Assert.Equal(1, viewModel.SetterInvokedCount);
        }

        private class TestStackOverflowViewModel : INotifyPropertyChanged
        {
            public List<string> Items { get; set; }

            public int SetterInvokedCount { get; private set; }

            public const int MaxInvokedCount = 1000;

            private string _selectedItem;

            public event PropertyChangedEventHandler PropertyChanged;

            public string SelectedItem
            {
                get { return _selectedItem; }
                set
                {
                    if (_selectedItem != value)
                    {
                        SetterInvokedCount++;

                        int index = Items.IndexOf(value);

                        if (MaxInvokedCount > SetterInvokedCount && index > 0)
                        {
                            _selectedItem = Items[index - 1];
                        }
                        else
                        {
                            _selectedItem = value;
                        }

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                    }
                }
            }
        }

        private Control CreateListBoxTemplate(ITemplatedControl parent, INameScope scope)
        {
            return new ScrollViewer
            {
                Template = new FuncControlTemplate(CreateScrollViewerTemplate),
                Content = new ItemsPresenter
                {
                    Name = "PART_ItemsPresenter",
                    [~ItemsPresenter.ItemsProperty] = parent.GetObservable(ItemsControl.ItemsProperty).ToBinding(),
                }.RegisterInNameScope(scope)
            };
        }

        private Control CreateScrollViewerTemplate(ITemplatedControl parent, INameScope scope)
        {
            return new ScrollContentPresenter
            {
                Name = "PART_ContentPresenter",
                [~ContentPresenter.ContentProperty] =
                    parent.GetObservable(ContentControl.ContentProperty).ToBinding(),
            }.RegisterInNameScope(scope);
        }

        private void ApplyTemplate(ListBox target)
        {
            // Apply the template to the ListBox itself.
            target.ApplyTemplate();

            // Then to its inner ScrollViewer.
            var scrollViewer = (ScrollViewer)target.GetVisualChildren().Single();
            scrollViewer.ApplyTemplate();

            // Then make the ScrollViewer create its child.
            ((ContentPresenter)scrollViewer.Presenter).UpdateChild();

            // Now the ItemsPresenter should be reigstered, so apply its template.
            target.Presenter.ApplyTemplate();
        }
    }
}