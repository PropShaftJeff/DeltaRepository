using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class PoLine
    {
        public decimal? OrderQty { get; set; }
        public decimal? ReceivedQty { get; set; }
        public string Description { get; set; }
        public string DrawingId { get; set; }
        public string Revision { get; set; }
        public string LineStatus { get; set; }

        public override string ToString()
        {
            return string.Format("Ordered: {0:n0} Received: {1:n0} Status: {2}",
                OrderQty, ReceivedQty, LineStatus);
        }
    }
}
