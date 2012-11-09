using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Phone.Controls;

namespace Hailstone.Mvvm.Messaging
{
    public class ShowContextMenuAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var ctx = ContextMenuService.GetContextMenu(this.AssociatedObject);
            if (ctx != null)
                ctx.IsOpen = true;
        }
    }
}
