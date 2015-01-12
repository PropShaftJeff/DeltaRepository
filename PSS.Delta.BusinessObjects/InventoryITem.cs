using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    /// <summary>
    /// Represents a snapshot of an inventory location that was checked for 
    /// rework during the course of the engineering change process.
    /// </summary>
    public class InventoryItem
    {
        public string PartId { get; set; }
        public string LocationId { get; set; }
        public decimal? QtyOnHand { get; set; }
        public string Notes { get; set; }
        public bool? Reworkable { get; set; }

        public override string ToString()
        {
            return string.Format("Location: {0} OnHand: {1:n2}", LocationId, QtyOnHand);
        }
    }
}
