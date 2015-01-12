using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class EngineeringMaster
    {
        public EngineeringMaster()
        {
            Operations = new List<Operation>();
        }

        public string EngId { get; set; }
        public string Description { get; set; }
        public string DrawingId { get; set; }
        public string Revision { get; set; }
        public short? NumberOfPages { get; set; }
        public short? GlobalRank { get; set; }
        public string Specifications { get; set; }
        public string Notes { get; set; }

        public List<Operation> Operations { get; set; }
    }
}
