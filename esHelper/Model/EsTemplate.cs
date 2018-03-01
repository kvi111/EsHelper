using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esHelper.Model
{
    public class EsTemplate
    {
        public string Name { get; set; }
        public string IndexName { get; set; }

        public JToken Mapping { get; set; }
    }
}
