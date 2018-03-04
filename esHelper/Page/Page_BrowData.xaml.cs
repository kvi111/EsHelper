using esHelper.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    public sealed partial class Page_BrowData : Page
    {
        int pageIndex = 0;
        int totalPageCount = 0;

        string indexName;
        EsSystemData esdata;
        public static Brush background1 = Application.Current.Resources["my_Brush_ListViewItem_Background1"] as SolidColorBrush;
        public static Brush background2 = Application.Current.Resources["my_Brush_ListViewItem_Background2"] as SolidColorBrush;

        public Page_BrowData()
        {
            this.InitializeComponent();

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null) return;

            esdata = e.Parameter as EsSystemData;
            indexName = esdata.Tag.ToString();

            await GetBrowsePageData(indexName, pageIndex);
        }

        private async Task GetBrowsePageData(string indexName, int pIndex)
        {
            PerPageData perPageData = await EsService.GetIndexData(esdata.EsConnInfo, indexName, pIndex);
            pageIndex = perPageData.pageIndex;
            totalPageCount = perPageData.totalPageCount;
            textBlockPageIndex.Text = (perPageData.pageIndex + 1).ToString();
            textBlockTotalPageCount.Text = perPageData.totalPageCount.ToString();
            //pivot1.SelectedIndex = 1;

            int rowIndex = 0;
            gridData.ColumnSpacing = 2;
            gridData.RowSpacing = 5;
            gridData.Children.Clear();
            gridData.RowDefinitions.Clear();
            gridData.ColumnDefinitions.Clear();
            if (perPageData != null)
            {
                JObject jObject = perPageData.pageData as JObject;
                JArray arrData = jObject.Root["hits"]["hits"] as JArray;
                if (arrData.Count > 0)
                {

                    gridData.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) }); //添加一行, 存放标题栏
                    foreach (JObject jObj in arrData) //行
                    {
                        gridData.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); //添加一行
                        GetAllProperty(jObj, 0, rowIndex, gridData);
                        rowIndex++;
                    }
                    gridData.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) }); //添加一行
                }
            }
        }

        public static void GetAllProperty(JObject jObject, int columnIndex,int rowIndex, Grid gridData)
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
                        border.Background = Application.Current.Resources["my_Brush_ListView_Title_Background"] as SolidColorBrush;

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

                    Border border1 = new Border();
                    if (rowIndex % 2 == 0)
                    {
                        border1.Background = background1;
                    }
                    else
                    {
                        border1.Background = background2;
                    }

                    TextBlock tb1 = new TextBlock();
                    tb1.Text = jpro.Value.ToString();
                    tb1.IsTextSelectionEnabled = true;

                    border1.Child = tb1;

                    gridData.Children.Add(border1);
                    Grid.SetColumn(border1, columnIndex);
                    Grid.SetRow(border1, rowIndex + 1);

                    columnIndex++;
                }
                else if (jpro.Value is JObject)
                {
                    GetAllProperty((JObject)jpro.Value, columnIndex, rowIndex, gridData);
                }
            }
        }

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
    }
}
