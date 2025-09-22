using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Manager.DBManager;
using Analysis_Middle_Server.Structure.DB;
using Analysis_Middle_Server.TRD;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager.StreamManager
{
    public class StreamManger : IStreamManger
    {
        private List<CameraInfoClass> m_CameraInfosClasses;
        private List<RtspStreamThreadClass> m_RtspStreamThreads;
        public StreamManger(IDBManagerClass dBManagerClass) {
            m_CameraInfosClasses = dBManagerClass.GetCameraInfosClasses();

            m_RtspStreamThreads = new List<RtspStreamThreadClass>();
            foreach (CameraInfoClass cameraInfosClasses in m_CameraInfosClasses)
            {
                RtspStreamThreadClass rtspStreamThread = new RtspStreamThread(cameraInfosClasses.cameraId, cameraInfosClasses.cctvUrl);
                rtspStreamThread.Start();
                m_RtspStreamThreads.Add(rtspStreamThread);
            }
        }

        public Mat GetStream(int cameraId)
        {
            foreach (RtspStreamThreadClass rtspStreamThread in m_RtspStreamThreads)
            {
                if (rtspStreamThread.GetCameraId() == cameraId)
                {
                    return rtspStreamThread.GetFrame();
                }

            }
            Mat mat = new Mat(new Size(720, 240), MatType.CV_8UC3, Scalar.All(0));
            return mat;
        }

        internal void SetAnlysisAndStart()
        {
        }
    }
}