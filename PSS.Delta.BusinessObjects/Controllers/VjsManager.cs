using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSS.Delta.BusinessObjects.Communication;

namespace PSS.Delta.BusinessObjects.Controllers
{
    /// <summary>
    /// The purpose of this class is to manage the extraction of data from VJS.
    /// </summary>
    internal class VjsManager
    {
        private VjsDatabaseDataContext _context;
        internal VjsManager()
        {
            //_context = new VjsDatabaseDataContext("PSS.Delta.Properties.Settings.vjs2013ConnectionString");
            _context = new VjsDatabaseDataContext();
        }

        internal void GetDataFor(EngineeringChangeNotice ecn)
        {
            try
            {
                if (ecn.PartNumber == null)
                {
                    throw new Exception("No data can be gathered from VJS for this ECN without a part number.");
                }

                //get vjs data
                LoadPartInfo(ecn);
                LoadReleasedWorkOrders(ecn);
                LoadAllPurchaseOrders(ecn);
                LoadOpenSalesOrders(ecn);
                LoadAllEngineeringMasters(ecn);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }
        }

        // Loads part information from VJS into the ecn.
        private void LoadPartInfo(EngineeringChangeNotice ecn)
        {
            INVENTORY_PART vjsPart = _context.INVENTORY_PARTs.Where(p => p.ID == ecn.PartNumber).FirstOrDefault();
            if (vjsPart != null)
            { 
                ecn.PssRevision = vjsPart.DRAWING_REV_NO;
                ecn.Description = vjsPart.DESCRIPTION;
            }
        }

        // Loads work orders from VJS into the ecn.
        private void LoadReleasedWorkOrders(EngineeringChangeNotice ecn)
        {
            // get released work orders for the part number in the ecn.
            List<SHOPFLOOR_WORK_ORDER> vjsWorkOrder = 
                _context.SHOPFLOOR_WORK_ORDERs
                .Where(wo => wo.PART_ID == ecn.PartNumber && wo.ORDER_STATUS == "RELEASED")
                .ToList();

            //get materials and operations for each work order
            foreach (SHOPFLOOR_WORK_ORDER dbWorkOrder in vjsWorkOrder)
            {
                string workId = dbWorkOrder.WORK_ID;

                //get all operations for the particular work order.
                IQueryable<SHOPFLOOR_OPERATION> operations = 
                    _context.SHOPFLOOR_OPERATIONs
                    .Where(op => op.WORK_ID == workId);
               
                // get all materials for the particular work order.
                List<SHOPFLOOR_MATERIAL> materials = _context.SHOPFLOOR_MATERIALs.Where(mat => mat.WORK_ID == workId).ToList();

                //create the work order object.
                WorkOrder workOrder = new WorkOrder(dbWorkOrder.RECORD_IDENTITY);
                workOrder.WorkId = dbWorkOrder.WORK_ID;
                workOrder.Description = dbWorkOrder.DESCRIPTION;
                workOrder.DrawingId = dbWorkOrder.DRAWING_ID;
                workOrder.GlobalRank = dbWorkOrder.GLOBAL_RANK;
                workOrder.Notes = dbWorkOrder.NOTES;
                workOrder.NumberOfPages = dbWorkOrder.DRAWING_PAGE_NO;
                workOrder.Revision = dbWorkOrder.DRAWING_REV_NO;
                workOrder.Specifications = dbWorkOrder.SPECIFICATIONS;

                foreach (SHOPFLOOR_OPERATION op in operations)
                {
                    //create operations
                    Operation woOperation = new Operation(op.RECORD_IDENTITY);
                    woOperation.Description = op.DESCRIPTION;
                    woOperation.DrawingFile = op.DRAWING_FILE;
                    woOperation.DrawingId = op.DRAWING_ID;
                    woOperation.NumberOfPages = op.DRAWING_PAGE_NO;
                    woOperation.ResourceId = op.RESOURCE_ID;
                    woOperation.Revision = op.DRAWING_REV_NO;
                    woOperation.RunPerHour = op.RUN_TYPE == "PCS_HR" ? op.RUN_EXPR : 0M;
                    woOperation.SequenceNumber = op.SEQ_NO;
                    woOperation.ServiceId = op.SERVICE_ID;
                    woOperation.SetupHours = op.SETUP_HRS;
                    woOperation.SupplierId = op.SUPPLIER_ID;
                    woOperation.TemplateId = op.OPER_TEMPLATE_ID;

                    // look for materials for the operation
                    List<SHOPFLOOR_MATERIAL> relaventMaterials = materials.Where(m => m.SEQ_NO == op.SEQ_NO).ToList();

                    foreach (SHOPFLOOR_MATERIAL mat in relaventMaterials)
                    {
                        // create materials
                        Material woMaterial = new Material(mat.RECORD_IDENTITY);
                        woMaterial.Description = mat.DESCRIPTION;
                        woMaterial.DrawingFile = mat.DRAWING_FILE;
                        woMaterial.DrawingId = mat.DRAWING_ID;
                        woMaterial.Notes = mat.NOTES;
                        woMaterial.NumberOfPages = mat.DRAWING_PAGE_NO;
                        woMaterial.OperationSequence = mat.SEQ_NO;
                        woMaterial.PartNumber = mat.SUBORD_PART_ID;
                        woMaterial.PieceNumber = mat.PIECE_NO;
                        woMaterial.QtyPer = mat.QTY_PER;
                        woMaterial.Revision = mat.DRAWING_REV_NO;
                        woMaterial.Specifications = mat.SPECIFICATIONS;
                        woMaterial.UnitOfMeasure = mat.USAGE_UM;

                        //add material to operation
                        woOperation.Materials.Add(woMaterial);
                    }

                    //add operation to work order.
                    workOrder.Operations.Add(woOperation);
                }
            }
        }

