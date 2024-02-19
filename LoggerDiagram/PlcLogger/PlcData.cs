using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerDiagram
{
    public class PlcData
    {
        private RoomNameEnum RoomName;
        private Byte BytePlc;
        private float DoublePlc;

        public PlcData(RoomNameEnum roomName, Byte bytePlc, float doublePlc)
        {
            RoomName = roomName;
            BytePlc = bytePlc;
            DoublePlc = doublePlc;
        }

        public bool GetIsNewDiagramId()
        {
            if((int)BytePlc > 0)
                return true;
            else
                return false;

        }

        public Byte GetByte()
        {
            return BytePlc;
        }

        public Double GetDouble()
        {
            return DoublePlc;
        }

        public RoomNameEnum getNameRoom()
        {
            return RoomName;
        }

    }
}
