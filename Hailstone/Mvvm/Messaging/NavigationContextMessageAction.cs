using System.Windows.Controls;
using Hailstone.Mvvm.Messaging.Messages;

namespace Hailstone.Mvvm.Messaging
{
    public class NavigationContextMessageAction : MessageActionBase<NavigationContextMessage, Page>
    {
        protected override void Invoke(NavigationContextMessage message)
        {
            string resp;
            if (this.AssociatedObject.NavigationContext.QueryString.TryGetValue(message.Query, out resp))
                message.Response = resp;
        }
    }
}
