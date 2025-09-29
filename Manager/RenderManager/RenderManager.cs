using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Manager.StreamManager;
using Analysis_Middle_Server.Structure;
using Analysis_Middle_Server.Structure.Analysis;
using Analysis_Middle_Server.TRD;
using Ninject;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager.RenderManager
{
    public class RenderManager : IRenderManager
    {
        private List<RenderThreadClass> m_RenderThreadClasses;
        private IStreamManger m_StreamManger;

        [Inject]
        public IAnalysisReceiverManger m_AnalysisReceiverManger { get; set; }
        private List<RenderShowClass> m_RenderShowClasses;

        private object m_Lock;
        public RenderManager(IStreamManger streamManger) 
        {
            m_StreamManger = streamManger;
            m_RenderThreadClasses = new List<RenderThreadClass>();
            m_RenderShowClasses = new List<RenderShowClass>();
            m_Lock = new object();
        }
        private Mat GetChannelImg(int userId, int cameraId)
        {
            long showCount = 0;
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            foreach (RenderShowClass renderShowClass in m_RenderShowClasses)
            {
                if (renderShowClass.m_userId == userId)
                {
                    foreach (AnalysisShowClass analysisShowClass in renderShowClass.m_AnalysisShowClasses)
                    {
                        if (analysisShowClass.cameraId == cameraId)
                        {
                            if (analysisShowClass.isShow && now - analysisShowClass.recentAnalysisResultTime <= 15)
                            {
                                showCount = now - analysisShowClass.recentAnalysisResultTime;
                                showCount = 15 - showCount;
                                break;
                            }
                            else if (analysisShowClass.isShow)
                            {
                                analysisShowClass.isShow = false;
                            }
                            return new Mat();
                        }
                    }
                    break;
                }
            }
            Mat mat = m_StreamManger.GetStream(cameraId);
            if (mat != null)
            {
                List<AnalysisReultClass> reultClasses = m_AnalysisReceiverManger.GetAnalysisReult(cameraId);
                if (reultClasses != null)
                {
                    int imgW = mat.Width;
                    int imgH = mat.Height;

                    foreach (AnalysisReultClass reultClass in reultClasses)
                    {
                        // 비율 → 픽셀 단위 변환
                        int x1 = (int)(reultClass.m_x * imgW);
                        int y1 = (int)(reultClass.m_y * imgH);
                        int x2 = (int)((reultClass.m_x + reultClass.m_width) * imgW);
                        int y2 = (int)((reultClass.m_y + reultClass.m_height) * imgH);

                        // 사각형 그리기 (빨간색, 두께 2)
                        Cv2.Rectangle(mat, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2),
                                      new Scalar(0, 0, 255), 2);

                        // 라벨 (classId + score)
                        string label = $"ID:{reultClass.m_classId} ({reultClass.m_score:F2})";
                        int baseLine;
                        double fontScale = 0.7; // 글자 크기 키움
                        int thickness = 2;
                        Size textSize = Cv2.GetTextSize(label, HersheyFonts.HersheySimplex, fontScale, thickness, out baseLine);

                        int padding = 4; // 박스 여백
                        int textY = Math.Max(y1 - 5, textSize.Height + baseLine + padding);
                        int bgY = Math.Max(0, y1 - textSize.Height - baseLine - padding);

                        // 텍스트 배경 박스 (박스 키우기)
                        Cv2.Rectangle(mat,
                                      new Rect(new Point(x1 - padding, bgY),
                                               new Size(textSize.Width + padding * 2, textSize.Height + baseLine + padding * 2)),
                                      new Scalar(0, 0, 255), Cv2.FILLED);

                        // 텍스트 쓰기 (하얀색)
                        Cv2.PutText(mat, label, new Point(x1, textY),
                                    HersheyFonts.HersheySimplex, fontScale, new Scalar(255, 255, 255), thickness);
                    }
                }
                if (showCount > 0)
                {
                    string countText = showCount.ToString();

                    int baseLine;
                    double fontScale = 1.0;
                    int thickness = 2;
                    Size textSize = Cv2.GetTextSize(countText, HersheyFonts.HersheySimplex, fontScale, thickness, out baseLine);

                    int x = mat.Width - textSize.Width - 10; // 오른쪽 여백 10
                    int y = mat.Height - 10;                 // 아래 여백 10

                    Cv2.PutText(mat, countText, new Point(x, y),
                                HersheyFonts.HersheySimplex, fontScale, new Scalar(0, 0, 255), thickness);
                }
            }
            return mat;
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
            List<AnalysisShowClass> analysisShowClasses = new List<AnalysisShowClass>();
            foreach (long cameraId in cameraIds)
            {
                AnalysisShowClass analysisShowClass = new AnalysisShowClass(Convert.ToInt32(cameraId));
                analysisShowClasses.Add(analysisShowClass);
            }

            m_RenderShowClasses.Add(new RenderShowClass(userId,analysisShowClasses));

            RenderThreadClass tmpRenderThreadClass = new RenderThreadClass(userId, cameraIds, x, y);
            tmpRenderThreadClass.SetCallback(GetChannelImg);
            tmpRenderThreadClass.Run();
            m_RenderThreadClasses.Add(tmpRenderThreadClass);
        }

        public void SetAnalysisTime(int cameraId)
        {
            lock (m_Lock)
            {
                foreach (RenderShowClass renderShowClass in m_RenderShowClasses)
                {
                    foreach (AnalysisShowClass analysisShowClass in renderShowClass.m_AnalysisShowClasses)
                    {
                        if (analysisShowClass.cameraId == cameraId && !analysisShowClass.isShow)
                        {
                            if (!analysisShowClass.isShow)
                            {
                                analysisShowClass.recentAnalysisResultTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            }
                            analysisShowClass.isShow = true;
                        }
                    }

                  
                }
            }
        }

        internal void SetAnlysisAndStart()
        {
            
        }
    }
}
