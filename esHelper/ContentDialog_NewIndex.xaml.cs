using esHelper.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace esHelper
{
    public sealed partial class ContentDialog_NewIndex : ContentDialog
    {
        public EsSystemData esdata;
        public StackPanel lastCheckSP;
        public FuncResult result;
        public ContentDialog_NewIndex()
        {
            this.InitializeComponent();

        }

        private bool checkLast()
        {
            lastCheckSP = (StackPanel)spContent.Children[spContent.Children.Count - 1];
            if (lastCheckSP.Children[1] is TextBox)
            {
                TextBox tb = (TextBox)lastCheckSP.Children[1];
                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.Focus(FocusState.Keyboard);
                    return false;
                }
            }
            return true;
        }
        private bool checkAllField(ref string json)
        {
            json = "{\"mappings\": { \"" + TypeName.Text + "\":{ \"properties\": {";

            foreach (StackPanel sp in spContent.Children)
            {
                if (sp.Children[1] is TextBox)
                {
                    TextBox tb = (TextBox)sp.Children[1];
                    if (string.IsNullOrEmpty(tb.Text.Trim()))
                    {
                        tb.Focus(FocusState.Keyboard);
                        (new MessageDialog("please input the field name.")).ShowAsync();
                        return false;
                    }
                    else
                    {
                        ComboBox comb2 = sp.Children[2] as ComboBox;
                        ComboBoxItem cbitem2 = comb2.SelectedItem as ComboBoxItem;
                        json += "\"" + tb.Text.Trim() + "\":{ \"type\":\"" + cbitem2.Content.ToString() + "\"";

                        ComboBox comb3 = sp.Children[3] as ComboBox;
                        if (comb3.SelectedItem != null)
                        {
                            ComboBoxItem cbitem3 = comb3.SelectedItem as ComboBoxItem;
                            if (cbitem3.Content != null && string.IsNullOrEmpty(cbitem3.Content.ToString()) == false) //选择了Analyzer
                            {
                                json += ",\"analyzer\": \"" + cbitem3.Content.ToString() + "\"";
                            }
                        }

                        CheckBox chkb3 = sp.Children[4] as CheckBox;
                        if (chkb3.IsChecked == false)
                        {
                            json += ",\"index\": false";
                        }

                        json += "},";
                    }
                }
            }
            json = json.Trim(',') + "}}}}";
            return true;
        }

        private StackPanel GetSP()
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            StackPanel spcb = new StackPanel();
            spcb.Width = 40;
            spcb.VerticalAlignment = VerticalAlignment.Top;
            RadioButton rb = new RadioButton();
            rb.GroupName = "sel";
            rb.Margin = new Thickness(10, 0, 0, 0);
            rb.Checked += RadioButton_Checked;
            spcb.Children.Add(rb);
            sp.Children.Add(spcb);

            TextBox tb = new TextBox();
            tb.Width = 160;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.BorderThickness = new Thickness(0.1);
            sp.Children.Add(tb);

            ComboBox cbox = new ComboBox();
            cbox.Width = 120;
            cbox.BorderThickness = new Thickness(0.1);
            foreach (ComboBoxItem item in comboxDataType.Items)
            {
                cbox.Items.Add(new ComboBoxItem() { Content = item.Content == null ? "" : item.Content, IsSelected = item.Content.ToString() == "text" ? true : false });
            }
            sp.Children.Add(cbox);

            ComboBox cbox1 = new ComboBox();
            cbox1.Width = 120;
            cbox1.BorderThickness = new Thickness(0.1);
            foreach (ComboBoxItem item in comboxAnalyzer.Items)
            {
                cbox1.Items.Add(new ComboBoxItem() { Content = item.Content == null ? "" : item.Content });
            }
            cbox1.SelectionChanged += comboxAnalyzer_SelectionChanged;
            sp.Children.Add(cbox1);

            CheckBox cb = new CheckBox();
            cb.Width = 60;
            cb.IsChecked = true;
            cb.VerticalAlignment = VerticalAlignment.Top;
            cb.Margin = new Thickness(20, 0, 0, 0);
            cb.BorderThickness = new Thickness(0.1);
            sp.Children.Add(cb);
            return sp;
        }

        private void AppBarButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (spContent.Children.Count > 0)
            {
                string json = "";
                if (checkAllField(ref json) == false)
                {
                    return;
                }
            }

            StackPanel sp0 = GetSP();
            spContent.Children.Add(sp0);
        }

        private void AppBarButtonInsert_Click(object sender, RoutedEventArgs e)
        {
            string json = "";
            if (checkAllField(ref json) == false)
            {
                return;
            }
            int index = spContent.Children.IndexOf(lastCheckSP);
            if (index >= 0)
            {
                spContent.Children.Insert(index, GetSP());
            }
        }

        private void AppBarButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lastCheckSP != null && lastCheckSP is StackPanel)
            {
                spContent.Children.Remove(lastCheckSP);

                if (spContent.Children.Count > 0)
                {
                    StackPanel sp = (StackPanel)spContent.Children[spContent.Children.Count - 1];
                    if (sp.Children.First() is StackPanel)
                    {
                        StackPanel sp1 = (StackPanel)sp.Children.First();
                        if (sp1.Children.First() is RadioButton)
                        {
                            RadioButton rb = (RadioButton)sp1.Children.First();
                            rb.IsChecked = true;
                        }
                    }
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            StackPanel sp = (StackPanel)rb.Parent;
            lastCheckSP = (StackPanel)sp.Parent;
        }

        private void comboxAnalyzer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox cb = (ComboBox)sender;
            //StackPanel sp = cb.Parent as StackPanel;
            //CheckBox chkbox = sp.Children.Last() as CheckBox;
            //ComboBoxItem comboxItem = (ComboBoxItem)cb.SelectedItem;
            //if (comboxItem.Content != null && string.IsNullOrEmpty(comboxItem.Content.ToString()) == false)
            //{
            //    chkbox.IsChecked = true;
            //    chkbox.IsEnabled = false;
            //}
            //else
            //{
            //    chkbox.IsEnabled = true;
            //}
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IndexName.Text.Trim()))
            {
                (new MessageDialog("please input the index name.")).ShowAsync();
               
                //args.Cancel = true;
                return;
            }
            if (string.IsNullOrEmpty(TypeName.Text.Trim()))
            {
                (new MessageDialog("please input the type name.")).ShowAsync();
                //args.Cancel = true;
                return;
            }
            string json = "";
            if (checkAllField(ref json) == false)
            {
                //args.Cancel = true;
                return;
            }
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                result = await EsService.CreateIndex(esdata.EsConnInfo, IndexName.Text.Trim(), json);
                if (result.Success == false)
                {
                    //args.Cancel = true;
                    (new MessageDialog(result.Message)).ShowAsync();
                    return;
                }
                this.Hide();
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
