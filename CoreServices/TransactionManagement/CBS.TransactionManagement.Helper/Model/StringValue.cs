using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper.Model
{
    public class StringValue
    {
        public string text { get; set; }
        public string value { get; set; }

        public StringValue(string name, string value) { text = name; this.value = value; }
        public StringValue(string name) { text = name; this.value = name; }

    }
}
