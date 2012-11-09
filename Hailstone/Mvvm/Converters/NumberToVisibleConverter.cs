using System.Windows;

namespace Hailstone.Mvvm.Converters
{
    public class NumberToVisibleConverter : OneWayConverter<int, Visibility>
    {
        public override Visibility ToTarget(int input, object parameter)
        {
            return input == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public class NumberToInvisibleConverter : OneWayConverter<int, Visibility>
    {
        public override Visibility ToTarget(int input, object parameter)
        {
            return input != 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
