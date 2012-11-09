using Hailstone.Mvvm.Messaging.Messages;
using Microsoft.Expression.Interactivity;

namespace Hailstone.Mvvm.Messaging
{
    public class GoToStateMessageAction : MessageActionBase<GoToStateMessage>
    {
        protected override void Invoke(GoToStateMessage message)
        {
            try
            {
                message.Result = VisualStateUtilities.GoToState(
                    this.AssociatedObject, message.StateName, message.UseTransisions);
            }
            catch { }
        }
    }
}
