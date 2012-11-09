using System.Windows;

namespace Hailstone.Mvvm.Converters
{
    public class StringEqVisibleConverter : OneWayConverter<object, Visibility>
    {
        public override Visibility ToTarget(object input, object parameter)
        {
            return (parameter as string) == input.ToString() ?
                Visibility.Visible :
                Visibility.Collapsed;
        }
    }
}
