using esHelper.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
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

        Brush background1 = Application.Current.Resources["my_Brush_ListViewItem_Background1"] as SolidColorBrush;
        Brush background2 = Application.Current.Resources["my_Brush_ListViewItem_Background2"] as SolidColorBrush;
        public Index()
        {
            this.InitializeComponent();

            //pivot1.Items.Add(new PivotItem() { Header = "111" });
            //PivotItem pi = pivot1.Items[0] as PivotItem;
            //pivot1.Items.RemoveAt(1);
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
            comboxIndex.ItemsSource = listIndex;
            comboxIndex.SelectedIndex = 0;
        }

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            await InitData(ToggleSwitch1.IsOn);
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

        #region search
        private async void AppBarButtonRun_Click(object sender, RoutedEventArgs e)
        {
            string commandTxt = string.IsNullOrEmpty(txtBoxCommand.SelectedText) ? txtBoxCommand.Text.Trim() : txtBoxCommand.SelectedText;
            if (string.IsNullOrEmpty(commandTxt) == false)
            {
                string[] arrCommandTxt = commandTxt.Split(new char[] { '{' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (arrCommandTxt.Length == 1)
                {
                    string[] arr1 = arrCommandTxt[0].ToString().Split(' ');
                    if (arr1.Length == 2)
                    {
                        string method = arr1[0].Trim();
                        string command = arr1[1].Trim();
                        ShowResult(await EsService.RunJson(esdata.EsConnInfo, method, command));
                    }
                }
                else if (arrCommandTxt.Length == 2)  //带{}的命令
                {
                    string[] arr1 = arrCommandTxt[0].ToString().Split(' ');
                    if (arr1.Length == 2)
                    {
                        string method = arr1[0].Trim();
                        string command = arr1[1].Trim().Trim('/');
                        string json = "{" + arrCommandTxt[1].Trim();
                        string result = await EsService.RunJson(esdata.EsConnInfo, method, command, json);
                        ShowResult(result);
                    }
                }
            }
        }

        private void ShowResult(string result)
        {
            try
            {
                JObject jobject = JObject.Parse(result);
                if (jobject != null)
                {
                    txtBoxResult.Text = jobject.ToString();
                }
                else
                {
                    txtBoxResult.Text = result;
                }
            }
            catch
            {
                txtBoxResult.Text = result;
            }
        }

        private void AppBarButtonAutoIndent_Click(object sender, RoutedEventArgs e)
        {
            string selTxt = txtBoxCommand.SelectedText;
            if (string.IsNullOrEmpty(selTxt) == false)
            {
                string[] arrCommandTxt = selTxt.Split(new char[] { '{' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (arrCommandTxt.Length == 2)
                {
                    try
                    {
                        JObject jobject = JObject.Parse("{" + arrCommandTxt[1]);
                        if (jobject != null)
                        {
                            txtBoxCommand.SelectedText = arrCommandTxt[0].Trim('\r') + "\r" + jobject.ToString();
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        #endregion

        private void ImageClose_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Image img = e.OriginalSource as Image;
            string itemHeaderName = img.Tag as string;
            int index = 0;
            foreach (PivotItem pi in pivot1.Items)
            {
                if (itemHeaderName == pi.Header.ToString())
                {
                    pivot1.Items.RemoveAt(index);
                }
                index++;
            }
        }

        #region senior search

        private async void comboxIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EsIndex esIndex = comboxIndex.SelectedItem as EsIndex;

            if (esIndex != null)
            {
                JObject jObject = await EsService.GetIndexMapping(esdata.EsConnInfo, esIndex.Name);
                List<string> list = new List<string>();
                EsService.GetFieldsByJson(jObject, esIndex.Name, list);
                comboxField.Items.Clear();
                foreach (string str in list)
                {
                    comboxField.Items.Add(new ComboBoxItem() { Content = str });
                }
            }
        }

        private void comboxMust_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp0 = new StackPanel();
            sp0.Orientation = Orientation.Horizontal;

            ComboBox comboBoxMust0 = new ComboBox();
            comboBoxMust0.BorderThickness = new Thickness(0.2);
            comboBoxMust0.Margin = new Thickness(80, 0, 0, 0);
            comboBoxMust0.Width = 110;
            foreach (ComboBoxItem item in comboxMust.Items)
            {
                comboBoxMust0.Items.Add(new ComboBoxItem() { Content = item.Content == null ? "" : item.Content, IsSelected = item.IsSelected });
            }
            sp0.Children.Add(comboBoxMust0);

            ComboBox comboxField0 = new ComboBox();
            comboxField0.BorderThickness = new Thickness(0.2);
            //comboxField0.Margin = new Thickness(0, 0, 0, 0);
            comboxField0.Width = 110;
            foreach (ComboBoxItem item in comboxField.Items)
            {
                comboxField0.Items.Add(new ComboBoxItem() { Content = item.Content == null ? "" : item.Content, IsSelected = item.IsSelected });
            }
            sp0.Children.Add(comboxField0);

            spContent.Children.Add(sp0);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
