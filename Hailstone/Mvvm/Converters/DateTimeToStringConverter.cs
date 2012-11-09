using System;

namespace Hailstone.Mvvm.Converters
{
    public class DateTimeToStringConverter : OneWayConverter<DateTime, string>
    {
        public override string ToTarget(DateTime input, object parameter)
        {
            try
            {
                var format = parameter as string;
                if (String.IsNullOrEmpty(format))
                    return input.ToString();
                else
                    return input.ToString(format);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return input.ToString();
            }
        }
    }
}
