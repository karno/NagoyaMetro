using System;
using System.Windows.Controls;
using Hailstone.Mvvm.Messaging.Messages;

namespace Hailstone.Mvvm.Messaging
{
    public class NavigateMessageAction : MessageActionBase<NavigateMessage, Page>
    {
        protected override void Invoke(NavigateMessage message)
        {
            switch (message.NavigateType)
            {
                case NavigateType.QueryGoBack:
                    message.Result = this.AssociatedObject.NavigationService.CanGoBack;
                    break;
                case NavigateType.GoBack:
                    if (this.AssociatedObject.NavigationService.CanGoBack)
                    {
                        message.Result = true;
                        try
                        {
                            this.AssociatedObject.NavigationService.GoBack();
                        }
                        catch
                        {
#if DEBUG
                            throw;
#else
                            message.Result = false;
#endif
                        }
                    }
                    else
                    {
                        message.Result = false;
                    }
                    break;

                case NavigateType.QueryGoForward:
                    message.Result = this.AssociatedObject.NavigationService.CanGoForward;
                    break;
                case NavigateType.GoForward:
                    if (this.AssociatedObject.NavigationService.CanGoForward)
                    {
                        message.Result = true;
                        try
                        {
                            this.AssociatedObject.NavigationService.GoForward();
                        }
                        catch
                        {
#if DEBUG
                            throw;
#else
                            message.Result = false;
#endif
                        }
                    }
                    else
                    {
                        message.Result = false;
                    }
                    break;

                case NavigateType.Navigate:
                    try
                    {
                        message.Result = this.AssociatedObject.NavigationService.Navigate(message.NavigateTo);
                    }
                    catch
                    {
#if DEBUG
                        throw;
#else
                        message.Result = false;
#endif
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid navigation type.");
            }
        }
    }
}
