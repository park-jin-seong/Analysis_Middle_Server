using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Analysis_Middle_Server.TRD;
using OpenCvSharp;

namespace Analysis_Middle_Server.TRD
{
    // 하나의 채널 단위: RTSP 입력 + TCP 분석 쓰레드
    public class StreamSessionThread
    {
        private RtspStreamThread _rtspThread;
        private bool _running;
        private readonly object _frameLock = new object();

        // 분석용 데이터 큐 (예시, 단순)
        private Mat _latestFrame;

        public StreamSessionThread(string rtspUrl, int fps = 30)
        {
            _rtspThread = new RtspStreamThread(rtspUrl, fps);
        }
        public Mat GetLatestFrame()
        {
            lock (_frameLock)
            {
                return _latestFrame?.Clone();
            }
        }

        // 세션 시작
        public void Start()
        {
            if (_running) return;
            _running = true;

            // RTSP 스트림 시작
            _rtspThread.Start();

        }

        // 세션 종료
        public void Stop()
        {
            _running = false;
            _rtspThread.Stop();
        }

        
    }
}
