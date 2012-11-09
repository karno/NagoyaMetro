using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Markup;
using System.Globalization;

namespace Hailstone.Toolkit
{
    public class LocalizedContextMenu : ContextMenu
    {
        public LocalizedContextMenu()
            : base()
        {
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name);
        }
    }
}
