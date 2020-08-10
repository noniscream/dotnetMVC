using Graphiczone.Models.SQLServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Graphiczone.Models
{
    public class MultipleModel
    {
        public List<User> users { get; set; }
        public List<Customer> customers { get; set; }
        public List<TypePrint> typePrints { get; set; }
        public List<Print> prints { get; set; }
        public List<OrderPrint> orderPrints { get; set; }
        public List<OrderDetailPrint> orderDetailPrints { get; set; }
        public List<ProofPayment> proofPayments { get; set; }
        public List<Shipping> shippings { get; set; }
    }
}
