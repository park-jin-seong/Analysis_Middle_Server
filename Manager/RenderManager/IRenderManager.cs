using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager
{
    public interface IRenderManager
    {
        void MakeRender(int userId, List<long> cameraIds, int x, int y);
        Mat GetImage(int userId);

        void SetAnalysisTime(int cameraId);

        void DeleteRender(int userId);
    }
}
