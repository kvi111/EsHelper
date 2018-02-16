using esHelper.Common;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class ContentDialog1 : ContentDialog
    {
        public bool isSuccess = false;

        public ContentDialog1()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            EsConnectionInfo connInfo = new EsConnectionInfo();
            connInfo.connectionName = connetionName.Text.Trim() == "" ? esIp.Text.Trim() + "_" + esPort.Text.Trim() : connetionName.Text.Trim();

            if (EsFile.checkFileName(connInfo.connectionName) == false)
            {
                (new MessageDialog("连接名称无效！")).ShowAsync();
                args.Cancel = true;
                return;
            }

            if (EsFile.checkFileExists(connInfo.connectionName))
            {
                (new MessageDialog("连接名称重复！")).ShowAsync();
                args.Cancel = true;
                return;
            }

            connInfo.esIp = esIp.Text.Trim();

            int intLanPort = 0;
            if (int.TryParse(esPort.Text.Trim(), out intLanPort) == false)
            {
                (new MessageDialog("内网端口输入不正确！")).ShowAsync();
                args.Cancel = true;
                return;
            }
            connInfo.esPort = intLanPort;

            connInfo.esUsername = esUserName.Text.Trim();
            connInfo.esPassword = esPassword.Text.Trim();

            connInfo.localPort = connInfo.esPort + 1; //把es端口设置和本地端口一样

            connInfo.isUseSSH = isUseSSH.IsOn;
            if (isUseSSH.IsOn)
            {
                if (string.IsNullOrEmpty(sshIp.Text.Trim()) || string.IsNullOrEmpty(sshPort.Text.Trim()) || string.IsNullOrEmpty(userName.Text.Trim()))
                {
                    (new MessageDialog("必须输入SSH主机名、端口和用户名！")).ShowAsync();
                    args.Cancel = true;
                    return;
                }

                connInfo.sshIp = sshIp.Text.Trim();

                int intSshPort = 0;
                if (int.TryParse(sshPort.Text.Trim(), out intSshPort) == false)
                {
                    (new MessageDialog("端口输入不正确！")).ShowAsync();
                    args.Cancel = true;
                    return;
                }
                connInfo.sshPort = intSshPort;

                connInfo.username = userName.Text.Trim();
                connInfo.password = password.Text.Trim();
            }
            try
            {
                if (connInfo.isUseSSH)
                {
                    SshClient sshClient = Port.GetSshClient(connInfo);  //连接测试
                    sshClient.Disconnect();
                    sshClient.Dispose();
                }
                else
                {
                    string version = await EsFile.GetEsVersion(connInfo.GetLastUrl());
                    if (string.IsNullOrEmpty(version))
                    {
                        (new MessageDialog("连接失败！")).ShowAsync();
                        args.Cancel = true;
                        return;
                    }
                }
                EsFile.SaveEsFile(connInfo);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                (new MessageDialog("连接异常！")).ShowAsync();
                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
