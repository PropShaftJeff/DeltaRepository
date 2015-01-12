using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSS.Delta.BusinessObjects;

namespace PSS.Delta.BusinessObjects.Controllers
{
    public class Engineer
    {
        private bool _isValidated;

        public Engineer()
        {
            _isValidated = true;
        }

        public Engineer(Security.UserCredentials credentials)
        {
            _isValidated = credentials.AreVerified;
        }

        public EngineeringChangeNotice CreateNewEcn(string partId)
        {
            if (_isValidated)
            {
                return new EngineeringChangeNotice(partId);
            }
            else
            {
                throw new Exception("You do not have permission to create a new ECN.\n See Jeff for details.");
            }
        }

        public void LoadDataFromVjs(EngineeringChangeNotice ecn)
        {
            //use a separate manager for the VJS side, because it is a separate datebase.
            VjsManager vjsManager = new VjsManager();
            vjsManager.GetDataFor(ecn);
        }

        

    }
}
