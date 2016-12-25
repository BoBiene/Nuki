using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Nuki.Converter
{
    public class MultiplyConverter : IValueConverter
    {

        public double Value { get; set; }
        public double MinValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double dValue = System.Convert.ToDouble(value);
            if (double.IsNaN(dValue))
                return MinValue;
            else
                return Math.Max(dValue * Value, MinValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
