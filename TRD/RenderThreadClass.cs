using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using OpenCvSharp;

namespace Analysis_Middle_Server.TRD
{
    public class RenderThreadClass
    {
        private List<int> _cameraIdList;
        private Thread _thread;
        private bool _running;
        private int _fps; // 초당 프레임 수 제한
        private Mat _rtspStream;
        private object _rtspStreamLock = new object();

        public RenderThreadClass(List<int> cameraIdList, int fps = 30)
        {
            _cameraIdList = cameraIdList;
            _fps = fps > 0 ? fps : 30; // 0 이하일 경우 기본 30fps
            _rtspStream = new Mat();
        }

        public void Start()
        {
            if (_thread == null || !_thread.IsAlive)
            {
                _running = true;
                _thread = new Thread(Run);
                _thread.Start();
            }
        }

        public void Stop()
        {
            _running = false;
            _thread?.Join();
        }

        public Mat GetFrame()
        {
            lock (_rtspStreamLock)
            {
                return _rtspStream.Clone();
            }
        }
        public int GetCameraId()
        {
            return _cameraId;
        }

        private void Run()
        {
            var capture = new VideoCapture(_rtspUrl);

            if (!capture.IsOpened())
            {
                Console.WriteLine($"Failed to open RTSP: {_rtspUrl}");
                return;
            }

            Mat frame = new Mat();
            double frameIntervalMs = 1000.0 / _fps;

            while (_running)
            {
                var startTime = DateTime.Now;

                if (!capture.Read(frame) || frame.Empty())
                {
                    Thread.Sleep(10);
                    continue;
                }

                // 프레임 이벤트 발생
                lock (_rtspStreamLock)
                {
                    _rtspStream = frame.Clone();
                }

                // FPS 제한 적용
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                int sleepTime = (int)(frameIntervalMs - elapsed);
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);
            }

            frame.Dispose();
            capture.Release();
        }
    }
}