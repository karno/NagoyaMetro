using System.Windows;
using Hailstone.Mvvm.Messaging.Messages;

namespace Hailstone.Mvvm.Messaging
{
    public class MessageBoxMessageAction : MessageActionBase<MessageBoxMessage>
    {
        protected override void Invoke(MessageBoxMessage message)
        {
            if (message.Caption == null)
                message.Result = MessageBox.Show(message.Text);
            else
                message.Result = MessageBox.Show(message.Text, message.Caption, message.Button);
        }
    }
}
