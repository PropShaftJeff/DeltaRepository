﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects
{
    public class ItemToCheck
    {
        // ties the POCO to the DAL
        public int Id { get; set; }
        
        public string Instructions { get; set; }
        public bool IsComplete { get; set; }
        public string Notes { get; set; }
    }
}
