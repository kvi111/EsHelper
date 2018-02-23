using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esHelper.Common
{
    public class EsIndex
    {
        public string Color { get; set; }
        public string isOpen { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }
        public string ShardsCount { get; set; }

        public string DocumentCount { get; set; }

        public string DataSpace { get; set; }

    }
}
