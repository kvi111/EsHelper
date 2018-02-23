using esHelper.Common;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TreeViewControl;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace esHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static List<Common.EsConnectionInfo> listConnectionInfo = new List<Common.EsConnectionInfo>();
        public TreeNode lastTreeNode;
        public MainPage()
        {
            this.InitializeComponent();

            sampleTreeView.TreeViewItemClick += SampleTreeView_TreeViewItemClick;
            sampleTreeView.RightTapped += SampleTreeView_RightTapped;
            sampleTreeView.DoubleTapped += SampleTreeView_DoubleTapped;

            //TreeNode workFolder = CreateFolderNode("Work Documents");
            //workFolder.Add(CreateFileNode("Feature Functional Spec"));
            //workFolder.Add(CreateFileNode("Feature Schedule"));
            //workFolder.Add(CreateFileNode("Overall Project Plan"));
            //workFolder.Add(CreateFileNode("Feature Resource allocation"));
            //sampleTreeView.RootNode.Add(workFolder);

            //TreeNode remodelFolder = CreateFolderNode("Home Remodel");
            //remodelFolder.IsExpanded = true;
            //remodelFolder.Add(CreateFileNode("Contactor Contact Information"));
            //remodelFolder.Add(CreateFileNode("Paint Color Scheme"));
            //remodelFolder.Add(CreateFileNode("Flooring woodgrain types"));
            //remodelFolder.Add(CreateFileNode("Kitchen cabinet styles"));

            //TreeNode personalFolder = CreateFolderNode("Personal Documents");
            //personalFolder.IsExpanded = true;
            //personalFolder.Add(remodelFolder);

            //sampleTreeView.RootNode.Add(personalFolder);

            //sampleTreeView.ContainerContentChanging += SampleTreeView_ContainerContentChanging;
        }

        private void SampleTreeView_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (e.OriginalSource is ListViewItemPresenter)
            {
                ListViewItemPresenter lvip = (ListViewItemPresenter)e.OriginalSource;
                lastTreeNode = (TreeNode)lvip.Content;
                EsSystemData data = lastTreeNode.Data as EsSystemData;
                if (data != null && data.ItemType == EsTreeItemType.esConnection)
                {
                    lastTreeNode.IsExpanded = !lastTreeNode.IsExpanded;
                    Menu_Open_Click(sender, e);
                }
            }
        }

        private void SampleTreeView_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (e.OriginalSource is ListViewItemPresenter)
            {
                ListViewItemPresenter lvip = (ListViewItemPresenter)e.OriginalSource;
                lastTreeNode = (TreeNode)lvip.Content;
                EsSystemData data = lastTreeNode.Data as EsSystemData;
                if (data != null && data.ItemType == EsTreeItemType.esConnection)
                {
                    ItemMenuFlyout.ShowAt(sampleTreeView, e.GetPosition(sampleTreeView));
                }
            }
            else if (e.OriginalSource is Grid)
            {
                //Grid grid = (Grid)e.OriginalSource;
                //BlankMenuFlyout.ShowAt(sampleTreeView, e.GetPosition(sampleTreeView));
            }
        }

        private void SampleTreeView_TreeViewItemClick(TreeView sender, TreeViewItemClickEventArgs args)
        {
            TreeNode node = (TreeNode)args.ClickedItem;
            EsSystemData data = node.Data as EsSystemData;
            switch (data.ItemType)
            {
                case EsTreeItemType.esIndex:
                    contentFrame.Navigate(typeof(Index), args);
                    break;
                case EsTreeItemType.esSysIndex:
                    break;
                case EsTreeItemType.esPlugin:
                    break;
                case EsTreeItemType.esNode:
                    break;
                    //case default:
                    //    contentFrame.Navigate(typeof(BlankPage1), args);
                    //    break;
            }
        }

        //private void SampleTreeView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        //{
        //    var node = args.Item as TreeNode;
        //    if (node != null)
        //    {
        //        var data = node.Data as FileSystemData;
        //        if (data != null)
        //        {
        //            args.ItemContainer.AllowDrop = data.IsFolder;
        //            args.ItemContainer.Tapped += ItemContainer_Tapped;
        //        }
        //    }
        //}

        private void ExpandCollapseIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            TreeNode obj = (TreeNode)sampleTreeView.SelectedItem;
            obj.IsExpanded = !obj.IsExpanded;
            //contentFrame.Navigate(typeof(BlankPage1), e);
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Menu_Refresh_Click(sender, e);
        }

        private void Page_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //if (sshClient.IsConnected)
            //{
            //    sshClient.Disconnect();
            //    sshClient.Dispose();
            //}
        }

        private void AddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog1 cd1 = new ContentDialog1();
            cd1.ShowAsync();
        }

        private void Menu_Refresh_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            sampleTreeView.RootNode.Clear();

            listConnectionInfo = EsService.GetEsFiles();
            foreach (EsConnectionInfo connInfo in listConnectionInfo)
            {
                TreeNode workFolder = new TreeNode() { Data = new EsSystemData(connInfo.connectionName, EsTreeItemType.esConnection) { EsConnInfo = connInfo } };
                sampleTreeView.RootNode.Add(workFolder);
            }
        }

        private async void Menu_Open_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (lastTreeNode != null && lastTreeNode.Data != null)
                {
                    if (lastTreeNode.HasItems)
                    {
                        return;
                        //lastTreeNode.Clear();
                    }

                    EsSystemData esSD = (EsSystemData)lastTreeNode.Data;

                    if (esSD.EsConnInfo.isUseSSH)
                    {
                        if (esSD.SSHClient == null || esSD.SSHClient.IsConnected == false)
                        {
                            esSD.SSHClient = EsService.GetSshClient(esSD.EsConnInfo);

                        }
                    }

                    if (await EsService.ConnectionTest(esSD.EsConnInfo) == false)   //最终检查是否能获取到Es 版本信息 为判断依据
                    {
                        (new MessageDialog("连接失败！")).ShowAsync();
                        return;
                    }

                    AddTreeNodeChild(lastTreeNode);
                }
            }
            catch
            {
                (new MessageDialog("连接异常！")).ShowAsync();
            }
        }

        private void AddTreeNodeChild(TreeNode tn)
        {
            tn.Add(new TreeNode() { Data = new EsSystemData("Index", EsTreeItemType.esIndex) });
            tn.Add(new TreeNode() { Data = new EsSystemData("SysIndex", EsTreeItemType.esSysIndex) });
            tn.Add(new TreeNode() { Data = new EsSystemData("Plugin", EsTreeItemType.esPlugin) });
            tn.Add(new TreeNode() { Data = new EsSystemData("Node", EsTreeItemType.esNode) });
            //tn.IsExpanded = true;
        }

        private void Menu_Close_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (lastTreeNode != null && lastTreeNode.Data != null)
            {
                EsSystemData esSD = (EsSystemData)lastTreeNode.Data;
                if (esSD != null && esSD.EsConnInfo.isUseSSH && esSD.SSHClient != null && esSD.SSHClient.IsConnected)
                {
                    esSD.SSHClient.Disconnect();
                    esSD.SSHClient.Dispose();
                    esSD.SSHClient = null;
                }
                lastTreeNode.IsExpanded = false;
                lastTreeNode.Clear();
            }
        }
        private async void Menu_Create_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog1 cd1 = new ContentDialog1();
            await cd1.ShowAsync();
            if (cd1.isSuccess)
            {
                //Menu_Refresh_Click(sender, e);
                TreeNode workFolder = new TreeNode() { Data = new EsSystemData(cd1.connInfo.connectionName, EsTreeItemType.esConnection) { EsConnInfo = cd1.connInfo } };
                sampleTreeView.RootNode.Add(workFolder);
            }
        }
        private void Menu_Edit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog1 cd1 = new ContentDialog1();
            cd1.ShowAsync().GetResults();
        }

        private async void Menu_Delete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Are you delete this connection?");

            dialog.Commands.Add(new UICommand("ok", cmd => { }, commandId: 0));
            dialog.Commands.Add(new UICommand("cancel", cmd => { }, commandId: 1));

            //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            //获取返回值
            IUICommand result = await dialog.ShowAsync();
            if (result.Label == "ok")
            {

                if (lastTreeNode != null && lastTreeNode.Data != null)
                {
                    EsSystemData esSD = (EsSystemData)lastTreeNode.Data;

                    Menu_Close_Click(sender, e); //先关闭，再删除

                    EsService.DelEsFile(esSD.EsConnInfo);
                    lastTreeNode.ParentNode.Remove(lastTreeNode);
                }
            }
        }

        private void Menu_Help_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            (new MessageDialog("esHelper for ElasticSearch 6！")).ShowAsync();
        }
    }
}
