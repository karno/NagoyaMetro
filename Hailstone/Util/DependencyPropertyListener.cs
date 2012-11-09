using System;
using System.Windows;
using System.Windows.Data;

namespace Hailstone.Util
{
    public class DependencyPropertyListener : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DependencyPropertyListener), new PropertyMetadata((sender, e) =>
        {
            if (e.OldValue != e.NewValue)
            {
                var handler = ((DependencyPropertyListener)sender).Changed;
                if (handler != null)
                    handler(sender, EventArgs.Empty);
            }
        }));

        public event EventHandler Changed;

        public object Value
        {
            get
            {
                return (object)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public void Listen(Binding binding)
        {
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }
    }
}