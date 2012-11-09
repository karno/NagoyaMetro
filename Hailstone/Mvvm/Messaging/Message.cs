using GalaSoft.MvvmLight.Messaging;

namespace Hailstone.Mvvm.Messaging
{
    public abstract class Message 
        : global::GalaSoft.MvvmLight.Messaging.MessageBase
    {
        /// <summary>
        /// Identifier token of this message.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Initialize message.
        /// </summary>
        public Message() : this(null) { }

        /// <summary>
        /// Initialize message with specific key.
        /// </summary>
        /// <param name="key">key of this message</param>
        public Message(string key)
        {
            Key = key;
        }
    }
}
