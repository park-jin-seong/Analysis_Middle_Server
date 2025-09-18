using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Structure;

namespace Analysis_Middle_Server.Manager.SystemInfoManager
{
    public interface ISystemInfoManagerClass
    {
        SystemInfoClass GetSystemInfoClass();
    }
}
