using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer
{
    public class DropshipzoneSettings : ISettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string ToEmail { get; set; }
        public string ToName { get; set; }
        //public string Subject { get; set; }
    }
}
