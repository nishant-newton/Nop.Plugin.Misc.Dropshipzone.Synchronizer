using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Models
{
    public class DszProduct
    {
        public string sku { get; set; }
        public string status { get; set; }
        public string stock_qty { get; set; }
        public string Category { get; set; }
        public RRP RRP { get; set; }
        public string RrpPrice { get; set; }
        public int Vendor_Product { get; set; }
        public string brand { get; set; }
        public string cost { get; set; }
        public string currency { get; set; }
        public string desc { get; set; }
        public string discontinuedproduct { get; set; }
        public string eancode { get; set; }
        public int entity_id { get; set; }
        public string height { get; set; }
        public string in_stock { get; set; }
        public string length { get; set; }
        public string price { get; set; }
        public int product_status { get; set; }
        public string title { get; set; }
        public string website_url { get; set; }
        public string weight { get; set; }
        public string width { get; set; }
        public string ETA { get; set; }
        public string bulky_item { get; set; }
        public string colour { get; set; }
        public string discontinued { get; set; }
        public ZoneRates zone_rates { get; set; }
        public List<string> gallery { get; set; }
        public string freeshipping { get; set; }
    }

    public class ZoneRates
    {
        public string sku { get; set; }
        public string act { get; set; }
        public string nsw_m { get; set; }
        public string nsw_r { get; set; }
        public string nt_m { get; set; }
        public string nt_r { get; set; }
        public string qld_m { get; set; }
        public string qld_r { get; set; }
        public string remote { get; set; }
        public string sa_m { get; set; }
        public string sa_r { get; set; }
        public string tas_m { get; set; }
        public string tas_r { get; set; }
        public string vic_m { get; set; }
        public string vic_r { get; set; }
        public string wa_m { get; set; }
        public string wa_r { get; set; }
        public string group_id { get; set; }
        public string created_at { get; set; }
    }

    public class RRP
    {
        public string Standard { get; set; }
    }
}
