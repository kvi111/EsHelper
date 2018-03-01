using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace esHelper.Converter
{
    public sealed class NodeExpendIconStateConverter : IValueConverter
    {
        public ImageSource OpenImage { get; set; }
        public ImageSource CloseImage { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
            var isConnect = value as bool?;

            if (isConnect.HasValue && isConnect.Value)
            {
                return OpenImage;
            }
            else
            {
                return CloseImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
