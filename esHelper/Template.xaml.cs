using esHelper.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TreeViewControl;
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
    public sealed partial class Template : Page
    {
        EsSystemData esdata;
        public Template()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null) return;

            esdata = e.Parameter as EsSystemData;
            await InitData();
        }

        private async System.Threading.Tasks.Task InitData()
        {
            listview1.ItemsSource = await EsService.GetTemplate(esdata.EsConnInfo);
        }


        private async void HyperlinkButtonMapping_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            JToken jToken = btn.CommandParameter as JToken;

            ContentDialog_Mapping contentDialog_Mapping = new ContentDialog_Mapping(jToken.First.ToString());
            contentDialog_Mapping.ShowAsync();
            //PageUtil.ShowMsg(jToken.First.ToString());
        }

        private async void HyperlinkButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("are you sure to delete this template?");

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
                string tempName = btn.CommandParameter.ToString();
                FuncResult funcResult = await EsService.DeleteTemplate(esdata.EsConnInfo, tempName);
                if (funcResult.Success == false)
                {
                    PageUtil.ShowMsg(funcResult.Message);
                    return;
                }
                else
                {
                    InitData();
                }
            }
        }
    }
}
