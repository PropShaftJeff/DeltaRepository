using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using PSS.Delta;
using PSS.Delta.BusinessObjects.Mapping;

namespace PSS.Delta.BusinessObjects.Controllers
{
    internal class EcnManager
    {
        EcnDatabaseDataContext _context;
        EcnHeaderMapper _ecnHeaderMapper;
        ChecklistMapper _checklistMapper;
        InventoryMapper _inventoryMapper;
        WorkOrderMapper _workOrderMapper;

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

            // note: this may not be the correct way to load Operations and Materials
            dlo.LoadWith<EcnOpenWorkOrder>(e => e.EcnWorkOrderOperations);
            dlo.LoadWith<EcnWorkOrderOperation>(e => e.EcnWorkOrderMaterials);
            dlo.LoadWith<EcnPurchaseOrder>(e => e.EcnPurchaseOrderLines);
            dlo.LoadWith<EcnSalesOrder>(e => e.EcnSalesOrderLines);
            dlo.LoadWith<EcnEngineeringMaster>(e => e.EcnEngineeringMasterOperations);
            dlo.LoadWith<EcnEngineeringMasterOperation>(e => e.EcnEngineeringMasterMaterials);
            
            // this can only be set once, applies for lifetime of _context.
            _context.LoadOptions = dlo;

            // create the mapping objects, to convert from POCO to DAL classes.
            _ecnHeaderMapper = new EcnHeaderMapper();
            _checklistMapper = new ChecklistMapper();
            _inventoryMapper = new InventoryMapper();
            _workOrderMapper = new WorkOrderMapper();
        }

        /// <summary>
        /// The purpose of this method is to load an existing engineering change notice from a given 
        /// ECN number.  
        /// </summary>
        /// <param name="ecn"></param>
        internal void Load(EngineeringChangeNotice ecn)
        {
            try
            {
                // becuse of DataLoadOptions used, all tables get loaded.
                EcnHeader header = _context.EcnHeaders.Where(e => e.EcnNumber == ecn.EcnId).FirstOrDefault();

                if (header == null || ecn == null)
                {
                    // todo, should I throw an exception here??
                    return;
                }
                else
                {
                    _ecnHeaderMapper.MapDbToPoco(header, ecn);
                    _checklistMapper.MapDbToPoco(header, ecn);
                    MapInventoryInfo(header, ecn);
                    
                    MapWorkOrders(header, ecn);
                    MapPurchaseOrders(header, ecn);
                    MapSalesOrders(header, ecn);
                    MapEngineeringMasters(header, ecn);
                }
            }
            catch (Exception)
            {                
                throw;
            }
        }

        /* Prerequsites
         *  1. Parameters "header" and "ecn" are not null
         *  2. 
         *  
         */
        private void MapInventoryInfo(EcnHeader header, EngineeringChangeNotice ecn)
        {
            // Get the current state of inventory at the time of this Ecn
            List<EcnInventoryOnHand> dbInventory = _context.EcnInventoryOnHands
                .Where(inv => inv.EcnId == ecn.EcnId)
                .ToList();

            foreach (EcnInventoryOnHand dbInventorItem in dbInventory)
            {
                InventoryItem inventoryItem = new InventoryItem();
                _inventoryMapper.MapDbToPoco(dbInventorItem, inventoryItem);

                // part number is not a field in dbInventoryItem, so need to get it somewhere else.
                inventoryItem.PartId = header.PartNumber;

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
                /* Load the work order how it appeared when it was last saved.
                 * This is not the time to get the state of the work order before
                 * the changes of the ECN were applied.
                 */
                WorkOrder wo = new WorkOrder(dbWorkOrder.WorkId);
                _workOrderMapper.MapDbToPoco(dbWorkOrder, wo);                               

                // add mapped work order to engineering change notice
                ecn.WorkOrders.Add(wo);

                // load the work order operations for the work order
                foreach (EcnWorkOrderOperation dbWorkOrderOp in dbWorkOrder.EcnWorkOrderOperations)
                {
                    // create a work order operation and map the database object to it.
                    Operation workOrderOp = new Operation(dbWorkOrderOp.Id.ToString());
                    WorkOrderMaterialMapper opMapper = new WorkOrderMaterialMapper();
                    opMapper.MapDbToPoco(dbWorkOrderOp, workOrderOp);

                    // add mapped operation to mapped work order
                    wo.Operations.Add(workOrderOp);

                    // load the materials for this work order operation
                    foreach (EcnWorkOrderMaterial dbWorkOrderMaterial in dbWorkOrderOp.EcnWorkOrderMaterials)
                    {
                        /* Load the operation material how it appeared when it was last saved.
                        * This is not the time to get the state of the work order operation before
                        * the changes of the ECN were applied.
                        */
                        Material mappedMaterial = new Material(dbWorkOrderMaterial.Id.ToString());

                        mappedMaterial.Description = dbWorkOrderMaterial.DescriptionAfter;
                        mappedMaterial.DrawingFile = dbWorkOrderMaterial.DrawingFileAfter;
                        mappedMaterial.DrawingId = dbWorkOrderMaterial.DrawingIdAfter;
                        mappedMaterial.Notes = dbWorkOrderMaterial.NotesAfter;
                        mappedMaterial.NumberOfPages = dbWorkOrderMaterial.NumberOfPagesAfter;
                        mappedMaterial.OperationSequence = dbWorkOrderMaterial.SeqNoAfter;
                        mappedMaterial.PartNumber = dbWorkOrderMaterial.PartNumberAfter;
                        mappedMaterial.PieceNumber = dbWorkOrderMaterial.PieceNoAfter;
                        mappedMaterial.QtyPer = dbWorkOrderMaterial.QtyPerAfter;
                        mappedMaterial.Revision = dbWorkOrderMaterial.RevisionAfter;
                        mappedMaterial.Specifications = dbWorkOrderMaterial.SpecificationsAfter;
                        mappedMaterial.UnitOfMeasure = dbWorkOrderMaterial.UomAfter ;

                        // add mapped material to mapped operation
                        workOrderOp.Materials.Add(mappedMaterial);
                    }
                }
            }
        }

