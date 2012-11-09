using System;

namespace Hailstone.Mvvm.Messaging.Messages
{
    public class NavigateMessage : Message
    {
        #region Factories

        public static NavigateMessage QueryGoBack(string key = null)
        {
            return new NavigateMessage(key, NavigateType.QueryGoBack, null);
        }

        public static NavigateMessage QueryGoForward(string key = null)
        {
#if WINDOWS_PHONE
            throw new ArgumentException("Can't go forward.");
#else
            return new NavigateMessage(id, NavigateType.QueryGoForward, null);
#endif
        }

        public static NavigateMessage GoBack(string key = null)
        {
            return new NavigateMessage(key, NavigateType.GoBack, null);
        }

        public static NavigateMessage GoForward(string key = null)
        {
#if WINDOWS_PHONE
            throw new ArgumentException("Can't go forward.");
#else
            return new NavigateMessage(id, NavigateType.GoForward, null);
#endif
        }
        
        public static NavigateMessage Navigate(Uri navigateTo)
        {
            return Navigate(null, navigateTo);
        }

        public static NavigateMessage Navigate(string key, Uri navigateTo)
        {
            return new NavigateMessage(key, NavigateType.Navigate, navigateTo);
        }

        #endregion

        private NavigateMessage(string key, NavigateType type, Uri uri)
            : base(key)
        {
            this.NavigateType = type;
            this.NavigateTo = uri;
        }

        public bool Result { get; set; }

        public NavigateType NavigateType { get; private set; }

        public Uri NavigateTo { get; private set; }
    }

    public enum NavigateType
    {
        QueryGoBack,
        QueryGoForward,
        Navigate,
        GoBack,
        GoForward,
    }
}
