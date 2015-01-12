using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class Operation
    {
        public Operation()
        {
            Materials = new List<Material>();
        }

        public string ResourceId { get; set; }
        public string TemplateId { get; set; }
        public string Description { get; set; }
        public string ServiceId { get; set; }
        public string SupplierId { get; set; }
        public decimal? SetupHours { get; set; }
        public decimal? RunPerHour { get; set; }
        public string DrawingId { get; set; }
        public string Revision { get; set; }
        public short? NumberOfPages { get; set; }
        public string DrawingFile { get; set; }
        public int SequenceNumber { get; set; }

        public List<Material> Materials { get; set; }

        public override string ToString()
        {
            return
                string.Format("Seq: {0} Resource: {1} - {2}", SequenceNumber, ResourceId, Description);
        }
    }
}