        // Loads purchase orders from VJS into the ecn.
        private void LoadAllPurchaseOrders(EngineeringChangeNotice ecn)
        {
            // look for PO lines.
            List<PURCHASING_ORDER_LINE> vjsPoLines = _context.PURCHASING_ORDER_LINEs
                .Where(pol => pol.PART_ID == ecn.PartNumber).ToList();

            // get the purchase id from each line item, so the PO can be found.
            string[] purchaseIds = vjsPoLines.Select(po => po.PURCHASE_ID).Distinct().ToArray();

            // look for PO's
            List<PURCHASING_PURCHASE_ORDER> vjsPurchaseOrders = 
                _context.PURCHASING_PURCHASE_ORDERs
                .Where(po => purchaseIds.Contains(po.ID)).ToList();

            foreach (PURCHASING_PURCHASE_ORDER vjsPO in vjsPurchaseOrders)
            {
                //create po
                PurchaseOrder po = new PurchaseOrder(vjsPO.ID);
                po.OrderDate = vjsPO.ORDER_DATE;
                po.Supplier = vjsPO.SUPPLIER_ID;
                
                // find relavent PO lines
                List<PURCHASING_ORDER_LINE> relaventLineItems = 
                    vjsPoLines.Where(pol => pol.PURCHASE_ID == vjsPO.ID)
                    .ToList();

                foreach (PURCHASING_ORDER_LINE vjsPoLine in relaventLineItems)
                {
                    PoLine lineItem = new PoLine(vjsPoLine.RECORD_IDENTITY);
                    lineItem.Description = vjsPoLine.DESCRIPTION;
                    lineItem.DrawingId = vjsPoLine.DRAWING_ID;
                    lineItem.LineStatus = vjsPoLine.LINE_STATUS;
                    lineItem.OrderQty = vjsPoLine.ORDER_QTY;
                    lineItem.ReceivedQty = vjsPoLine.RECEIVED_QTY;
                    lineItem.Revision = vjsPoLine.DRAWING_REV_NO;

                    //add line item to po.
                    po.LineItems.Add(lineItem);
                }

                //add purchase order to ecn
                ecn.PurchaseOrders.Add(po);
            }
        }

        // Loads open sales orders from VJS into the ecn.
        private void LoadOpenSalesOrders(EngineeringChangeNotice ecn)
        {
            // used for null checking.
            List<SALES_ORDER_LINE> vjsSoLines = 
                _context.SALES_ORDER_LINEs
                .Where(sol => sol.PART_ID == ecn.PartNumber && sol.LINE_STATUS == "OPEN")
                .ToList();

            string[] salesIds = vjsSoLines.Select(so => so.SALES_ID).Distinct().ToArray();

            List<SALES_SALES_ORDER> vjsSalesOrders =
                _context.SALES_SALES_ORDERs
                .Where(so => salesIds.Contains(so.ID))
                .ToList();

            foreach (SALES_SALES_ORDER vjsSalesOrder in vjsSalesOrders)
            {
                // create sales order.
                SalesOrder so = new SalesOrder(vjsSalesOrder.RECORD_IDENTITY);
                so.Customer = vjsSalesOrder.CUSTOMER_ID;
                so.OrderDate = vjsSalesOrder.ORDER_DATE;

                // look for sales order line items that matter.
                List<SALES_ORDER_LINE> relaventLineItems = 
                    vjsSoLines.Where(sol => sol.SALES_ID == vjsSalesOrder.ID).ToList();

                foreach (SALES_ORDER_LINE vjsLineItem in relaventLineItems)
                {
                    SoLine lineItem = new SoLine(vjsLineItem.RECORD_IDENTITY);
                    lineItem.LineNo = vjsLineItem.LINE_NO;
                    lineItem.Description = vjsLineItem.DESCRIPTION;
                    lineItem.DrawingId = vjsLineItem.DRAWING_ID;
                    lineItem.LineStatus = vjsLineItem.LINE_STATUS;
                    lineItem.OrderQty = vjsLineItem.ORDER_QTY;
                    lineItem.Revision = vjsLineItem.DRAWING_REV_NO;
                    lineItem.ShippedQty = vjsLineItem.SHIPPED_QTY;

                    // add line item to sales order.
                    so.LineItems.Add(lineItem);
                }

                // add sales order to ecn
                ecn.SalesOrders.Add(so);
            }
        }

