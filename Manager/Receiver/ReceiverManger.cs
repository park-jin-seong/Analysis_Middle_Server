using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Manager.DBManager;
using Analysis_Middle_Server.Structure.Analysis;
using Analysis_Middle_Server.Structure.DB;
using Analysis_Middle_Server.TRD;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager
{
    public class ReceiverManger : IReceiverManger
    {
        private List<CameraInfoClass> m_CameraInfosClasses;
        private List<RtspStreamThread> m_RtspStreamThreads;
        public ReceiverManger(IDBManagerClass dBManagerClass) {
            m_CameraInfosClasses = dBManagerClass.GetCameraInfosClasses();

            m_RtspStreamThreads = new List<RtspStreamThread>();
            foreach (CameraInfoClass cameraInfosClasses in m_CameraInfosClasses)
            {
                RtspStreamThread rtspStreamThread = new RtspStreamThread(cameraInfosClasses.cameraId, cameraInfosClasses.cctvUrl);
                rtspStreamThread.Start();
                m_RtspStreamThreads.Add(rtspStreamThread);
            }
        }

        public List<AnalysisReultClass> GetAnalysisReult(int cameraId)
        {
            foreach (RtspStreamThread rtspStreamThread in m_RtspStreamThreads)
            {
                if (rtspStreamThread.GetCameraId() == cameraId)
                {
                    return rtspStreamThread.GetFrame();
                }

            }
            Mat mat = new Mat(new Size(720, 240), MatType.CV_8UC3, Scalar.All(0));
            return mat;
        }
    }
}