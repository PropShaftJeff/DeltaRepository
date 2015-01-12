using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class SoLine
    {
        public decimal? OrderQty { get; set; }
        public decimal? ShippedQty { get; set; }
        public string Description { get; set; }
        public string DrawingId { get; set; }
        public string Revision { get; set; }
        public string LineStatus { get; set; }
        public int LineNo { get; set; }

        public override string ToString()
        {
            return string.Format("Line: {0} Ordered: {1:n0} Shipped: {2:n0}",
                LineNo, OrderQty, ShippedQty);
        }
    }
}
