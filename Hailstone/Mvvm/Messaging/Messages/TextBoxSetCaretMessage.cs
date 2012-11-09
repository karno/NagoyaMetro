
namespace Hailstone.Mvvm.Messaging.Messages
{
    public class TextBoxSetCaretMessage : Message
    {
        public int SelectionStart { get; set; }

        public int SelectionLength { get; set; }
    }
}
