using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Structure.Analysis;
using OpenCvSharp;

namespace Analysis_Middle_Server.Manager
{
    public interface IAnalysisReceiverManger
    {
        List<AnalysisReultClass> GetAnalysisReult(int cameraId);
    }
}
