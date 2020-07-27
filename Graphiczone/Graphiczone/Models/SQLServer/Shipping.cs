using System;
using System.Collections.Generic;

namespace Graphiczone.Models.SQLServer
{
    public partial class Shipping
    {
        public string OrPrintId { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string ShippingFile { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; set; }
        public virtual OrderPrint OrderPrint { get; set; }
    }
}
