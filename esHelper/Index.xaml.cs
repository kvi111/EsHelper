using esHelper.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TreeViewControl;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Index : Page
    {
        EsSystemData esdata;
        public Index()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null) return;
            TreeViewItemClickEventArgs args = e.Parameter as TreeViewItemClickEventArgs;
            TreeNode node = args.ClickedItem as TreeNode;
            esdata = node.ParentNode.Data as EsSystemData;

            InitData(false);
        }

        private async void InitData(bool isShowSysIndex)
        {
            listview1.ItemsSource = null;

            List<EsIndex> listIndex = new List<EsIndex>();
            string[] indexs = await EsFile.GetIndexList(esdata.EsConnInfo.GetLastUrl());
            foreach (string str in indexs)
            {
                if (str == "") continue;

                string[] arr = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 10)
                {
                    if (arr[2].StartsWith(".") == false || isShowSysIndex)
                    {
                        listIndex.Add(new EsIndex()
                        {
                            Color = arr[0],
                            isOpen = arr[1],
                            Name = arr[2],
                            Id = arr[3],
                            ShardsCount = arr[4],
                            DocumentCount = arr[6],
                            DataSpace = arr[8]
                        });
                    }
                }
                else if (arr.Length == 3)
                {
                    listIndex.Add(new EsIndex()
                    {
                        isOpen = arr[0],
                        Name = arr[1],
                        Id = arr[2]
                    });
                }
            }

            listview1.ItemsSource = listIndex;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            InitData(ToggleSwitch1.IsOn);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
