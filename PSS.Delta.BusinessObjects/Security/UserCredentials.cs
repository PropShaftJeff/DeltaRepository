using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects.Security
{
    public class UserCredentials
    {
        public UserCredentials(string username, string password)
        {
            //validate the user.
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public bool AreVerified
        {
            get;
            private set;
        }
    }
}