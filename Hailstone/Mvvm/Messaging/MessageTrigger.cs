using System.Windows;
using System.Windows.Interactivity;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace Hailstone.Mvvm.Messaging
{
    public class MessageTrigger
        : TriggerBase<DependencyObject>
    {
        private IMessenger _messenger = null;
        /// <summary>
        /// CLR Property for the accessing MessengerProperty.
        /// </summary>
        public IMessenger Messenger
        {
            get { return (IMessenger)GetValue(MessengerProperty); }
            set { SetValue(MessengerProperty, value); }
        }

        private string _messageKey = null;
        /// <summary>
        /// Key of receiving message. if set null or empty, accepts any message type matched.
        /// </summary>
        public string MessageKey
        {
            get { return _messageKey; }
            set { _messageKey = value; }
        }

        /// <summary>
        /// Messenger
        /// </summary>
        public static readonly DependencyProperty MessengerProperty =
            DependencyProperty.Register("Messenger", typeof(IMessenger), typeof(MessageTrigger), new PropertyMetadata(null, (o, e) =>
            {
                var tbase = (MessageTrigger)o;
                if (tbase._messenger != null)
                    tbase._messenger.Unregister(tbase);
                tbase._messenger = e.NewValue as IMessenger;
                if (tbase._messenger != null)
                    tbase._messenger.Register<MessageGift>(tbase, tbase.Dispatch);
            }));

        private void Dispatch(MessageGift messageGift)
        {
            var message = messageGift.Content;
            if (string.IsNullOrEmpty(this._messageKey)
                || message.Key == this._messageKey)
            DispatcherHelper.CheckBeginInvokeOnUI(() => InvokeActions(message));
        }
    }
}
