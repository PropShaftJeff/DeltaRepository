using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class PurchaseOrder
    {
        public PurchaseOrder(string purchaseId)
        {
            PurchaseId = purchaseId;
            LineItems = new List<PoLine>();
        }

        /// <summary>
        /// Ties the POCO back to the DAL
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Purchase order number
        /// </summary>
        public string PurchaseId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Supplier { get; set; }
        public string OrderStatus { get; set; }
        public List<PoLine> LineItems { get; set; }

        public override string ToString()
        {
            return string.Format("Po: {0} Supplier: {1} Ordered: {2}",
                PurchaseId, Supplier, OrderDate);
        }
    }
}
