using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KaspScan.Converters
{
    [ContentProperty("Converter")]
    public abstract class ChainConverter<TFrom, TTo> : MarkupExtension, IValueConverter
    {
        public IValueConverter Converter { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;
            object convertedValue = Convert((TFrom)value, targetType, parameter, culture);
            if (Converter != null)
                convertedValue = Converter.Convert(convertedValue, targetType, parameter, culture);
            return convertedValue;
        }

        protected abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;
            object convertedValue = ConvertBack((TTo)value, targetType, parameter, culture);
            if (Converter != null)
                convertedValue = Converter.ConvertBack(convertedValue, targetType, parameter, culture);
            return convertedValue;
        }

        protected abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }}
