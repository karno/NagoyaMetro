using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Hailstone.Mvvm.Behaviors
{
    public class UpdatePasswordBindingOnPropertyChangedBehavior : Behavior<PasswordBox>
    {
        private BindingExpression expression;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.expression = this.AssociatedObject.GetBindingExpression(PasswordBox.PasswordProperty);
            this.AssociatedObject.PasswordChanged += this.OnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.PasswordChanged -= this.OnTextChanged;
            this.expression = null;
        }

        private void OnTextChanged(object sender, EventArgs args)
        {
            this.expression.UpdateSource();
        }
    }
}
