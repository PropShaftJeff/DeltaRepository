using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSS.Delta.BusinessObjects.Mapping
{
    internal interface IMapper
    {
        void MapDbToPoco(object db, object poco);
        void MapPocoToDb(object poco, object db);

    }
}
