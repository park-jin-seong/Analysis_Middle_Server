using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Analysis_Middle_Server.DI;
using Analysis_Middle_Server.Manager;
using Analysis_Middle_Server.Manager.DBManager;
using Analysis_Middle_Server.Manager.StreamManager;
using Analysis_Middle_Server.Structure.Stream;
using Analysis_Middle_Server.TRD;
using Newtonsoft.Json;
using Ninject;

namespace Analysis_Middle_Server
{
    internal class Program
    {
        private static IKernel kernel;
        static void Main(string[] args)
        {
            SetAnlysisStart();
            //int port = 20000;
            //TcpListener listener = new TcpListener(IPAddress.Any, port);
            //listener.Start();
            //Console.WriteLine($"Server listening on port {port}...");

            //TcpClient client = listener.AcceptTcpClient();
            //Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

            //using (NetworkStream stream = client.GetStream())
            //using (StreamReader reader = new StreamReader(stream))
            //using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            //{
            //    writer.AutoFlush = true;

            //    // 1. JSON 읽기
            //    string json = reader.ReadLine(); // 메시지 끝은 \n
            //    StreamInfoClass numbers = JsonConvert.DeserializeObject<StreamInfoClass>(json);
            //    Console.WriteLine("받은 객체: " + numbers.ToString());

            //    // 2. 문자열 응답 보내기
            //    string response = "rtsp://192.168.0.10:554/" + numbers.userId.ToString() + "\n";
            //    writer.Write(response); // 반드시 \n 붙여야 클라이언트 ReadLine() 가능
            //    Console.WriteLine("클라이언트로 응답 전송: " + response.Trim());
            //}
            //client.Close();
            //listener.Stop();
        }

        private static void SetAnlysisStart()
        {
            kernel = new StandardKernel(new AppModuleClass());
            kernel.Get<StreamManger>().SetAnlysisAndStart();
            kernel.Get<AnalysisReceiverManger>().SetAnlysisAndStart();
            kernel.Get<SocketManagerClass>().SetAnlysisAndStart();
        }
    }
}
