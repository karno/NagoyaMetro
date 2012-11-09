using System.Windows.Controls;
using Hailstone.Mvvm.Messaging.Messages;

namespace Hailstone.Mvvm.Messaging
{
    public class TextBoxSetCaretAction : MessageActionBase<TextBoxSetCaretMessage, TextBox>
    {
        protected override void Invoke(TextBoxSetCaretMessage message)
        {
            this.AssociatedObject.SelectionStart = message.SelectionStart;
            this.AssociatedObject.SelectionLength = message.SelectionLength;
        }
    }
}
