using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using Analysis_Middle_Server.Structure.Stream;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using OpenCvSharp;

namespace Analysis_Middle_Server.TRD
{
    public class RenderThreadClass
    {
        private Thread m_Thread;
        private bool m_Running;
        private bool m_pause;

        private int m_userId;
        private List<long> m_cameraIds;
        private int m_x;
        private int m_y;

        private Mat m_TotalImg;

        public delegate Mat SendResultDelegate(int userId, int cameraId);
        private SendResultDelegate m_callback;
        private int m_cellWidth = 720;
        private int m_cellHeight = 480;
        private object m_lock;

        public RenderThreadClass(int userId, List<long> cameraIds, int x, int y)
        {
            m_userId = userId;
            m_cameraIds = cameraIds;
            m_x = x;
            m_y = y;

            m_Thread = new Thread(DoWork);
            m_Running = false;
            m_pause = false;
            m_TotalImg = new Mat();
            m_lock = new object();
        }

        public void SetCallback(SendResultDelegate callback)
        {
            m_callback = callback;
        }

        public Mat GetImage()
        {
            lock (m_lock)
            {
                return m_TotalImg.Clone();
            }
            
        }

        public int GetUserId()
        {
            return m_userId;
        }

        public void Run()
        {
            m_Running = true;
            m_pause = true;
            m_Thread.Start();
        }

        public void pause()
        {
            m_pause = false;
        }

        public void restart()
        {
            m_pause = true;
        }

        public void quit()
        {
            m_Running = false;
            m_pause = false;

            if (m_Thread != null && m_Thread.IsAlive)
            {
                m_Thread.Join();
                m_Thread = null;
            }
        }
        private void DoWork()
        {
            while (m_Running)
            {
                if (!m_pause)
                {
                    Thread.Sleep(100);
                    continue;
                }

                try
                {
                    int totalWidth = m_x * m_cellWidth;
                    int totalHeight = m_y * m_cellHeight;

                    using (Mat img = new Mat(totalHeight, totalWidth, MatType.CV_8UC3, Scalar.Black))
                    {
                        int cnt = 0;
                        for (int j = 0; j < m_y; j++)
                        {
                            for (int i = 0; i < m_x; i++)
                            {
                                if (cnt >= m_cameraIds.Count)
                                {
                                    j = m_y;
                                    break;
                                }

                                try
                                {
                                    Mat frame = m_callback?.Invoke(m_userId,(int)m_cameraIds[cnt]) ?? new Mat();
                                    if (!frame.Empty())
                                    {
                                        Cv2.Resize(frame, frame, new OpenCvSharp.Size(m_cellWidth, m_cellHeight));
                                        Rect roi = new Rect(i * m_cellWidth, j * m_cellHeight, m_cellWidth, m_cellHeight);
                                        frame.CopyTo(new Mat(img, roi));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Frame error (CameraId={m_cameraIds[cnt]}): {ex}");
                                }

                                cnt++;
                            }
                        }

                        lock (m_lock)
                        {
                            m_TotalImg?.Dispose();
                            m_TotalImg = img.Clone();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RenderThreadClass] Error: {ex}");
                }

                Thread.Sleep(30);
            }
        }


    }
}
