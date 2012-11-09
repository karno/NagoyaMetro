using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Hailstone.Mvvm.Messaging;
using Microsoft.Phone.Reactive;

namespace Hailstone.Mvvm
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
            : base(new Messenger()) { }

        public IMessenger Messenger { get { return this.MessengerInstance; } }

        /// <summary>
        /// Send message asynchronously.
        /// </summary>
        /// <param name="message"></param>
        public void RaiseMessageAsync(Message message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => this.Messenger.Send(new MessageGift(message)));
        }

        protected override void RaisePropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => base.RaisePropertyChanged<T>(propertyExpression));
        }

        /// <summary>
        /// Send message and wait response.
        /// </summary>
        public IObservable<T> RaiseMessageQuery<T>(T message) where T : Message
        {
            return Observable.Return(message)
                .ObserveOnDispatcher()
                .Do(_ => this.Messenger.Send(new MessageGift(_)))
                .ObserveOn(Scheduler.ThreadPool); // return to thread-pool.
        }
    }
}
