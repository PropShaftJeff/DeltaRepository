using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class EngineeringChangeNotice
    {
        public EngineeringChangeNotice(string partId)
        {
            PartNumber = partId;

            //instantiate all the lists.
            EngineeringMasters = new List<EngineeringMaster>();
            PurchaseOrders = new List<PurchaseOrder>();
            SalesOrders = new List<SalesOrder>();
            WorkOrders = new List<WorkOrder>();
            Checklist = new List<ItemToCheck>();
            Inventory = new List<InventoryItem>();
        }

        public int Id { get; set; }
        public int EcnId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string CustomerPartNumber { get; set; }
        public string Customer { get; set; }
        public string PssRevision { get; set; }
        public string CustomerRevision { get; set; }
        public string Status { get; set; }

        public List<EngineeringMaster> EngineeringMasters { get; set; }
        public List<PurchaseOrder> PurchaseOrders { get; set; }
        public List<SalesOrder> SalesOrders { get; set; }
        public List<WorkOrder> WorkOrders { get; set; }
        public List<ItemToCheck> Checklist { get; set; }
        public List<InventoryItem> Inventory { get; set; }
    }
}
