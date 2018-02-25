using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ContentDialog_Mapping : ContentDialog
    {
        public ContentDialog_Mapping(string text)
        {
            this.InitializeComponent();
            tbMultiLine.Text = text;

            Style buttonStyle = (Style)Application.Current.Resources["ButtonStyleNormal"];
            PrimaryButtonStyle = buttonStyle;
            SecondaryButtonStyle = buttonStyle;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(tbMultiLine.Text);
            Clipboard.SetContent(dataPackage);

            args.Cancel = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
