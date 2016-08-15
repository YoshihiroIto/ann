using System.Windows;
using System.Windows.Interactivity;

namespace Ann.Foundation.Control.Behavior
{
    // ReSharper disable once InconsistentNaming
    public class UIElementFocusBehavior : Behavior<UIElement>
    {
        #region IsFocused

        public bool IsFocused
        {
            get { return (bool) GetValue(IsFocusedProperty); }
            set { SetValue(IsFocusedProperty, value); }
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                nameof(IsFocused),
                typeof(bool),
                typeof(UIElementFocusBehavior),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(bool),
                    BindsTwoWayByDefault = true
                }
                );

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.GotFocus += AssociatedObjectOnGotFocus;
            AssociatedObject.LostFocus += AssociatedObjectOnLostFocus;
            AssociatedObject.IsVisibleChanged += AssociatedObjectOnIsVisibleChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged += AssociatedObjectOnIsVisibleChanged;
            AssociatedObject.LostFocus -= AssociatedObjectOnLostFocus;
            AssociatedObject.GotFocus -= AssociatedObjectOnGotFocus;

            base.OnDetaching();
        }

        private void AssociatedObjectOnIsVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (AssociatedObject.IsVisible == false)
                IsFocused = false;
        }

        private void AssociatedObjectOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            IsFocused = true;
        }

        private void AssociatedObjectOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            IsFocused = false;
        }
    }
}