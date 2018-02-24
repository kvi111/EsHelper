using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace esHelper.Converter
{
    public sealed class GlyphConverter : IValueConverter
    {
        public string ExpandedGlyph { get; set; }
        public string CollapsedGlyph { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isExpanded = value as bool?;

            if (isExpanded.HasValue && isExpanded.Value)
            {
                return ExpandedGlyph;
            }
            else
            {
                return CollapsedGlyph;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
