using esHelper.Common;
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
    public sealed partial class Page_Query : Page
    {
        EsSystemData esdata;
        public Page_Query()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null) return;

            esdata = e.Parameter as EsSystemData;
            if (esdata.Tag != null)
            {
                txtBoxCommand.Text = AutoIndent(esdata.Tag.ToString());
            }
        }

        #region search
        private async void AppBarButtonRun_Click(object sender, RoutedEventArgs e)
        {
            PageUtil.SetLoadingCursor();
            try
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
            catch
            {

            }
            PageUtil.SetDefaultCursor();
        }

        private void ShowResult(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                result = "";
            }
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
            txtBoxCommand.SelectedText = AutoIndent(selTxt);
        }

        /// <summary>
        /// json格式化
        /// </summary>
        /// <param name="selTxt"></param>
        private string AutoIndent(string selTxt)
        {
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
                            return arrCommandTxt[0].Trim('\r') + "\r" + jobject.ToString();
                        }
                    }
                    catch
                    {

                    }
                }
            }
            return "";
        }
        #endregion

    }
}
