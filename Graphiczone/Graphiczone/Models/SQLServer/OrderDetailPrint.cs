using System;
using System.Collections.Generic;

namespace Graphiczone.Models.SQLServer
{
    public partial class OrderDetailPrint
    {
        public string OrdPrintId { get; set; }
        public double? OrdPrintWidth { get; set; }
        public double? OrdPrintHeight { get; set; }
        public string OrdPrintFile { get; set; }
        public string OrdPrintDetail { get; set; }
        public double? OrdPrintPriceset { get; set; }
        public int? OrdPrintNum { get; set; }
        public double? OrdPrintTotal { get; set; }
        public string PrintId { get; set; }
        public string OrPrintId { get; set; }

        public virtual OrderPrint OrPrint { get; set; }
        public virtual Print Print { get; set; }
    }
}
