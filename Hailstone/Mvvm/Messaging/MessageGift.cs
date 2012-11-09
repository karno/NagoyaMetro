using GalaSoft.MvvmLight.Messaging;

namespace Hailstone.Mvvm.Messaging
{
    /// <summary>
    /// Message wrapper for MVVM Light Messenger
    /// </summary>
    public class MessageGift : MessageBase
    {
        public MessageGift(Message content)
        {
            this.Content = content;
        }

        public Message Content { get; set; }
    }
}
