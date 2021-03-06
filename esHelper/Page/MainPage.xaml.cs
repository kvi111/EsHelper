﻿using esHelper.Common;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TreeViewControl;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
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
        public static MainPage mainPage;

        public List<Common.EsConnectionInfo> listConnectionInfo = new List<Common.EsConnectionInfo>();
        public List<EsIndex> listIndex = new List<EsIndex>();
        public TreeNode lastTreeNode;
        public MainPage()
        {
            this.InitializeComponent();

            sampleTreeView.TreeViewItemClick += SampleTreeView_TreeViewItemClick;
            sampleTreeView.RightTapped += SampleTreeView_RightTapped;
            sampleTreeView.DoubleTapped += SampleTreeView_DoubleTapped;

            contentFrame.Navigate(typeof(Welcome), null);

            mainPage = this;
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Menu_Refresh_Click(sender, e);
        }

        #region treeview

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
                    if (data.IsConnect)
                    {
                        Menu_Open.IsEnabled = false;
                        Menu_Close.IsEnabled = true;
                    }
                    else
                    {
                        Menu_Open.IsEnabled = true;
                        Menu_Close.IsEnabled = false;
                    }
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
            PivotItem pi = pivot1.Items[0] as PivotItem;
            switch (data.ItemType)
            {
                case EsTreeItemType.esIndex:
                    contentFrame.Navigate(typeof(Page_Index), node.ParentNode.Data as EsSystemData);
                    pi.Header = "Index";
                    break;
                case EsTreeItemType.esTemplate:
                    contentFrame.Navigate(typeof(Template), node.ParentNode.Data as EsSystemData);
                    pi.Header = "Template";
                    break;
                case EsTreeItemType.esPlugin:
                    contentFrame.Navigate(typeof(Page_Plugin), node.ParentNode.Data as EsSystemData);
                    pi.Header = "Plugin";
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

        //private void AddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    ContentDialog_Connection cd1 = new ContentDialog_Connection();
        //    cd1.ShowAsync().GetResults();
        //    if (cd1.isSuccess && cd1.connInfo != null)
        //    {
        //        AddNode(cd1.connInfo);
        //    }
        //}

        private void AddNode(EsConnectionInfo connInfo)
        {
            TreeNode workFolder = new TreeNode() { Data = new EsSystemData(connInfo.connectionName, EsTreeItemType.esConnection) { EsConnInfo = connInfo } };
            sampleTreeView.RootNode.Add(workFolder);
        }

        private void AddTreeNodeChild(TreeNode tn)
        {
            tn.Add(new TreeNode() { Data = new EsSystemData("Index", EsTreeItemType.esIndex) });
            tn.Add(new TreeNode() { Data = new EsSystemData("Template", EsTreeItemType.esTemplate) });
            tn.Add(new TreeNode() { Data = new EsSystemData("Plugin", EsTreeItemType.esPlugin) });

            //tn.Add(new TreeNode() { Data = new EsSystemData("Node", EsTreeItemType.esNode) });
            //tn.IsExpanded = true;
        }
        #endregion

        #region menu
        private void Menu_Open_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PageUtil.SetLoadingCursor();
            try
            {
                if (lastTreeNode != null && lastTreeNode.Data != null)
                {
                    if (lastTreeNode.HasItems)
                    {
                        PageUtil.SetDefaultCursor();
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

                    if (EsService.ConnectionTest(esSD.EsConnInfo) == false)   //最终检查是否能获取到Es 版本信息 为判断依据
                    {
                        PageUtil.ShowMsg("connect fail!");
                        //(new MessageDialog("连接失败！")).ShowAsync();
                        return;
                    }
                    (lastTreeNode.Data as EsSystemData).IsConnect = true;
                    lastTreeNode.Data = esSD;
                    if (lastTreeNode.IsExpanded == false)
                    {
                        lastTreeNode.IsExpanded = true;
                    }
                    AddTreeNodeChild(lastTreeNode);
                }
                PageUtil.SetDefaultCursor();
            }
            catch
            {
                PageUtil.ShowMsg("connect error!");
                //(new MessageDialog("连接异常！")).ShowAsync();
            }
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
                //(lastTreeNode.Data as EsSystemData).IsConnect = false;
                esSD.IsConnect = false;
                lastTreeNode.Data = esSD;
                lastTreeNode.IsExpanded = false;
                lastTreeNode.Clear();

                contentFrame.Navigate(typeof(Welcome), e);
            }
        }
        private async void Menu_Create_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog_Connection cd1 = new ContentDialog_Connection();
            await cd1.ShowAsync();
            if (cd1.isSuccess)
            {
                //Menu_Refresh_Click(sender, e);
                TreeNode workFolder = new TreeNode() { Data = new EsSystemData(cd1.connInfo.connectionName, EsTreeItemType.esConnection) { EsConnInfo = cd1.connInfo } };
                sampleTreeView.RootNode.Add(workFolder);
            }
        }
        private void Menu_Refresh_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            sampleTreeView.RootNode.Clear();

            listConnectionInfo = EsService.GetEsFiles();
            foreach (EsConnectionInfo connInfo in listConnectionInfo)
            {
                AddNode(connInfo);
            }
        }

        private void Menu_Edit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ContentDialog1 cd1 = new ContentDialog1();
            //cd1.ShowAsync().GetResults();
        }

        private async void Menu_Delete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Are you sure to delete this connection?");

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

        private void Menu_Query_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AddPivotItem(typeof(Page_Query), "Query");
        }

        public void AddPivotItem(Type sourcePageType, String title)
        {
            if (sampleTreeView.SelectedItem != null)
            {
                TreeNode treeNode = sampleTreeView.SelectedItem as TreeNode;
                EsSystemData esSystemData = treeNode.Data as EsSystemData;

                if (esSystemData.ItemType == EsTreeItemType.esConnection)
                {
                    if (esSystemData.IsConnect)
                    {
                        AddPivotItem(sourcePageType, esSystemData, title + "@" + esSystemData.Name);
                    }
                }
                else
                {
                    esSystemData = treeNode.ParentNode.Data as EsSystemData;
                    if (esSystemData.IsConnect)
                    {
                        AddPivotItem(sourcePageType, esSystemData, title + "@" + esSystemData.Name);
                    }
                }
            }
        }

        /// <summary>
        /// 添加一个新的item，并且加载对应的页面,并且切换过去
        /// </summary>
        /// <param name="sourcePageType"></param>
        /// <param name="esSystemData"></param>
        /// <param name="title"></param>
        public void AddPivotItem(Type sourcePageType, EsSystemData esSystemData, string title)
        {
            PivotItem pi = new PivotItem() { Header = title + "$" + Guid.NewGuid().ToString() };

            Frame frame = new Frame();
            //frame.Background = new SolidColorBrush() { Color = Colors.AliceBlue };
            pi.Content = frame;
            //frame.Tag = esSystemData;
            frame.Navigate(sourcePageType, esSystemData);

            pivot1.Items.Add(pi);
            pivot1.SelectedIndex = pivot1.Items.Count - 1;
        }

        private void Menu_Search_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AddPivotItem(typeof(Page_Search), "Search");
        }

        

        private void Menu_Help_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PageUtil.ShowMsg("esHelper for above ElasticSearch 5！");
            //(new MessageDialog("esHelper for above ElasticSearch 5！")).ShowAsync();
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
    }
}
