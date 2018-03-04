using esHelper.Common;
using Renci.SshNet;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace esHelper
{
    public sealed partial class ContentDialog_Connection : ContentDialog
    {
        public bool isSuccess = false;
        public EsConnectionInfo connInfo;

        public ContentDialog_Connection()
        {
            this.InitializeComponent();

            Style buttonStyle = (Style)Application.Current.Resources["ButtonStyleNormal"];
            PrimaryButtonStyle = buttonStyle;
            SecondaryButtonStyle = buttonStyle;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PageUtil.SetLoadingCursor();
            connInfo = new EsConnectionInfo();
            connInfo.connectionName = connetionName.Text.Trim() == "" ? esIp.Text.Trim() + "_" + esPort.Text.Trim() : connetionName.Text.Trim();

            if (EsService.checkFileName(connInfo.connectionName) == false)
            {
                PageUtil.ShowMsg("Connection name invalid");
                //(new MessageDialog("连接名称无效！")).ShowAsync();
                args.Cancel = true;
                return;
            }

            if (EsService.checkFileExists(connInfo.connectionName))
            {
                PageUtil.ShowMsg("Connection name already exists");
                //(new MessageDialog("连接名称重复！")).ShowAsync();
                args.Cancel = true;
                return;
            }

            connInfo.esIp = esIp.Text.Trim();

            int intLanPort = 0;
            if (int.TryParse(esPort.Text.Trim(), out intLanPort) == false)
            {
                PageUtil.ShowMsg("Lan Port is not correct");
                //(new MessageDialog("内网端口输入不正确！")).ShowAsync();
                args.Cancel = true;
                return;
            }
            connInfo.esPort = intLanPort;

            connInfo.esUsername = esUserName.Text.Trim();
            connInfo.esPassword = esPassword.Text.Trim();

            connInfo.localPort = connInfo.esPort + 1; //把本地端口设置为es端口加1

            connInfo.isUseSSH = isUseSSH.IsOn;
            if (isUseSSH.IsOn)
            {
                if (string.IsNullOrEmpty(sshIp.Text.Trim()) || string.IsNullOrEmpty(sshPort.Text.Trim()) || string.IsNullOrEmpty(userName.Text.Trim()))
                {
                    PageUtil.ShowMsg("Lan Port is not correct");
                    //(new MessageDialog("必须输入SSH主机名、端口和用户名！")).ShowAsync();
                    args.Cancel = true;
                    return;
                }

                connInfo.sshIp = sshIp.Text.Trim();

                int intSshPort = 0;
                if (int.TryParse(sshPort.Text.Trim(), out intSshPort) == false)
                {
                    PageUtil.ShowMsg("Port is not correct");
                    //(new MessageDialog("端口输入不正确！")).ShowAsync();
                    args.Cancel = true;
                    return;
                }
                connInfo.sshPort = intSshPort;

                connInfo.username = userName.Text.Trim();
                connInfo.password = password.Text.Trim();
            }
            try
            {
                SshClient sshClient = null;
                if (connInfo.isUseSSH)
                {
                    sshClient = EsService.GetSshClient(connInfo);  //连接测试
                }

                isSuccess = EsService.ConnectionTest(connInfo);

                if (isSuccess == false)  //最终检查是否能获取到Es 版本信息 为判断依据
                {
                    PageUtil.ShowMsg("Connect fail");
                    //(new MessageDialog("连接失败！")).ShowAsync();
                    args.Cancel = true;
                    return;
                }
                if (connInfo.isUseSSH && sshClient.IsConnected)
                {
                    sshClient.Disconnect();
                    sshClient.Dispose();
                }
                EsService.SaveEsFile(connInfo);
                PageUtil.SetDefaultCursor();

            }
            catch (Exception ex)
            {
                PageUtil.ShowMsg("Connect error");
                //(new MessageDialog("连接异常！")).ShowAsync();
                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void isUseSSH_Toggled(object sender, RoutedEventArgs e)
        {
            sshIp.IsEnabled = isUseSSH.IsOn;
            sshPort.IsEnabled = isUseSSH.IsOn;
            userName.IsEnabled = isUseSSH.IsOn;
            password.IsEnabled = isUseSSH.IsOn;
        }
    }
}