        private void MapPurchaseOrders(EcnHeader header, EngineeringChangeNotice ecn)
        {
            List<EcnPurchaseOrder> dbPurchaseOrders = _context.EcnPurchaseOrders
                .Where(wo => wo.EcnId == ecn.EcnId)
                .ToList();

            foreach (EcnPurchaseOrder dbPo in dbPurchaseOrders)
            {
                // todo update EcnPurchaseOrder table in Engineering database, with 
                // before and after info, like work order.
                PurchaseOrder po = new PurchaseOrder(dbPo.Id.ToString());
                po.PurchaseId = dbPo.PurchaseId;
                po.OrderDate = dbPo.OrderDate;
                po.Supplier = dbPo.SupplierId;
                po.OrderStatus = dbPo.OrderStatus;

                //add to the ECN
                ecn.PurchaseOrders.Add(po);

                // get all the saved purchase order lines.
                foreach (EcnPurchaseOrderLine dbPoLine in dbPo.EcnPurchaseOrderLines)
                {
                    PoLine pol = new PoLine(dbPoLine.Id.ToString());
                    pol.Description = dbPoLine.DescriptionAfter;
                    pol.DrawingId = dbPoLine.DrawingIdAfter;
                    pol.LineStatus = dbPoLine.LineStatus;
                    pol.OrderQty = dbPoLine.OrderQuantity;
                    pol.ReceivedQty = dbPoLine.ReceivedQuantity;
                    pol.Revision = dbPoLine.DrawingRevAfter;
                    pol.ServiceId = dbPoLine.ServiceId;
                    pol.Specifications = dbPoLine.SpecificationsAfter;

                    // add to the purchase order
                    po.LineItems.Add(pol);
                }
            }
        }

        private void MapSalesOrders(EcnHeader header, EngineeringChangeNotice ecn)
        {
            List<EcnSalesOrder> dbSalesOrders = _context.EcnSalesOrders
                .Where(so => so.EcnId == ecn.EcnId)
                .ToList();

            foreach (EcnSalesOrder dbSalesOrder in dbSalesOrders)
            {
                // map the sales order
                SalesOrder so = new SalesOrder(dbSalesOrder.Id.ToString());
                so.Customer = dbSalesOrder.CustomerId;
                so.OrderDate = dbSalesOrder.OrderDate;
                so.SalesId = dbSalesOrder.SalesId;

                // add it to the ecn
                ecn.SalesOrders.Add(so);

                foreach (EcnSalesOrderLine dbSoLine in dbSalesOrder.EcnSalesOrderLines)
                {
                    // map the sales order line
                    SoLine soLine = new SoLine(dbSoLine.Id.ToString());
                    soLine.Description = dbSoLine.DescriptionAfter;
                    soLine.DrawingId = dbSoLine.DrawingIdAfter;
                    soLine.LineNo = dbSoLine.LineNo;
                    soLine.LineStatus = dbSoLine.LineStatus;
                    soLine.OrderQty = dbSoLine.OrderQty;
                    soLine.Revision = dbSoLine.DrawingRevAfter;
                    soLine.ShippedQty = dbSoLine.ShippedQty;

                    // add it to the mapped sales order
                    so.LineItems.Add(soLine);
                }
            }
        }

