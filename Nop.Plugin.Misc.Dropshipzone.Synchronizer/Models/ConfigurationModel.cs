using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer.UserName")]

        public string UserName { get; set; }
        public bool UserName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer.Password")]

        public string Password { get; set; }
        public bool Password_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer.ToEmail")]

        public string ToEmail { get; set; }
        public bool ToEmail_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer.ToName")]

        public string ToName { get; set; }  
        public bool ToName_OverrideForStore { get; set; }

    }
}
