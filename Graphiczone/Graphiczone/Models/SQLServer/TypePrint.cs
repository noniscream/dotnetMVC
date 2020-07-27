using System;
using System.Collections.Generic;

namespace Graphiczone.Models.SQLServer
{
    public partial class TypePrint
    {
        public TypePrint()
        {
            Print = new HashSet<Print>();
        }

        public string TypePrintId { get; set; }
        public string TypePrintName { get; set; }

        public virtual ICollection<Print> Print { get; set; }
    }
}
