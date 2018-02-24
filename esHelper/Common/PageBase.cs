using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace esHelper.Common
{
    public class PageBase : Page
    {

        public PageBase() {
            SolidColorBrush scb = new SolidColorBrush();
            Color color = new Color() { R = 245, G = 245, B = 245 };
            scb.Color = color;
            this.Background = scb;
        }
    }
}
