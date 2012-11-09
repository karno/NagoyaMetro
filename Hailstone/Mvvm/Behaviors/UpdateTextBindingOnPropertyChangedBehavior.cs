using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Hailstone.Mvvm.Behaviors
{
    /// <summary>
    /// Custom behavior that updates the source of a binding on a text box as the text changes.
    /// </summary>
    public class UpdateTextBindingOnPropertyChangedBehavior : Behavior<TextBox>
    {
        private BindingExpression expression;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.expression = this.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            this.AssociatedObject.TextChanged += this.OnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.TextChanged -= this.OnTextChanged;
            this.expression = null;
        }

        private void OnTextChanged(object sender, EventArgs args)
        {
            this.expression.UpdateSource();
        }
    }
}
