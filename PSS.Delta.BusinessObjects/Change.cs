using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    /// <summary>
    /// Represents a general notion of a change.
    /// </summary>
    public abstract class Change
    {
        public string ChangedBy { get; set; }
        public DateTime ChangeDate { get; set; }
        public string Description { get; set; }
    }
}
