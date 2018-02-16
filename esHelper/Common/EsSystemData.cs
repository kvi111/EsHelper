using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esHelper.Common
{
    public class EsSystemData
    {
        public EsSystemData(string name, EsTreeItemType itemType)
        {
            this.Name = name;
            this.ItemType = itemType;
            if (this.ItemType == EsTreeItemType.esConnection)
            {
                IsFolder = true;
            }
        }

        public string Name { get; set; }
        public EsConnectionInfo EsConnInfo { get; set; }
        public SshClient SSHClient { get; set; }
        public EsTreeItemType ItemType { get; set; }

        public bool IsFolder { get; set; }
    }

    public enum EsTreeItemType
    {
        esConnection,
        esNodeFolder,
        esNode,
        esIndexFolder,
        esIndex,
        esSysIndexFolder,
        esSysIndex,
        esShardFolder,
        esShard,
        esPluginFolder,
        esPlugin
    }
}
