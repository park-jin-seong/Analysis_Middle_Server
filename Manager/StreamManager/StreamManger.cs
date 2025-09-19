using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Manager.DBManager;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager.StreamManager
{
    public class StreamManger : IStreamManger
    {
        public StreamManger(IDBManagerClass dBManagerClass) { }

        public Mat GetStream(int cameraId)
        {
            throw new NotImplementedException();
        }
    }
}
