using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class PartInfo
    {
        public string PartId { get; set; }
        public string Description { get; set; }
        public string DrawingFile { get; set; }
        public string Revision { get; set; }
        public string Notes { get; set; }
        public string Specifications { get; set; }        
    }
}
