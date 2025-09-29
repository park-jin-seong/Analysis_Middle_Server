using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis_Middle_Server.Structure
{
    public class RenderShowClass
    {
        public int m_userId;
        public List<AnalysisShowClass> m_AnalysisShowClasses;

        public RenderShowClass(int userId, List<AnalysisShowClass> analysisShowClasses)
        {
            m_userId = userId;
            m_AnalysisShowClasses = analysisShowClasses;
        }
    }
    public class AnalysisShowClass
    {
        public int cameraId;
        public bool isShow;
        public long recentAnalysisResultTime;

        public AnalysisShowClass(int cameraId)
        {
            this.cameraId = cameraId;
            this.isShow = false;
            this.recentAnalysisResultTime = 0;
        }
    }
}
