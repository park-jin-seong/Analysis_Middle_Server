using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using Analysis_Middle_Server.Structure.Analysis;
using Newtonsoft.Json;

namespace Analysis_Middle_Server.TRD
{
    public class AnalysisReceiverThreadClass
    {
        private Thread m_Thread;
        private bool m_Running;
        private TcpClient m_tcpClient;
        private int m_CameraID;

        private string m_ip;
        private int m_port;

        private long m_analysisTime;

        private readonly object m_AnalysisLock = new object();
        private List<AnalysisReultClass> m_analysisReultClasses;

        public delegate void SendResultDelegate(int cameraId);
        private SendResultDelegate m_callback;

        public AnalysisReceiverThreadClass(string ip, int port, int cameraId)
        {
            m_ip = ip;
            m_port = port;
            m_CameraID = cameraId;
            m_analysisTime = 0;
            m_analysisReultClasses = new List<AnalysisReultClass>();
            m_Thread = new Thread(DoWork);
        }

        public void Start()
        {
            if (!m_Running)
            {
                m_Running = true;
                m_Thread.Start();
            }
        }

        public void Stop()
        {
            m_Running = false;
            m_tcpClient?.Close();
            m_Thread?.Join();
        }

        public int getCameraID()
        {
            return m_CameraID;
        }
        public List<AnalysisReultClass> GetAnalysisReult()
        {
            lock (m_AnalysisLock)
            {
                return new List<AnalysisReultClass>(m_analysisReultClasses);
            }
        }
        public void SetCallback(SendResultDelegate callback)
        {
            m_callback = callback;
        }

        private void DoWork()
        {
            while (m_Running)
            {
                try
                {
                    m_tcpClient = new TcpClient(m_ip, m_port);
                    NetworkStream stream = m_tcpClient.GetStream();
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

                    // 서버에 먼저 videoSourceId 전송 (SenderThreadClass에서 수신하므로)
                    writer.AutoFlush = true;

                    // 서버에서 ReadLine()으로 받기 때문에 반드시 \n 필요!
                    string msg = m_CameraID.ToString() + "\n";
                    writer.Write(msg);

                    while (m_Running)
                    {
                        // 1. 길이 정보(4바이트) 먼저 수신
                        byte[] lengthBuffer = new byte[4];
                        int read = stream.Read(lengthBuffer, 0, 4);
                        if (read == 0) break; // 연결 끊김

                        int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

                        // 2. 데이터 본문 수신
                        byte[] buffer = new byte[dataLength];
                        int totalRead = 0;
                        while (totalRead < dataLength)
                        {
                            int bytesRead = stream.Read(buffer, totalRead, dataLength - totalRead);
                            if (bytesRead == 0) break;
                            totalRead += bytesRead;
                        }

                        string json = Encoding.UTF8.GetString(buffer, 0, totalRead);

                        try
                        {
                            lock (m_AnalysisLock)
                            {
                                m_analysisReultClasses = JsonConvert.DeserializeObject<List<AnalysisReultClass>>(json);
                                m_callback.Invoke(m_CameraID);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("JSON 파싱 실패: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ReceiverThreadClass 오류: " + ex.Message);
                    Thread.Sleep(10000);
                }
                finally
                {
                    m_tcpClient?.Close();
                }
                
            }
        }
    }
}
