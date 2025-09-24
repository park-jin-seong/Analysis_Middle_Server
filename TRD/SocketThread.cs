using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Analysis_Middle_Server.Structure.Analysis;
using Analysis_Middle_Server.Structure.Stream;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using OpenCvSharp;

namespace Analysis_Middle_Server.TRD
{
    public class SocketThread
    {
        private Thread m_Thread;
        private bool m_Running;
        private bool m_pause;
        private TcpClient m_tcpClient;
        private StreamReader m_reader;

        public delegate void SendResultDelegate(int userId, List<long> cameraIds, int x, int y);
        private SendResultDelegate m_callback;

        public delegate Mat GetFrameDelegate(int userId);
        private GetFrameDelegate m_FrameCallback;
        public SocketThread(TcpClient tcpClient, StreamReader reader)
        {
            m_tcpClient = tcpClient;
            m_reader = reader;
            m_Thread = new Thread(DoWork);
            m_Running = false;
            m_pause = false;
        }

        public void SetCallback(SendResultDelegate callback1, GetFrameDelegate callback2)
        {
            m_callback = callback1;
            m_FrameCallback = callback2;
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
            m_Thread.Join();
            m_Thread = null;
        }

        private void DoWork()
        {
            NetworkStream stream = m_tcpClient.GetStream();

            // 1. JSON 읽기
            string json = m_reader.ReadLine(); // 메시지 끝은 \n
            StreamInfoClass numbers = JsonConvert.DeserializeObject<StreamInfoClass>(json);
            Console.WriteLine("받은 객체: " + numbers.ToString());
            m_callback.Invoke(Convert.ToInt32(numbers.userId), numbers.cameraIds, numbers.x, numbers.y);
            while (m_Running)
            {
                while (m_pause)
                {
                    try
                    {
                        Mat sendMat = m_FrameCallback.Invoke(Convert.ToInt32(numbers.userId));

                        if (sendMat != null && !sendMat.Empty())
                        {
                            byte[] imgBytes = sendMat.ToBytes(".jpg");

                            // 길이 → 4바이트 빅엔디언
                            int length = imgBytes.Length;
                            byte[] lenBytes = BitConverter.GetBytes(length);
                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(lenBytes);

                            // 길이 + 이미지 전송
                            stream.Write(lenBytes, 0, lenBytes.Length);
                            stream.Write(imgBytes, 0, imgBytes.Length);
                            stream.Flush();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(m_tcpClient.Client.RemoteEndPoint + "통신 종료");
                        m_Running = false;
                        m_pause = false;
                    }
                    Thread.Sleep(33);
                }
            }
            m_tcpClient.Close();
        }
    }
}
