using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerDiagram.Plc
{
    internal class OldPlcData
    {
        private RoomNameEnum RoomName;
        private StatusByteEnum StatusByte;

        public OldPlcData(RoomNameEnum roomName , StatusByteEnum statusByteEnum)
        {
            RoomName = roomName;
            StatusByte = statusByteEnum;
        }

        public RoomNameEnum GetRoomName() 
        {
            return RoomName;
        }

        public StatusByteEnum GetStatusByte()
        {
            return StatusByte;
        }
    }
}
