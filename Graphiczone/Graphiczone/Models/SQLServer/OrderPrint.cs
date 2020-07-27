using System;
using System.Collections.Generic;

namespace Graphiczone.Models.SQLServer
{
    public partial class OrderPrint
    {
        public OrderPrint()
        {
            OrderDetailPrint = new HashSet<OrderDetailPrint>();
        }

        public string OrPrintId { get; set; }
        public DateTime? OrPrintDate { get; set; }
        public DateTime? OrPrintDue { get; set; }
        public int? OrPrintShipping { get; set; }
        public string OrPrintLocalshipping { get; set; }
        public double? OrPrintTotal { get; set; }
        public int? OrPrintStatus { get; set; }
        public string CusId { get; set; }

        public virtual Customer Cus { get; set; }
        public virtual ProofPayment OrPrint { get; set; }
        public virtual Shipping OrPrintNavigation { get; set; }
        public virtual ICollection<OrderDetailPrint> OrderDetailPrint { get; set; }
    }
}
