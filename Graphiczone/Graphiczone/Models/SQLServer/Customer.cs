using System;
using System.Collections.Generic;

namespace Graphiczone.Models.SQLServer
{
    public partial class Customer
    {
        public Customer()
        {
            OrderPrint = new HashSet<OrderPrint>();
        }

        public string CusId { get; set; }
        public string CusFirstname { get; set; }
        public string CusLastname { get; set; }
        public string CusUsername { get; set; }
        public string CusPassword { get; set; }
        public string CusAddress { get; set; }
        public string CusTel { get; set; }
        public string CusEmail { get; set; }

        public virtual ICollection<OrderPrint> OrderPrint { get; set; }
    }
}
