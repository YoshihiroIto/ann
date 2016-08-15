using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Reactive.Bindings.Extensions;

namespace Ann.Foundation.Control.Behavior
{
    public class ListBoxLastItemFocusOnItemAddedBehavior : Behavior<ListBox>
    {
        #region ItemType

        public Type ItemType
        {
            get { return (Type)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register(
                nameof (ItemType),
                typeof (Type),
                typeof (ListBoxLastItemFocusOnItemAddedBehavior),
                new FrameworkPropertyMetadata
                {
                    DefaultValue            = default(Type),
                    BindsTwoWayByDefault    = true
                }
            );

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObjectOnLoaded;

            _ItemsChangedObservable.Dispose();

            base.OnDetaching();
        }

        private IDisposable _ItemsChangedObservable;

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _ItemsChangedObservable = AssociatedObject.Items.CollectionChangedAsObservable()
                .Subscribe(e =>
                {
                    if (e.Action != NotifyCollectionChangedAction.Add)
                        return;

                    AssociatedObject.UpdateLayout();

                    var item = AssociatedObject
                        .ItemContainerGenerator
                        .ContainerFromIndex(e.NewStartingIndex);

                    var inputBox = WpfHelper.FindChild(item, ItemType) as UIElement;
                    inputBox?.Focus();
                });
        }
    }
}