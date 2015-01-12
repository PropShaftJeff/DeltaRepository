using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class SalesOrder
    {
        public SalesOrder(string id)
        {
            Id = id;
            LineItems = new List<SoLine>();
        }

        public string Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Customer { get; set; }
        public List<SoLine> LineItems { get; set; }

        public override string ToString()
        {
            return string.Format("So: {0} Customer: {1} Ordered: {2}",
                Id, Customer, OrderDate);
        }
    }
}
