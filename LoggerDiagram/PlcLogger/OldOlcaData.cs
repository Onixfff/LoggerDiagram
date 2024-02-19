using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerDiagram.Plc
{
    internal class OldOlcaData
    {
        private RoomNameEnum RoomName;
        private bool Update;

        public OldOlcaData(RoomNameEnum roomName ,Byte bite)
        {
            if ((int)bite > 0) 
            {
                RoomName = roomName;
                Update = true;
            }
        }

        private bool GetWasUpdate()
        {
            return Update;
        }
    }
}