        private void MapEngineeringMasters(EcnHeader header, EngineeringChangeNotice ecn)
        {
            List<EcnEngineeringMaster> dbEngineeringMasters = _context.EcnEngineeringMasters
               .Where(m => m.EcnId == ecn.EcnId)
               .ToList();

            foreach (EcnEngineeringMaster dbEngMaster in dbEngineeringMasters)
            {
                /* Load the work order how it appeared when it was last saved.
                 * This is not the time to get the state of the engineering master before
                 * the changes of the ECN were applied.
                 */
                EngineeringMaster engMaster = new EngineeringMaster(dbEngMaster.Id.ToString());
                engMaster.Description = dbEngMaster.EndingDescription;
                engMaster.DrawingId = dbEngMaster.EndingDrawingId;
                engMaster.EngId = dbEngMaster.EngId;
                engMaster.GlobalRank = dbEngMaster.EndingGlobalRank;
                engMaster.Notes = dbEngMaster.EndingNotes;
                engMaster.NumberOfPages = dbEngMaster.EndingNumPages;
                engMaster.Revision = dbEngMaster.EndingRevision;
                engMaster.Specifications = dbEngMaster.EndingSpecifications;

                // add mapped engineering master to engineering change notice
                ecn.EngineeringMasters.Add(engMaster);

                // load the work order operations for the work order
                foreach (EcnEngineeringMasterOperation dbEngMasterOp in dbEngMaster.EcnEngineeringMasterOperations)
                {
                    /* Load the engineering master operation how it appeared when it was last saved.
                     * This is not the time to get the state of the engineering master operation before
                     * the changes of the ECN were applied.
                     */
                    Operation engMasterOp = new Operation(dbEngMasterOp.Id.ToString());
                    engMasterOp.Description = dbEngMasterOp.DescriptionAfter;
                    engMasterOp.DrawingFile = dbEngMasterOp.DrawingFileAfter;
                    engMasterOp.DrawingId = dbEngMasterOp.DrawingIdAfter;
                    engMasterOp.NumberOfPages = dbEngMasterOp.NumberOfPagesAfter;
                    engMasterOp.ResourceId = dbEngMasterOp.ResourceIdAfter;
                    engMasterOp.Revision = dbEngMasterOp.RevisionAfter;
                    engMasterOp.RunPerHour = dbEngMasterOp.RunPerHourAfter;
                    engMasterOp.SequenceNumber = dbEngMasterOp.SeqNoAfter;
                    engMasterOp.ServiceId = dbEngMasterOp.ServiceIdAfter;
                    engMasterOp.SetupHours = dbEngMasterOp.SetupHoursAfter;
                    engMasterOp.SupplierId = dbEngMasterOp.SupplierIdAfter;
                    engMasterOp.TemplateId = dbEngMasterOp.TemplateIdAfter;

                    // add mapped operation to mapped engineering master
                    engMaster.Operations.Add(engMasterOp);

                    // load the materials for this work order operation
                    foreach (EcnEngineeringMasterMaterial dbEngMasterMaterial in dbEngMasterOp.EcnEngineeringMasterMaterials)
                    {
                        /* Load the operation material how it appeared when it was last saved.
                        * This is not the time to get the state of the work order operation before
                        * the changes of the ECN were applied.
                        */
                        Material engMasterMat = new Material(dbEngMasterMaterial.Id.ToString());
                        engMasterMat.Description = dbEngMasterMaterial.DescriptionAfter;
                        engMasterMat.DrawingFile = dbEngMasterMaterial.DrawingFileAfter;
                        engMasterMat.DrawingId = dbEngMasterMaterial.DrawingIdAfter;
                        engMasterMat.Notes = dbEngMasterMaterial.NotesAfter;
                        engMasterMat.NumberOfPages = dbEngMasterMaterial.NumberOfPagesAfter;
                        engMasterMat.OperationSequence = dbEngMasterMaterial.SeqNoAfter;
                        engMasterMat.PartNumber = dbEngMasterMaterial.PartNumberAfter;
                        engMasterMat.PieceNumber = dbEngMasterMaterial.PieceNoAfter;
                        engMasterMat.QtyPer = dbEngMasterMaterial.QtyPerAfter;
                        engMasterMat.Revision = dbEngMasterMaterial.RevisionAfter;
                        engMasterMat.Specifications = dbEngMasterMaterial.SpecificationsAfter;
                        engMasterMat.UnitOfMeasure = dbEngMasterMaterial.UomAfter;

                        // add mapped material to mapped operation
                        engMasterOp.Materials.Add(engMasterMat);
                    }
                }
            }
        }

        internal static void Save(EngineeringChangeNotice ecn)
        {
            // map the POCO back to DAL and save
            throw new NotImplementedException();
        }
    }
}
