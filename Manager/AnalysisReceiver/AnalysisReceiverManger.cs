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
using Ninject;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager
{
    public class AnalysisReceiverManger : IAnalysisReceiverManger
    {
        private List<ServerInfosClass> m_ServerInfosClasses;
        private List<CameraInfoClass> m_CameraInfosClasses;
        private List<AnalysisReceiverThreadClass> analysisReceiverThreadClasses;

        [Inject]
        public IRenderManager m_RenderManager { get; set; }
        public AnalysisReceiverManger(IDBManagerClass dBManagerClass) {
            m_ServerInfosClasses = dBManagerClass.GetServerInfosClasses();
            m_CameraInfosClasses = dBManagerClass.GetCameraInfosClasses();
            analysisReceiverThreadClasses = new List<AnalysisReceiverThreadClass>();

            foreach (CameraInfoClass cameraInfoClass in m_CameraInfosClasses)
            {
                if (cameraInfoClass.isAnalisis != true)
                {
                    continue;
                }
                ServerInfosClass tmpServerInfo = null;
                foreach (ServerInfosClass serverInfosClass in m_ServerInfosClasses)
                {
                    if (serverInfosClass.serverId == cameraInfoClass.analysisServerId)
                    {
                        tmpServerInfo = serverInfosClass;
                    }
                }
                if (tmpServerInfo == null)
                {
                    continue;
                }
                AnalysisReceiverThreadClass analysisReceiverThreadClass = new AnalysisReceiverThreadClass(tmpServerInfo.serverIp, tmpServerInfo.serverPort, cameraInfoClass.cameraId);
                analysisReceiverThreadClass.Start();
                analysisReceiverThreadClass.SetCallback(SetAnalysisTime);
                analysisReceiverThreadClasses.Add(analysisReceiverThreadClass);
            }
        }

        public void SetAnalysisTime(int cameraId)
        {
            if (m_RenderManager == null)
            {
                return;
            }
            m_RenderManager.SetAnalysisTime(cameraId);
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
        

        internal void SetAnlysisAndStart()
        {
        }
    }
}