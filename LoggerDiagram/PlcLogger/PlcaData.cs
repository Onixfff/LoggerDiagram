using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerDiagram
{
    public class PlcaData
    {
        private RoomNameEnum RoomName;
        private Byte BytePlc;
        private double DoublePlc;

        public PlcaData(RoomNameEnum roomName, Byte bytePlc, double doublePlc)
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
