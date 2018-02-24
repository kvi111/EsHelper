using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace esHelper.Converter
{
    public class OpenCloseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string isopen = value as string;
            if (isopen == "open")
            {
                return "close";
            }
            else if (isopen == "close")
            {
                return "open";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
