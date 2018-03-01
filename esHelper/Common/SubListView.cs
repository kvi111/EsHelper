using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace esHelper.Common
{
    public class SubListView : ListView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is ListViewItem)
            {
                int index = IndexFromContainer(element);
                ListViewItem lvi = element as ListViewItem;
                if (index % 2 == 0)
                {
                    lvi.Background = Application.Current.Resources["my_Brush_ListViewItem_Background1"] as SolidColorBrush;
                }
                else
                {
                    lvi.Background = Application.Current.Resources["my_Brush_ListViewItem_Background2"] as SolidColorBrush;
                }
            }
        }
    }
}
