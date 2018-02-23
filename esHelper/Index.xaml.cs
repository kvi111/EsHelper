using esHelper.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TreeViewControl;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        SortedSet<string> indexNameSet = new SortedSet<string>();
        EsSystemData esdata;
        int pageIndex = 0;
        int totalPageCount = 0;
        string indexName = "";
        public Index()
        {
            this.InitializeComponent();
        }

        #region index
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null) return;
            TreeViewItemClickEventArgs args = e.Parameter as TreeViewItemClickEventArgs;
            TreeNode node = args.ClickedItem as TreeNode;
            esdata = node.ParentNode.Data as EsSystemData;

            await InitData(false);
        }

        private async Task InitData(bool isShowSysIndex)
        {
            listview1.ItemsSource = null;

            List<EsIndex> listIndex = new List<EsIndex>();
            string[] indexs = await EsService.GetIndexList(esdata.EsConnInfo);
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

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            await InitData(ToggleSwitch1.IsOn);
        }

        int rowIndex = 0;
        private async void HyperlinkButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            pageIndex = 0;
            totalPageCount = 0;
            HyperlinkButton btn = sender as HyperlinkButton;
            indexName = btn.CommandParameter.ToString();
            await GetBrowsePageData(indexName, pageIndex);
        }

        private async Task GetBrowsePageData(string indexName, int pIndex)
        {
            PerPageData perPageData = await EsService.GetIndexData(esdata.EsConnInfo, indexName, pIndex);
            pageIndex = perPageData.pageIndex;
            totalPageCount = perPageData.totalPageCount;
            textBlockPageIndex.Text = (perPageData.pageIndex + 1).ToString();
            textBlockTotalPageCount.Text = perPageData.totalPageCount.ToString();
            pivot1.SelectedIndex = 1;
            if (perPageData != null)
            {
                JObject jObject = perPageData.pageData as JObject;
                JArray arrData = jObject.Root["hits"]["hits"] as JArray;
                if (arrData.Count > 0)
                {
                    rowIndex = 0;

                    gridData.ColumnSpacing = 2;
                    gridData.RowSpacing = 5;
                    gridData.Children.Clear();
                    gridData.RowDefinitions.Clear();
                    gridData.ColumnDefinitions.Clear();

                    gridData.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); //添加一行, 存放标题栏
                    foreach (JObject jObj in arrData) //行
                    {
                        gridData.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); //添加一行
                        GetAllProperty(jObj, 0);
                        rowIndex++;
                    }
                    gridData.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) }); //添加一行
                }
            }
        }

        private void GetAllProperty(JObject jObject, int columnIndex)
        {
            foreach (JProperty jpro in jObject.Properties())
            {
                if (jpro.Value is JValue)
                {
                    if (rowIndex == 0) //存标题
                    {
                        gridData.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 100, MaxWidth = 300 }); //添加一列

                        Border border = new Border();
                        //border.BorderThickness = 1;
                        border.Background = new SolidColorBrush(Colors.AliceBlue);

                        TextBlock tb = new TextBlock();
                        tb.Text = jpro.Name;
                        tb.IsTextSelectionEnabled = true;
                        tb.FontSize = tb.FontSize + 2;
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Center;

                        border.Child = tb;

                        gridData.Children.Add(border);
                        Grid.SetColumn(border, columnIndex);
                        Grid.SetRow(border, rowIndex);
                    }

                    TextBlock tb1 = new TextBlock();
                    tb1.Text = jpro.Value.ToString();
                    tb1.IsTextSelectionEnabled = true;

                    gridData.Children.Add(tb1);
                    Grid.SetColumn(tb1, columnIndex);
                    Grid.SetRow(tb1, rowIndex + 1);

                    columnIndex++;
                }
                else if (jpro.Value is JObject)
                {
                    GetAllProperty((JObject)jpro.Value, columnIndex);
                }
            }
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
            var dialog = new MessageDialog("are you sure delete this index?");

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
            if (cdni.result!=null && cdni.result.Success)
            {
                ToggleSwitch_Toggled(sender, e); //重新加载索引列表
            }
        }
        #endregion

        #region browse
        private async void AppBarButtonFirst_Click(object sender, RoutedEventArgs e)
        {
            if (pageIndex != 0)
            {
                pageIndex = 0;
                await GetBrowsePageData(indexName, pageIndex);
            }
        }
        private async void AppBarButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (pageIndex > 0)
            {
                pageIndex--;
                await GetBrowsePageData(indexName, pageIndex);
            }
        }
        private async void AppBarButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if ((pageIndex + 1) <= (totalPageCount - 1))
            {
                pageIndex++;
                await GetBrowsePageData(indexName, pageIndex);
            }
        }
        private async void AppBarButtonLast_Click(object sender, RoutedEventArgs e)
        {
            if ((totalPageCount - 1) >= 0 && pageIndex != (totalPageCount - 1))
            {
                await GetBrowsePageData(indexName, totalPageCount - 1);
            }
        }
        

        private async void ButtonGO_Click(object sender, RoutedEventArgs e)
        {
            int goPageIndex = 1;
            if (int.TryParse(textBoxPageIndex.Text, out goPageIndex))
            {
                if (goPageIndex > 0 && goPageIndex < totalPageCount)
                {
                    await GetBrowsePageData(indexName, goPageIndex - 1);
                }
            }
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            if (!Regex.IsMatch(textbox.Text, "^\\d*\\.?\\d*$") && textbox.Text != "")
            {
                int pos = textbox.SelectionStart - 1;
                textbox.Text = textbox.Text.Remove(pos, 1);
                textbox.SelectionStart = pos;
            }

        }
        #endregion
    }
}
