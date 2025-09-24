using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis_Middle_Server.Structure.Stream
{
    public class StreamInfoClass
    {
        public long userId;
        public List<long> cameraIds;
        public int x;
        public int y;

        public StreamInfoClass(long id)
        {
            userId = id;
            cameraIds = new List<long>();
            x = 0;
            y = 0;
        }
    }
}
