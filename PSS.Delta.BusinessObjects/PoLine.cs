using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class PoLine
    {
        public PoLine(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Ties the POCO back to the DAL
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Quantity ordered
        /// </summary>
        public decimal? OrderQty { get; set; }
        /// <summary>
        /// Quantity received
        /// </summary>
        public decimal? ReceivedQty { get; set; }
        /// <summary>
        /// An internal description for the part on the purchase order.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The part number of the drawing used
        /// </summary>
        public string DrawingId { get; set; }
        /// <summary>
        /// The internal revision level
        /// </summary>
        public string Revision { get; set; }
        /// <summary>
        /// The status of the line item, 
        /// </summary>
        public string LineStatus { get; set; }
        /// <summary>
        /// Specifications the supplier must conform to.
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// Used to determine if this is a service or part order.
        /// </summary>
        public string ServiceId { get; set; }

        public override string ToString()
        {
            return string.Format("Ordered: {0:n0} Received: {1:n0} Status: {2}",
                OrderQty, ReceivedQty, LineStatus);
        }
    }
}
