﻿using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;

namespace Hailstone.Mvvm.Converters
{
    // Converter templates
    public abstract class TwoWayConverter<TSource, TTarget> : ConvertBase, IValueConverter
    {
        public abstract TTarget ToTarget(TSource input, object parameter);

        public abstract TSource ToSource(TTarget input, object parameter);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ConvertSink<TSource, TTarget>(value, parameter, ToTarget);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ConvertSink<TTarget, TSource>(value, parameter, ToSource);
        }
    }

    public abstract class OneWayConverter<TSource, TTarget> : ConvertBase, IValueConverter
    {
        public abstract TTarget ToTarget(TSource input, object parameter);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ConvertSink<TSource, TTarget>(value, parameter, ToTarget);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public abstract class ConvertBase
    {
        protected static TTarget ConvertSink<TSource, TTarget>(object value, object parameter, Func<TSource, object, TTarget> converter)
        {
            if (IsInDesignTime)
            {
                try
                {
                    return converter((TSource)value, parameter);
                }
                catch
                {
                    return converter(default(TSource), parameter);
                }
            }
            else
            {
                return converter((TSource)value, parameter);
            }
        }

        protected static bool IsInDesignTime
        {
            get
            {
                try
                {
                    return DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual);
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
