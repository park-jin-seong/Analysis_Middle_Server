using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Manager.SystemInfoManager;
using Analysis_Middle_Server.Structure;
using Analysis_Middle_Server.Structure.DB;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Analysis_Middle_Server.Manager.DBManager
{
    public class DBManagerClass : IDBManagerClass
    {
        private SystemInfoClass m_SystemInfoClass;
        private ServerInfosClass m_MyServerInfosClass;
        private List<ServerInfosClass> m_ServerInfosClasses;
        private List<CameraInfoClass> m_CameraInfosClasses;

        public DBManagerClass(ISystemInfoManagerClass systemInfoManagerClass)
        {
            m_SystemInfoClass = systemInfoManagerClass.GetSystemInfoClass();
            m_ServerInfosClasses = new List<ServerInfosClass>();
            m_CameraInfosClasses = new List<CameraInfoClass>();

            InsertServerInfo();
            GetCameraInfosDB();
        }
        public ServerInfosClass GetMyServerInfosClass() => m_MyServerInfosClass;
        public List<ServerInfosClass> GetServerInfosClasses() => m_ServerInfosClasses;
        public List<CameraInfoClass> GetCameraInfosClasses() => m_CameraInfosClasses;

        private void InsertServerInfo()
        {
            var host = m_SystemInfoClass.m_dataBaseClass.m_Ip;
            var port = m_SystemInfoClass.m_dataBaseClass.m_Port;
            var user = m_SystemInfoClass.m_dataBaseClass.m_Id;
            var password = m_SystemInfoClass.m_dataBaseClass.m_Pw;
            var analysisDatabaseName = "sentry_server";


            var dbConnString = $"Server={host};Port={port};Uid={user};Pwd={password};Database={analysisDatabaseName};";
            using (var conn = new MySqlConnection(dbConnString))
            {
                conn.Open();

                string targetIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                string targetPort = "20000";
                string targetJson = JsonConvert.SerializeObject(new List<string>());

                using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM serverInfos WHERE serverIp = @ip AND serverType = 'Middle';", conn))
                {
                    checkCmd.Parameters.AddWithValue("@ip", targetIp);
                    long count = Convert.ToInt64(checkCmd.ExecuteScalar());

                    if (count == 0)
                    {
                        using (var insertCmd = new MySqlCommand("INSERT INTO serverInfos (serverIp, serverPort, serverType, osId, osPw) VALUES (@ip, @port, @type, @osId, @osPw);", conn))
                        {
                            insertCmd.Parameters.AddWithValue("@ip", targetIp);
                            insertCmd.Parameters.AddWithValue("@port", int.Parse(targetPort)); // int로 변환
                            insertCmd.Parameters.AddWithValue("@type", "Middle");
                            insertCmd.Parameters.AddWithValue("@osId", "");
                            insertCmd.Parameters.AddWithValue("@osPw", "");

                            insertCmd.ExecuteNonQuery();
                            Console.WriteLine("새 서버 데이터 삽입 완료");
                        }
                    }
                    using (var selectCmd = new MySqlCommand(
                            "SELECT serverId, serverIp, serverPort, serverType, osId, osPw FROM serverInfos WHERE serverIp = @ip AND serverType = 'Middle';", conn))
                    {
                        selectCmd.Parameters.AddWithValue("@ip", targetIp);

                        using (var reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                m_MyServerInfosClass = new ServerInfosClass(
                                    reader.GetInt32("serverId"),
                                    reader.GetString("serverIp"),
                                    reader.GetInt32("serverPort"),
                                    reader.GetString("serverType"),
                                    reader.IsDBNull(reader.GetOrdinal("osId")) ? "" : reader.GetString("osId"),
                                    reader.IsDBNull(reader.GetOrdinal("osPw")) ? "" : reader.GetString("osPw")
                                );

                                Console.WriteLine("이미 존재하는 서버 정보:");
                                Console.WriteLine($"ID: {m_MyServerInfosClass.serverId}, IP: {m_MyServerInfosClass.serverIp}, Port: {m_MyServerInfosClass.serverPort}, Type: {m_MyServerInfosClass.serverType}");
                            }
                        }
                    }
                    using (var selectCmd = new MySqlCommand(
                           "SELECT serverId, serverIp, serverPort, serverType, osId, osPw FROM serverInfos;", conn))
                    {

                        using (var reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ServerInfosClass tempServerInfosClass = new ServerInfosClass(
                                    reader.GetInt32("serverId"),
                                    reader.GetString("serverIp"),
                                    reader.GetInt32("serverPort"),
                                    reader.GetString("serverType"),
                                    reader.IsDBNull(reader.GetOrdinal("osId")) ? "" : reader.GetString("osId"),
                                    reader.IsDBNull(reader.GetOrdinal("osPw")) ? "" : reader.GetString("osPw")
                                );
                                m_ServerInfosClasses.Add(tempServerInfosClass);
                            }
                        }
                    }
                }
            }
        }
        private void GetCameraInfosDB()
        {
            string targetIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            var host = m_SystemInfoClass.m_dataBaseClass.m_Ip;
            var port = m_SystemInfoClass.m_dataBaseClass.m_Port;
            var user = m_SystemInfoClass.m_dataBaseClass.m_Id;
            var password = m_SystemInfoClass.m_dataBaseClass.m_Pw;
            var databaseName = "sentry_server";
            var dbConnString = $"Server={host};Port={port};Uid={user};Pwd={password};Database={databaseName};";

            using (var conn = new MySqlConnection(dbConnString))
            {
                conn.Open();

                using (var cmd = new MySqlCommand(
                    "SELECT cameraId, cameraName, cctvUrl, coordx, coordy, isAnalisis, analysisServerId FROM camerainfos WHERE isAnalisis = 1;", conn))
                {

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            m_CameraInfosClasses.Add(new CameraInfoClass(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetFloat(3),
                                reader.GetFloat(4),
                                reader.GetBoolean(5),
                                reader.GetInt32(6)));
                        }
                    }
                }
            }
        }
    }
}
