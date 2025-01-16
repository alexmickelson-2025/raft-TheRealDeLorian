using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftLibrary
{
    public class RPCData
    {
        public int Term { get; set; }
        public string Entry { get; set; }
        public int SentFrom { get; set; }
    }
}
