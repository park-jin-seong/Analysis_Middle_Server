using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using OpenCvSharp;

namespace Analysis_Middle_Server.TRD
{
    public class RtspStreamThreadClass
    {
        private int _cameraId;
        private Thread _thread;
        private string _rtspUrl;
        private bool _running;
        private int _fps;
        private Mat _rtspStream;
        private readonly object _rtspStreamLock = new object();

        public RtspStreamThreadClass(int cameraId, string rtspUrl, int fps = 60)
        {
            _cameraId = cameraId;
            _rtspUrl = rtspUrl;
            _fps = fps > 0 ? fps : 60;
            _rtspStream = new Mat();
        }

        public void Start()
        {
            if (_thread == null || !_thread.IsAlive)
            {
                _running = true;
                _thread = new Thread(Run) { IsBackground = true };
                _thread.Start();
            }
        }

        public void Stop()
        {
            _running = false;
            _thread?.Join();

            lock (_rtspStreamLock)
            {
                _rtspStream?.Dispose();
                _rtspStream = null;
            }
        }

        public Mat GetFrame()
        {
            lock (_rtspStreamLock)
            {
                return _rtspStream?.Clone(); // 안전하게 복사
            }
        }

        public int GetCameraId() => _cameraId;


        private void Run()
        {
            VideoCapture capture = new VideoCapture(_rtspUrl);

            if (!capture.IsOpened())
            {
                Console.WriteLine($"Failed to open RTSP: {_rtspUrl}");
                return;
            }

            Mat frame = new Mat();
            double frameIntervalMs = 1000.0 / _fps;

            bool savedOnce = false;

            while (_running)
            {
                var startTime = DateTime.Now;

                if (!capture.Read(frame) || frame.Empty())
                {
                    Thread.Sleep(10);
                    continue;
                }
                //if (!savedOnce)
                //{
                //    try
                //    {
                //        string dir = @"C:\RTSP_Frames";
                //        Directory.CreateDirectory(dir); // 없으면 생성
                //        string savePath = $@"C:\RTSP_Frames\camera_{_cameraId}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                //        Cv2.ImWrite(savePath, frame);
                //        Console.WriteLine($"Saved first frame as {savePath}");
                //        savedOnce = true;
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine($"Failed to save frame: {ex.Message}");
                //    }
                //}

                // 이전 Mat Dispose 후 교체
                lock (_rtspStreamLock)
                {
                    _rtspStream?.Dispose();
                    _rtspStream = frame.Clone();
                }

                // FPS 제한
                //var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                //int sleepTime = (int)(frameIntervalMs - elapsed);
                //if (sleepTime > 0)
                //    Thread.Sleep(sleepTime);
                Thread.Sleep(10);
            }
            frame.Dispose();
        }
    }
}
