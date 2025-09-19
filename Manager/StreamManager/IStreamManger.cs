using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager.StreamManager
{
    public interface IStreamManger
    {
        Mat GetStream(int cameraId);
    }
}
