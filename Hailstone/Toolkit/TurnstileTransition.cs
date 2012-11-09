using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Controls = Microsoft.Phone.Controls;

namespace Hailstone.Toolkit
{
    public class TurnstileTransition : Controls.TransitionElement
    {
        Controls.TurnstileTransition inner = new Controls.TurnstileTransition();
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(Controls.TurnstileTransitionMode), 
            typeof(TurnstileTransition), new PropertyMetadata(Controls.TurnstileTransitionMode.ForwardIn));

        public Controls.TurnstileTransitionMode Mode
        {
            get
            {
                return (Controls.TurnstileTransitionMode)GetValue(ModeProperty);
            }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

        public TurnstileTransition()
        {
            BindingOperations.SetBinding(inner, Controls.TurnstileTransition.ModeProperty, new Binding("Mode")
            {
                Source = this,
            });
        }

        public override Controls.ITransition GetTransition(UIElement element)
        {
            var rt = inner.GetTransition(element);
            ((PlaneProjection)element.Projection).CenterOfRotationX = -0.1;
            return rt;
        }
    }
}