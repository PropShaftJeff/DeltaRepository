using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects.Mapping
{
    internal class WorkOrderOperationMapper : IMapper
    {
        #region IMapper Members

        public void MapDbToPoco(object db, object poco)
        {
            Operation op = poco as Operation;
            EcnWorkOrderOperation dbOp = db as EcnWorkOrderOperation;

            if (op == null || dbOp == null) 
            {
                return; 
            }

            /* Load the work order operation how it appeared when it was last saved.
             * This is not the time to get the state of the work order operation before
             * the changes of the ECN were applied.
             */
            op.Description = dbOp.DescriptionAfter;
            op.DrawingFile = dbOp.DrawingFileAfter;
            op.DrawingId = dbOp.DrawingIdAfter;
            op.NumberOfPages = dbOp.NumberOfPagesAfter;
            op.ResourceId = dbOp.ResourceIdAfter;
            op.Revision = dbOp.RevisionAfter;
            op.RunPerHour = dbOp.RunPerHourAfter;
            op.SequenceNumber = dbOp.SeqNoAfter;
            op.ServiceId = dbOp.ServiceIdAfter;
            op.SetupHours = dbOp.SetupHoursAfter;
            op.SupplierId = dbOp.SupplierIdAfter;
            op.TemplateId = dbOp.TemplateIdAfter;                  
        }

        public void MapPocoToDb(object poco, object db)
        {
            Operation op = poco as Operation;
            EcnWorkOrderOperation dbOp = db as EcnWorkOrderOperation;

            dbOp.DescriptionAfter = op.Description;
            dbOp.DrawingFileAfter = op.DrawingFile;
            dbOp.DrawingIdAfter = op.DrawingId;
            dbOp.NumberOfPagesAfter = op.NumberOfPages;
            dbOp.ResourceIdAfter = op.ResourceId;
            dbOp.RevisionAfter = op.Revision;
            dbOp.RunPerHourAfter = op.RunPerHour;
            dbOp.SeqNoAfter = op.SequenceNumber;
            dbOp.ServiceIdAfter = op.ServiceId;
            dbOp.SetupHoursAfter = op.SetupHours;
            dbOp.SupplierIdAfter = op.SupplierId;
            dbOp.TemplateIdAfter = op.TemplateId; 
        }

        #endregion
    }
}
