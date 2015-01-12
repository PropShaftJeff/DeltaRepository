using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSS.Delta.BusinessObjects.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSS.Delta.BusinessObjects;

namespace PSS.Delta.BusinessObjects.Controllers.Tests
{
    [TestClass()]
    public class EngineerTests
    {
        [TestMethod()]
        public void LoadDataFromVjsTest()
        {
            Engineer engineer = new Engineer();
            EngineeringChangeNotice ecn = engineer.CreateNewEcn("135257");
            engineer.LoadDataFromVjs(ecn);

            Assert.AreEqual(ecn.Description, "ASSEMBLY, CV; 2.00-16; 11.68 F2E; 14000072");
            Assert.AreEqual(ecn.PssRevision, "H");
        }
    }
}
