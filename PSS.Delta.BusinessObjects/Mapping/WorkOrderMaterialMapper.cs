using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects.Mapping
{
    internal class WorkOrderMaterialMapper : IMapper
    {
        #region IMapper Members

        public void MapDbToPoco(object db, object poco)
        {
            Material mat = poco as Material;
            EcnWorkOrderMaterial dbMat = db as EcnWorkOrderMaterial;

            if (mat == null || dbMat == null) 
            {
                return; 
            }

            /* Load the work order operation how it appeared when it was last saved.
             * This is not the time to get the state of the work order operation before
             * the changes of the ECN were applied.
             */
            mat.Description = dbMat.DescriptionAfter;
            mat.DrawingFile = dbMat.DrawingFileAfter;
            mat.DrawingId = dbMat.DrawingIdAfter;
            mat.NumberOfPages = dbMat.NumberOfPagesAfter;
            mat.Id = dbMat.Id;
            mat.Revision = dbMat.RevisionAfter;

              
        }

        public void MapPocoToDb(object poco, object db)
        {
            Material mat = poco as Material;
            EcnWorkOrderMaterial dbMat = db as EcnWorkOrderMaterial;

            if (mat == null || dbMat == null)
            {
                return;
            }

            
        }

        #endregion
    }
}
