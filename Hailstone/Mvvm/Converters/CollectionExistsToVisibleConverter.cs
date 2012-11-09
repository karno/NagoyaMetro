using System.Collections;
using System.Linq;
using System.Windows;

namespace Hailstone.Mvvm.Converters
{
    public class CollectionExistsToVisibleConverter : OneWayConverter<IEnumerable, Visibility>
    {
        public override Visibility ToTarget(IEnumerable input, object parameter)
        {
            return input.Cast<object>().Count() > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public class CollectionExistsToInvisibleConverter : OneWayConverter<IEnumerable, Visibility>
    {
        public override Visibility ToTarget(IEnumerable input, object parameter)
        {
            return input.Cast<object>().Count() > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
