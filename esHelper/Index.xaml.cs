using esHelper.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        int i = 0;
        private async void HyperlinkButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            JObject jObject = await EsFile.GetIndexData(esdata.EsConnInfo.GetLastUrl(), btn.CommandParameter.ToString(), 0, 50);
            pivot1.SelectedIndex = 1;
            if (jObject != null && jObject.Root["hits"]["hits"] != null)
            {
                JArray arrData = jObject.Root["hits"]["hits"] as JArray;
                if (arrData.Count > 0)
                {
                    i = 0;

                    //gridData.Background = new SolidColorBrush(Colors.Transparent);
                    //gridData.BorderBrush = new SolidColorBrush(Colors.Red);
                    //gridData.BorderThickness = new Thickness(1);
                    gridData.ColumnSpacing = 10;
                    gridData.RowSpacing = 5;
                    gridData.Children.Clear();
                    gridData.RowDefinitions.Clear();
                    gridData.ColumnDefinitions.Clear();

                    gridData.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); //添加一行
                    foreach (JObject jObj in arrData) //行
                    {
                        gridData.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); //添加一行
                        GetAllProperty(jObj, true, 0);
                        i++;
                    }
                    gridData.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) }); //添加一行
                    gridData.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    //scrollView. = gridData.ActualWidth;
                    //scrollView.UpdateLayout();
                    //RowDefinition rd = gridData.RowDefinitions[0];
                    //rd.SetValue(BackgroundProperty, new SolidColorBrush(Colors.AliceBlue));
                }
            }
        }
        private void GetAllProperty(JObject jObject, bool isRoot, int j)
        {
            foreach (JProperty jpro in jObject.Properties())
            {
                if (jpro.Value is JValue)
                {
                    if (i == 0)
                    {
                        Border border = new Border();
                        border.Background = new SolidColorBrush(Colors.AliceBlue);

                        TextBlock tb = new TextBlock();
                        tb.Text = jpro.Name;
                        tb.IsTextSelectionEnabled = true;
                        tb.FontSize = tb.FontSize + 2;

                        border.Child = tb;

                        gridData.Children.Add(border);
                        Grid.SetColumn(border, j);
                        Grid.SetRow(border, i);
                    }

                    TextBlock tb1 = new TextBlock();
                    tb1.Text = jpro.Value.ToString();
                    tb1.IsTextSelectionEnabled = true;


                    gridData.Children.Add(tb1);
                    Grid.SetColumn(tb1, j);
                    Grid.SetRow(tb1, i + 1);

                    gridData.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 100, MaxWidth = 300 }); //添加一列
                    j++;
                }
                else if (jpro.Value is JObject)
                {
                    GetAllProperty((JObject)jpro.Value, false, j);
                }
            }
        }

        private async void HyperlinkButtonMapping_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            JObject jObject = await EsFile.GetIndexMapping(esdata.EsConnInfo.GetLastUrl(), btn.CommandParameter.ToString());
            ContentDialog_Mapping mappingDig = new ContentDialog_Mapping(jObject.ToString());
            mappingDig.ShowAsync();
        }

        private async void HyperlinkButtonOpenClose_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            if (btn.Content.ToString() == "open")
            {
                bool result = await EsFile.OpenIndex(esdata.EsConnInfo.GetLastUrl(), btn.CommandParameter.ToString());
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
                bool result = await EsFile.CloseIndex(esdata.EsConnInfo.GetLastUrl(), btn.CommandParameter.ToString());
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

        private async void AppBarButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog_NewIndex cdni = new ContentDialog_NewIndex();
            cdni.esdata = esdata;
            await cdni.ShowAsync();
        }
        #endregion

        #region browse
        private void AppBarButtonFirst_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AppBarButtonPrevious_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AppBarButtonNext_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AppBarButtonLast_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
