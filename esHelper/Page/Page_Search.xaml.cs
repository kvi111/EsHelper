﻿using esHelper.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class Page_Search : Page
    {
        int pageIndex = 0;
        int totalPageCount = 0;

        SortedSet<string> indexNameSet = new SortedSet<string>();
        EsSystemData esdata;

        Brush background1 = Application.Current.Resources["my_Brush_ListViewItem_Background1"] as SolidColorBrush;
        Brush background2 = Application.Current.Resources["my_Brush_ListViewItem_Background2"] as SolidColorBrush;

        public Page_Search()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null) return;

            esdata = e.Parameter as EsSystemData;

            if (MainPage.mainPage.listIndex.Count == 0)
            {
                MainPage.mainPage.listIndex = await EsService.GetIndexList(esdata.EsConnInfo);
            }
            comboxIndex.ItemsSource = MainPage.mainPage.listIndex;
            comboxIndex.SelectedIndex = 0;
        }

        #region senior search

        private async void comboxIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EsIndex esIndex = comboxIndex.SelectedItem as EsIndex;

            if (esIndex != null)
            {
                List<string> list = await EsService.GetFieldsByJson(esdata.EsConnInfo, esIndex.Name);
                //comboxField.SelectionChanged -= comboxField_SelectionChanged;
                comboxField.Items.Clear();
                comboxField.Items.Add(new ComboBoxItem() { Content = "match_all", IsSelected = true });
                comboxField.Items.Add(new ComboBoxItem() { Content = "_all" });
                foreach (string str in list)
                {
                    comboxField.Items.Add(new ComboBoxItem() { Content = str });
                }
                //comboxField.SelectionChanged += comboxField_SelectionChanged;
                spContent.Children.Clear();
            }
        }

        private void comboxMust_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comField = sender as ComboBox;
            StackPanel spLine = comField.Parent as StackPanel;
            ComboBoxItem comItem = comField.SelectedItem as ComboBoxItem;
            if (comItem != null && comItem.Content != null)
            {
                var spLineChildrenCount = spLine == null ? 3 : spLine.Children.Count;
                if (comItem.Content.ToString() == "match_all")
                {
                    if (spLine != null && spLineChildrenCount == 4)
                    {
                        spLine.Children.RemoveAt(spLineChildrenCount - 1);
                    }
                }
                else if (comItem.Content.ToString() == "_all")
                {

                }
                else
                {
                    if (spLine != null && spLineChildrenCount == 3)
                    {
                        StackPanel spH = new StackPanel();
                        spH.Name = "spH";
                        spH.Orientation = Orientation.Horizontal;

                        ComboBox comboBoxMustSearchKeyword = new ComboBox();
                        comboBoxMustSearchKeyword.BorderThickness = new Thickness(0.2);
                        comboBoxMustSearchKeyword.Margin = new Thickness(10, 0, 0, 0);
                        comboBoxMustSearchKeyword.Width = 110;
                        //comboBoxMustSearchKeyword.SelectionChanged += comboxMust_SelectionChanged;
                        comboBoxMustSearchKeyword.Items.Add(new ComboBoxItem() { Content = "term", IsSelected = true });
                        comboBoxMustSearchKeyword.Items.Add(new ComboBoxItem() { Content = "wildcard" });
                        comboBoxMustSearchKeyword.Items.Add(new ComboBoxItem() { Content = "prefix" });
                        //comboBoxMustSearchKeyword.Items.Add(new ComboBoxItem() { Content = "fuzzy" });
                        //comboBoxMustSearchKeyword.Items.Add(new ComboBoxItem() { Content = "range" });
                        comboBoxMustSearchKeyword.Items.Add(new ComboBoxItem() { Content = "query_string" });
                        comboBoxMustSearchKeyword.Items.Add(new ComboBoxItem() { Content = "text" });
                        comboBoxMustSearchKeyword.Items.Add(new ComboBoxItem() { Content = "missing" });
                        spH.Children.Add(comboBoxMustSearchKeyword);

                        TextBox textBox = new TextBox();
                        textBox.Width = 150;
                        textBox.BorderThickness = new Thickness(0.2);

                        InputScope inputScope = new InputScope();
                        InputScopeName inputScopeName = new InputScopeName();
                        inputScopeName.NameValue = InputScopeNameValue.Digits;
                        inputScope.Names.Add(inputScopeName);

                        textBox.InputScope = inputScope;
                        spH.Children.Add(textBox);

                        spLine.Children.Add(spH);
                    }
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            StackPanel spLine = new StackPanel();
            spLine.Orientation = Orientation.Horizontal;
            spLine.Tag = "sp1";

            Button buttonDel = new Button();
            buttonDel.Margin = new Thickness(40, 0, 0, 0);
            buttonDel.Width = 30;
            buttonDel.Content = "-";
            buttonDel.Style = Application.Current.Resources["ButtonStyleNormal"] as Style;
            buttonDel.Click += ButtonDelete_Click;
            spLine.Children.Add(buttonDel);

            ComboBox comboBoxMust0 = new ComboBox();
            comboBoxMust0.BorderThickness = new Thickness(0.2);
            comboBoxMust0.Margin = new Thickness(10, 0, 0, 0);
            comboBoxMust0.Width = 110;
            comboBoxMust0.SelectionChanged += comboxMust_SelectionChanged;
            foreach (ComboBoxItem item in comboxMust.Items)
            {
                comboBoxMust0.Items.Add(new ComboBoxItem() { Content = item.Content == null ? "" : item.Content, IsSelected = item.Content.ToString() == "must" ? true : false });
            }
            spLine.Children.Add(comboBoxMust0);

            ComboBox comboxField0 = new ComboBox();
            comboxField0.BorderThickness = new Thickness(0.2);
            //comboxField0.Margin = new Thickness(0, 0, 0, 0);
            comboxField0.Width = 200;
            comboxField0.SelectionChanged += comboxField_SelectionChanged;
            foreach (ComboBoxItem item in comboxField.Items)
            {
                comboxField0.Items.Add(new ComboBoxItem() { Content = item.Content == null ? "" : item.Content, IsSelected = item.Content.ToString() == "match_all" ? true : false });
            }
            spLine.Children.Add(comboxField0);

            spContent.Children.Add(spLine);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StackPanel sp = button.Parent as StackPanel;
            spContent.Children.Remove(sp);
        }

        List<string> mustJson = new List<string>();
        List<string> mustnotJson = new List<string>();
        List<string> shouldJson = new List<string>();
        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            mustJson = new List<string>();
            mustnotJson = new List<string>();
            shouldJson = new List<string>();


            GetSearchLineCondition(comboxMust.Parent as StackPanel);
            foreach (StackPanel sp in spContent.Children)
            {
                GetSearchLineCondition(sp);
            }
            string indexName = (comboxIndex.SelectedItem as EsIndex).Name;

            string mustStr = "", mustnotStr = "", shouldStr = "";
            foreach (string str in mustJson)
            {
                mustStr += str;
            }
            mustStr = "\"must\":[" + mustStr.Trim(',') + "]";

            foreach (string str in mustnotJson)
            {
                mustnotStr += str;
            }
            mustnotStr = "\"must_not\":[" + mustnotStr.Trim(',') + "]";

            foreach (string str in shouldJson)
            {
                shouldStr += str;
            }
            shouldStr = "\"should\":[" + shouldStr.Trim(',') + "]";

            string json = "\"bool\":{" + mustStr + "," + mustnotStr + "," + shouldStr + "}";
            PerPageData perPageData = await EsService.GetIndexData(esdata.EsConnInfo, indexName, pageIndex: pageIndex, strJson: json);

            int rowIndex = 0;
            gridData.ColumnSpacing = 2;
            gridData.RowSpacing = 5;
            gridData.Children.Clear();
            gridData.RowDefinitions.Clear();
            gridData.ColumnDefinitions.Clear();
            if (perPageData != null)
            {
                txtJson.Text = perPageData.json;
                pageIndex = perPageData.pageIndex;
                totalPageCount = perPageData.totalPageCount;

                JObject jObject = perPageData.pageData as JObject;
                JArray arrData = jObject.Root["hits"]["hits"] as JArray;
                if (arrData.Count > 0)
                {

                    gridData.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) }); //添加一行, 存放标题栏
                    foreach (JObject jObj in arrData) //行
                    {
                        gridData.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); //添加一行
                        Page_BrowData.GetAllProperty(jObj, 0, rowIndex, gridData);
                        rowIndex++;
                    }
                    gridData.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) }); //添加一行
                }
            }

        }

        private void GetSearchLineCondition(StackPanel stackPanelParent)
        {
            ComboBox comboxMust0 = stackPanelParent.Children[1] as ComboBox;
            ComboBox comboxField0 = stackPanelParent.Children[2] as ComboBox;
            string firstMust = comboxMust0.SelectionBoxItem.ToString(); //(comboxMust.SelectedItem as ComboBoxItem).Content.ToString();

            //StackPanel spP = comboxMust.Parent as StackPanel;
            if (stackPanelParent.Children.Count == 3)
            {
                if (firstMust == "must")
                    mustJson.Add(",{\"match_all\":{}}");
                else if (firstMust == "must_not")
                    mustnotJson.Add(",{\"match_all\":{}}");
                else if (firstMust == "should")
                    shouldJson.Add(",{\"match_all\":{}}");
            }
            else if (stackPanelParent.Children.Count == 4)
            {
                StackPanel spHoriParent = stackPanelParent.Children[3] as StackPanel;
                ComboBox comboBoxTerm = spHoriParent.Children[0] as ComboBox;
                string strTerm = (comboBoxTerm.SelectedItem as ComboBoxItem).Content.ToString();

                string strField = (comboxField0.SelectedItem as ComboBoxItem).Content.ToString();

                TextBox txtBox = spHoriParent.Children[1] as TextBox;

                if (firstMust == "must")
                    mustJson.Add(",{\"" + strTerm + "\":{\"" + strField + "\":\"" + txtBox.Text + "\"}}");
                else if (firstMust == "must_not")
                    mustnotJson.Add(",{\"" + strTerm + "\":{\"" + strField + "\":\"" + txtBox.Text + "\"}}");
                else if (firstMust == "should")
                    shouldJson.Add(",{\"" + strTerm + "\":{\"" + strField + "\":\"" + txtBox.Text + "\"}}");

            }

        }


        #endregion
    }
}
