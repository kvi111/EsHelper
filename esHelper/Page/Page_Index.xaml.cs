using esHelper.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace esHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page_Index : Page
    {
        EsSystemData esdata;

        public Page_Index()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null) return;

            esdata = e.Parameter as EsSystemData;

            await InitData(false);
        }

        #region index
        private async Task InitData(bool isShowSysIndex)
        {
            listview1.ItemsSource = null;

            List<EsIndex> listIndex = await EsService.GetIndexList(esdata.EsConnInfo, isShowSysIndex);
            listview1.ItemsSource = listIndex;
            MainPage.mainPage.listIndex = listIndex;
        }

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            await InitData(false);  //ToggleSwitch1.IsOn
        }

        private async void HyperlinkButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string indexName = btn.CommandParameter.ToString();
            AddPivotItem(typeof(Page_BrowData), indexName);
        }

        /// <summary>
        /// 添加一个新的item，并且加载对应的页面,并且切换过去
        /// </summary>
        /// <param name="sourcePageType"></param>
        /// <param name="indexName"></param>
        private void AddPivotItem(Type sourcePageType, string indexName)
        {
            PivotItem pi = new PivotItem() { Header = indexName + "$" + Guid.NewGuid().ToString() };

            Frame frame = new Frame();
            //frame.Background = new SolidColorBrush() { Color = Colors.AliceBlue };
            pi.Content = frame;
            frame.Tag = esdata;
            frame.Navigate(sourcePageType, indexName);

            PivotItem piParent = (this.Parent as Frame).Parent as PivotItem;
            Pivot pivot1 = piParent.Parent as Pivot;
            pivot1.Items.Add(pi);
            pivot1.SelectedIndex = pivot1.Items.Count - 1;
        }



        private async void HyperlinkButtonMapping_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            JObject jObject = await EsService.GetIndexMapping(esdata.EsConnInfo, btn.CommandParameter.ToString());
            ContentDialog_Mapping mappingDig = new ContentDialog_Mapping(jObject.ToString());
            mappingDig.ShowAsync();
        }

        private async void HyperlinkButtonOpenClose_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            if (btn.Content.ToString() == "open")
            {
                bool result = await EsService.OpenIndex(esdata.EsConnInfo, btn.CommandParameter.ToString());
                if (result == false)
                {
                    (new MessageDialog("open fail")).ShowAsync();
                }
                else
                {
                    ToggleSwitch_Toggled(sender, e); //重新加载索引列表
                }
            }
            if (btn.Content.ToString() == "close")
            {
                bool result = await EsService.CloseIndex(esdata.EsConnInfo, btn.CommandParameter.ToString());
                if (result == false)
                {
                    (new MessageDialog("close fail")).ShowAsync();
                }
                else
                {
                    ToggleSwitch_Toggled(sender, e); //重新加载索引列表
                }
            }
        }
        private async void HyperlinkButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("are you sure to delete this index?");

            dialog.Commands.Add(new UICommand("ok", cmd => { }, commandId: 0));
            dialog.Commands.Add(new UICommand("cancel", cmd => { }, commandId: 1));

            //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            //获取返回值
            var result = await dialog.ShowAsync();
            if (result.Label == "ok")
            {
                HyperlinkButton btn = sender as HyperlinkButton;
                bool resultBool = await EsService.DeleteIndex(esdata.EsConnInfo, btn.CommandParameter.ToString());
                if (resultBool == false)
                {
                    (new MessageDialog("delete fail")).ShowAsync();
                }
                else
                {
                    ToggleSwitch_Toggled(sender, e); //重新加载索引列表
                }
            }
        }
        private async void AppBarButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog_NewIndex cdni = new ContentDialog_NewIndex();
            cdni.esdata = esdata;
            await cdni.ShowAsync();
            if (cdni.result != null && cdni.result.Success)
            {
                ToggleSwitch_Toggled(sender, e); //重新加载索引列表
            }
        }
        #endregion
    }
}
