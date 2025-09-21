using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
        private List<ServerInfosClass> m_ServerInfosClasses;
        private List<CameraInfoClass> m_CameraInfosClasses;
        private List<AnalysisReceiverThreadClass> analysisReceiverThreadClasses;
        public ReceiverManger(IDBManagerClass dBManagerClass) {
            m_ServerInfosClasses = dBManagerClass.GetServerInfosClasses();
            m_CameraInfosClasses = dBManagerClass.GetCameraInfosClasses();
            analysisReceiverThreadClasses = new List<AnalysisReceiverThreadClass>();

            foreach (CameraInfoClass cameraInfoClass in m_CameraInfosClasses)
            {
                ServerInfosClass tmpServerInfo = new ServerInfosClass();
                foreach (ServerInfosClass serverInfosClass in m_ServerInfosClasses)
                {
                    if (serverInfosClass.serverId == cameraInfoClass.analysisServerId)
                    {
                        tmpServerInfo = serverInfosClass;
                    }
                }
                AnalysisReceiverThreadClass analysisReceiverThreadClass = new AnalysisReceiverThreadClass(new TcpClient(tmpServerInfo.serverIp, tmpServerInfo.serverPort), cameraInfoClass.cameraId);
            }

        }

        public List<AnalysisReultClass> GetAnalysisReult(int cameraId)
        {
            foreach (AnalysisReceiverThreadClass analysisReceiverThreadClass in analysisReceiverThreadClasses)
            {
                if (analysisReceiverThreadClass.getCameraID() == cameraId)
                {
                    return analysisReceiverThreadClass.GetAnalysisReult();
                }
            }
            return null;
        }
    }
}