using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Manager.StreamManager;
using Analysis_Middle_Server.TRD;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager.RenderManager
{
    public class RenderManager : IRenderManager
    {
        private List<RenderThreadClass> m_RenderThreadClasses;
        private IStreamManger m_StreamManger;
        private IAnalysisReceiverManger m_AnalysisReceiverManger;
        public RenderManager(IStreamManger streamManger, IAnalysisReceiverManger analysisReceiverManger) 
        {
            m_StreamManger = streamManger;
            m_AnalysisReceiverManger = analysisReceiverManger;
            m_RenderThreadClasses = new List<RenderThreadClass>();
        }

        private Mat GetChannelImg(int cameraId)
        {
            return m_StreamManger.GetStream(cameraId);
        }

        public Mat GetImage(int userId)
        {
            foreach (RenderThreadClass thread in m_RenderThreadClasses)
            {
                if (thread.GetUserId().Equals(userId))
                {
                    return thread.GetImage();
                }
            }
            return null;
        }

        public void MakeRender(int userId, List<long> cameraIds, int x, int y)
        {
            RenderThreadClass tmpRenderThreadClass = new RenderThreadClass(userId, cameraIds, x, y);
            tmpRenderThreadClass.SetCallback(GetChannelImg);
            tmpRenderThreadClass.Run();
            m_RenderThreadClasses.Add(tmpRenderThreadClass);
        }
        
    }
}
