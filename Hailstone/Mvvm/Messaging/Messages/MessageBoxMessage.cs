using System.Windows;

namespace Hailstone.Mvvm.Messaging.Messages
{
    public class MessageBoxMessage : Message
    {
        public MessageBoxMessage(string key, string text)
            : this(key, text, null, MessageBoxButton.OK)
        { }

        public MessageBoxMessage(string text)
            : this(text, null, MessageBoxButton.OK)
        { }

        public MessageBoxMessage(string text, string caption, MessageBoxButton button)
            :base()
        {
            this.Text = text;
            this.Caption = caption;
            this.Button = button;
        }

        public MessageBoxMessage(string key, string text, string caption, MessageBoxButton button) 
            : base(key)
        {
            this.Text = text;
            this.Caption = caption;
            this.Button = button;
        }

        public string Text { get; set; }

        public string Caption { get; set; }

        public MessageBoxButton Button { get; set; }

        public MessageBoxResult Result { get; set; }
    }
}
