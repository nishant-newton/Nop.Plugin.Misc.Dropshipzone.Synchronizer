using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Models
{
    public class AuthResponse
    {
        public long iat { get; set; }
        public long exp { get; set; }
        public string token { get; set; }
    }
}
