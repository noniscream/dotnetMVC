using System;
using System.Collections.Generic;

namespace Graphiczone.Models.SQLServer
{
    public partial class Print
    {
        public Print()
        {
            OrderDetailPrint = new HashSet<OrderDetailPrint>();
        }

        public string PrintId { get; set; }
        public string PrintName { get; set; }
        public double? PrintPrice { get; set; }
        public string PrintUnit { get; set; }
        public string TypePrintId { get; set; }

        public virtual TypePrint TypePrint { get; set; }
        public virtual ICollection<OrderDetailPrint> OrderDetailPrint { get; set; }
    }
}
