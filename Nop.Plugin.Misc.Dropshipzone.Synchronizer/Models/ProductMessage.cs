using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Models
{
    public class ProductMessage
    {
        public string Sku { get; set; }

        public int OldStockQuantity { get; set; }
        public int NewStockQuantity { get; set; }

        public decimal OldOldPrice { get; set; }
        public decimal NewOldPrice { get; set; }

        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }

        public decimal OldProductCost { get; set; }
        public decimal NewProductCost { get; set; }

        public bool OldPublish { get; set; }
        public bool NewPublish { get; set; }

        public string Comment { get; set; }

    }
}