        // Loads all engineering masters for the given part number into the ecn.
        private void LoadAllEngineeringMasters(EngineeringChangeNotice ecn)
        {
            List<ENGINEERING_MASTER> vjsMasters = _context.ENGINEERING_MASTERs
                .Where(em => em.PART_ID == ecn.PartNumber)
                .ToList();

            string[] engIds = vjsMasters.Select(m=> m.ENG_ID).Distinct().ToArray();

            List<ENGINEERING_MATERIAL> vjsMaterials = _context.ENGINEERING_MATERIALs
                .Where(mat => mat.PART_ID == ecn.PartNumber && engIds.Contains(mat.ENG_ID))
                .ToList();
            List<ENGINEERING_OPERATION> vjsOperations = _context.ENGINEERING_OPERATIONs
                .Where(mat => mat.PART_ID == ecn.PartNumber && engIds.Contains(mat.ENG_ID))
                .ToList();

            foreach (ENGINEERING_MASTER vjsMaster in vjsMasters)
            {
                // Create an engineering master.
                EngineeringMaster master = new EngineeringMaster(vjsMaster.RECORD_IDENTITY);
                master.Description = vjsMaster.DESCRIPTION;
                master.DrawingId = vjsMaster.DRAWING_ID;
                master.EngId = vjsMaster.ENG_ID;
                master.GlobalRank = vjsMaster.GLOBAL_RANK;
                master.Notes = vjsMaster.NOTES;
                master.NumberOfPages = vjsMaster.DRAWING_PAGE_NO;
                master.Revision = vjsMaster.DRAWING_REV_NO;
                master.Specifications = vjsMaster.SPECIFICATIONS;

                // add the master to the ECN here so the break will not 
                // prevent the master from being added.
                ecn.EngineeringMasters.Add(master);

                if (vjsOperations == null)
                {
                    // no operations means no materials also, a rule in VJS, 
                    // that any material must have an associated operation.
                    break;
                }

                foreach (ENGINEERING_OPERATION vjsOp in vjsOperations)
                {
                    Operation op = new Operation(vjsOp.RECORD_IDENTITY);
                    op.Description = vjsOp.DESCRIPTION;
                    op.DrawingFile = vjsOp.DRAWING_FILE;
                    op.DrawingId = vjsOp.DRAWING_ID;
                    op.NumberOfPages = vjsOp.DRAWING_PAGE_NO;
                    op.ResourceId = vjsOp.RESOURCE_ID;
                    op.Revision = vjsOp.DRAWING_REV_NO;
                    op.RunPerHour = vjsOp.RUN_EXPR;
                    op.SequenceNumber = vjsOp.SEQ_NO;
                    op.ServiceId = vjsOp.SERVICE_ID;
                    op.SetupHours = vjsOp.SETUP_HRS;
                    op.SupplierId = vjsOp.SUPPLIER_ID;
                    op.TemplateId = vjsOp.OPER_TEMPLATE_ID;

                    List<ENGINEERING_MATERIAL> relaventMaterials = 
                        vjsMaterials.Where(mat => 
                            mat.SEQ_NO == op.SequenceNumber && 
                            mat.ENG_ID == master.EngId).ToList();

                    foreach (ENGINEERING_MATERIAL vjsMaterial in relaventMaterials)
                    {
                        Material material = new Material(vjsMaterial.RECORD_IDENTITY);
                        material.Description = vjsMaterial.DESCRIPTION;
                        material.DrawingFile = vjsMaterial.DRAWING_FILE;
                        material.DrawingId = vjsMaterial.DRAWING_ID;
                        material.Notes = vjsMaterial.NOTES;
                        material.NumberOfPages = vjsMaterial.DRAWING_PAGE_NO;
                        material.OperationSequence = vjsMaterial.SEQ_NO;
                        material.PartNumber = vjsMaterial.SUBORD_PART_ID;
                        material.PieceNumber = vjsMaterial.PIECE_NO;
                        material.QtyPer = vjsMaterial.QTY_PER;
                        material.Revision = vjsMaterial.DRAWING_REV_NO;
                        material.Specifications = vjsMaterial.SPECIFICATIONS;
                        material.UnitOfMeasure = vjsMaterial.USAGE_UM;

                        //add material to operation
                        op.Materials.Add(material);
                    }

                    //add operation to master.
                    master.Operations.Add(op);
                }
                //remember, master was added to ECN already, so no need to add here.
            }      
      
        }

    }
}
