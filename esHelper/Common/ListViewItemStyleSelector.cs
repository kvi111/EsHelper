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
    public class ListViewItemStyleSelector : StyleSelector
    {
        //protected override Style SelectStyleCore(object item, DependencyObject container)
        //{
        //    return base.SelectStyleCore(item, container);
        //}
        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            Style st = new Style();
            st.TargetType = typeof(ListViewItem);
            Setter backGroundSetter = new Setter();
            backGroundSetter.Property = ListViewItem.BackgroundProperty;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(container) as ListView;
            int index = listView.IndexFromContainer(container);
            if (index % 2 == 0)
            {
                backGroundSetter.Value = Application.Current.Resources["my_Brush_ListViewItem_Background1"] as SolidColorBrush;
            }
            else
            {
                backGroundSetter.Value = Application.Current.Resources["my_Brush_ListViewItem_Background2"] as SolidColorBrush;
            }
            st.Setters.Add(backGroundSetter);
            return st;
        }
    }
}
