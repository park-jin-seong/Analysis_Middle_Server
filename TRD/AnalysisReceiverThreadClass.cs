using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private readonly object m_AnalysisLock = new object();
        private List<AnalysisReultClass> m_analysisReultClasses;

        public AnalysisReceiverThreadClass(TcpClient tcpClient, int cameraId)
        {
            m_tcpClient = tcpClient;
            m_CameraID = cameraId;
            m_analysisReultClasses = new List<AnalysisReultClass>();
            m_Thread = new Thread(DoWork);
        }

        public void Start()
        {
            if (!m_Running)
            {
                m_Running = true;
                m_Thread.IsBackground = true;
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
                return JsonConvert.DeserializeObject<List<AnalysisReultClass>>(JsonConvert.SerializeObject(m_analysisReultClasses));
            }
        }

        private void DoWork()
        {
            try
            {
                NetworkStream stream = m_tcpClient.GetStream();

                // 서버에 먼저 videoSourceId 전송 (SenderThreadClass에서 수신하므로)
                byte[] sourceIdBytes = Encoding.UTF8.GetBytes(m_CameraID.ToString());
                byte[] lengthPrefix = BitConverter.GetBytes(sourceIdBytes.Length);
                stream.Write(lengthPrefix, 0, lengthPrefix.Length);
                stream.Write(sourceIdBytes, 0, sourceIdBytes.Length);

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
                        List<AnalysisReultClass> result = JsonConvert.DeserializeObject<List<AnalysisReultClass>>(json);
                        lock (m_AnalysisLock)
                        {
                            m_analysisReultClasses = result;
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
            }
            finally
            {
                m_tcpClient?.Close();
            }
        }
    }
}
