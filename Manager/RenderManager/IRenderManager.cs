using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis_Middle_Server.Manager
{
    public interface IRenderManager
    {
        void MakeRender(List<int> cameraIdList);
    }
}
