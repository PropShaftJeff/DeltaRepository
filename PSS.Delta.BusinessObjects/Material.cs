using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class Material
    {
        public Material(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Ties the POCO back to the DAL
        /// </summary>
        public string Id { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public decimal? QtyPer { get; set; }
        public string UnitOfMeasure { get; set; }
        public int? OperationSequence { get; set; }
        public int? PieceNumber { get; set; }
        public string Specifications { get; set; }
        public string Notes { get; set; }
        public string DrawingId { get; set; }
        public string Revision { get; set; }
        public short? NumberOfPages { get; set; }
        public string DrawingFile { get; set; }

        public override string ToString()
        {
            return string.Format("Part Id: {0} - {1}", PartNumber, Description);
        }
    }
}
