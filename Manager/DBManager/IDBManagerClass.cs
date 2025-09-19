using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Structure;
using Analysis_Middle_Server.Structure.DB;

namespace Analysis_Middle_Server.Manager.DBManager
{
    public interface IDBManagerClass
    {
        ServerInfosClass GetMyServerInfosClass();
        List<ServerInfosClass> GetServerInfosClasses();
        List<CameraInfoClass> GetCameraInfosClasses();
    }
}
