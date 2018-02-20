using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esHelper.Common
{
    public class FuncResult
    {
        public bool Success { get; set; }
        public String Message { get; set; }
        public int Code { get; set; }
        public object Data { get; set; }
    }
}
