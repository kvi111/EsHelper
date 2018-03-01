using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace esHelper.Converter
{
    public class StringTxtTrimDotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string txt = value as string;
            if (string.IsNullOrEmpty(txt) == false && txt.Contains("$"))
            {
                return txt.Split('$')[0];
            }
            return txt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
