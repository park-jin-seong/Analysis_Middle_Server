using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis_Middle_Server.Structure
{
    public class SystemInfoClass
    {
        public DataBaseClass m_dataBaseClass { get; set; }

        public SystemInfoClass() 
        {
            m_dataBaseClass = new DataBaseClass();
        }

        public SystemInfoClass(int DBType, string ip, string port, string id, string pw)
        {
            m_dataBaseClass = new DataBaseClass(DBType, ip, port, id, pw);
        }


    }
}
