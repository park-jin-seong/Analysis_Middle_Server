using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Analysis_Middle_Server.Manager.DBManager;
using Analysis_Middle_Server.Manager.RenderManager;
using Analysis_Middle_Server.Structure.Analysis;
using Analysis_Middle_Server.TRD;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager
{
    public class SocketManagerClass : ISocketManagerClass
    {
        private ConnectThreadClass m_connectThreadClass;
        private List<SocketThread> m_SocketThreads;
        private IRenderManager m_RenderManager;
        public SocketManagerClass(IDBManagerClass dBManagerClass, IRenderManager renderManager)
        {
            m_RenderManager = renderManager;
            m_connectThreadClass = new ConnectThreadClass(dBManagerClass.GetMyServerInfosClass().serverPort);
            m_connectThreadClass.SetCallback(AddSenderSession);
            m_SocketThreads = new List<SocketThread>();
            m_connectThreadClass.Run();
        }
        public void AddSenderSession(TcpClient tcpClient, string type, StreamReader reader)
        {
            if (type.Equals("render"))
            {
                m_SocketThreads.Add(new SocketThread(tcpClient, reader));
                m_SocketThreads.Last<SocketThread>().SetCallback(SendResultDelegate, GetFrameDelegate, DeleteDelegate);
                m_SocketThreads.Last<SocketThread>().Run();
            }
        }

        public void SendResultDelegate(int userId, List<long> cameraIds, int x, int y)
        {
            m_RenderManager.MakeRender(userId, cameraIds, x, y);
        }
        public Mat GetFrameDelegate(int userId)
        {
            return m_RenderManager.GetImage(userId);
        }

        public void DeleteDelegate(int userId)
        {
            m_RenderManager.DeleteRender(userId);
        }

        internal void SetAnlysisAndStart()
        {
        }
    }
}
