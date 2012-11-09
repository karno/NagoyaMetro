using System.Windows;
using System.Windows.Interactivity;

namespace Hailstone.Mvvm.Messaging
{
    public abstract class MessageActionBase<TMessage, TAssociated>
        : TriggerAction<TAssociated>
        where TAssociated : DependencyObject
        where TMessage : Message
    {
        protected sealed override void Invoke(object parameter)
        {
            var message = parameter as TMessage;
            if (message != null)
                Invoke(message);
        }

        protected abstract void Invoke(TMessage message);
    }

    public abstract class MessageActionBase<TMessage> : MessageActionBase<TMessage, FrameworkElement>
        where TMessage : Message { }
}