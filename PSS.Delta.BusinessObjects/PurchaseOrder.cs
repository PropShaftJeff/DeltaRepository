using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class PurchaseOrder
    {
        public PurchaseOrder(string id)
        {
            Id = id;
            LineItems = new List<PoLine>();
        }

        public string Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Supplier { get; set; }
        public List<PoLine> LineItems { get; set; }

        public override string ToString()
        {
            return string.Format("Po: {0} Supplier: {1} Ordered: {2}",
                Id, Supplier, OrderDate);
        }
    }
}
