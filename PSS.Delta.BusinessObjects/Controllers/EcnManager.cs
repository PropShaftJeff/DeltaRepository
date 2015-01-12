using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using PSS.Delta;

namespace PSS.Delta.BusinessObjects.Controllers
{
    internal class EcnManager
    {
        EcnDatabaseDataContext _context;

        internal EcnManager()
        {
            _context = new EcnDatabaseDataContext();

            // load everything.
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<EcnHeader>(e => e.EcnSalesOrders);
            dlo.LoadWith<EcnHeader>(e => e.EcnPurchaseOrders);
            dlo.LoadWith<EcnHeader>(e => e.EcnOpenWorkOrders);
            dlo.LoadWith<EcnHeader>(e => e.EcnInventoryOnHands);
            dlo.LoadWith<EcnHeader>(e => e.EcnChecks);

            // this can only be set once, applies for lifetime of _context.
            _context.LoadOptions = dlo;
        }

        internal void Load(EngineeringChangeNotice ecn)
        {
            try
            {
                // becuse of DataLoadOptions used, all tables get loaded.
                EcnHeader header = _context.EcnHeaders.Where(e => e.EcnNumber == ecn.EcnId).FirstOrDefault();

                // consider mapping as going from database classes to domain classes.
                MapHeader(header, ecn);
                MapChecklist(header, ecn);
                MapInventoryInfo(header, ecn);
                MapWorkOrders(header, ecn);
                MapPurchaseOrders(header, ecn);
                MapSalesOrders(header, ecn);
            }
            catch (Exception)
            {                
                throw;
            }
        }
       
        private void MapHeader(EcnHeader header, EngineeringChangeNotice ecn)
        {
            if (header == null) 
            {
                return;
            }

            //map header info.
            ecn.CreatedBy = header.CreatedBy;
            ecn.Customer = header.Customer;
            ecn.CustomerPartNumber = header.CustomerPartNumber;
            ecn.CustomerRevision = header.CustomerRevision;
            ecn.DateCreated = header.DateCreated;
            ecn.Description = header.PartDescription;
            ecn.Id = header.Id;
            ecn.LastModified = header.LastModified;
            ecn.LastModifiedBy = header.LastModifiedBy;
            ecn.PartNumber = header.PartNumber;
            ecn.PssRevision = header.PssRevision;
            ecn.Status = header.Status;
        }

        private void MapChecklist(EcnHeader header, EngineeringChangeNotice ecn)
        {
            if (header == null)
            {
                return;
            }

            // get standard checks, so we can correlate the check id with the actual
            // text of the item that needs to get checked.
            List<EcnStandardCheck> standardChecks = _context.EcnStandardChecks.ToList();

            foreach (EcnCheck check in header.EcnChecks)
            {
                ItemToCheck ecnCheck = new ItemToCheck();

                ecnCheck.Instructions = standardChecks.Where(sc => sc.Id == check.StandardCheckId)
                    .Select(sc => sc.ItemToCheck)
                    .FirstOrDefault();

                ecnCheck.IsComplete = check.Completed;
                ecnCheck.Notes = check.Notes;

                // add the check to the ecn
                ecn.Checklist.Add(ecnCheck);
            }
        }

        private void MapInventoryInfo(EcnHeader header, EngineeringChangeNotice ecn)
        {
            if (header == null)
            {
                return;
            }

            List<EcnInventoryOnHand> dbInventory = _context.EcnInventoryOnHands
                .Where(inv => inv.EcnId == ecn.EcnId)
                .ToList();

            foreach (EcnInventoryOnHand dbInventorItem in dbInventory)
            {
                InventoryItem inventoryItem = new InventoryItem();
                inventoryItem.LocationId = dbInventorItem.LocationId;
                inventoryItem.Notes = dbInventorItem.Notes;
                inventoryItem.PartId = dbInventorItem.PartId;
                inventoryItem.QtyOnHand = dbInventorItem.QtyOnHand;
                inventoryItem.Reworkable = dbInventorItem.Reworkable;

                // add inventory item to ecn
                ecn.Inventory.Add(inventoryItem);
            }
        }

        private void MapWorkOrders(EcnHeader header, EngineeringChangeNotice ecn)
        {
            List<EcnOpenWorkOrder> dbWorkOrders = _context.EcnOpenWorkOrders
                .Where(wo => wo.EcnId == ecn.EcnId)
                .ToList();

            foreach (EcnOpenWorkOrder dbWorkOrder in dbWorkOrders)
            {
                WorkOrder wo = new WorkOrder(dbWorkOrder.WorkId);
                wo.Description = dbWorkOrder.Description;
                wo.DrawingId = dbWorkOrder.DrawingId;
                wo.GlobalRank = dbWorkOrder.GlobalRank;
                wo.Notes = dbWorkOrder.Notes;
                wo.NumberOfPages = dbWorkOrder.NumberOfPages;
                wo.Revision = dbWorkOrder.Revision;
                wo.Specifications = dbWorkOrder.Specifications;
                wo.WorkId = dbWorkOrder.WorkId;
            }
        }

        private void MapPurchaseOrders(EcnHeader header, EngineeringChangeNotice ecn)
        {
            throw new NotImplementedException();
        }

        private void MapSalesOrders(EcnHeader header, EngineeringChangeNotice ecn)
        {
            throw new NotImplementedException();
        }
    }
}
