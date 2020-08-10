using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Graphiczone.Models.SQLServer
{
    public partial class Shipping
    {
        public int Id { get; set; }
        public string OrPrintId { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string ShippingFile { get; set; }
        public string UserId { get; set; }

        public virtual OrderPrint OrPrint { get; set; }
        public virtual User User { get; set; }

    }
}
