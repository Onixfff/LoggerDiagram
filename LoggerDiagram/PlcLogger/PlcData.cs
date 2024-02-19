using System;

namespace LoggerDiagram
{
    public class PlcData
    {
        private RoomNameEnum RoomName;
        private Byte BytePlc;
        private float DoublePlc;
        private int Time;

        public PlcData(RoomNameEnum roomName, Byte bytePlc, float doublePlc, int time)
        {
            RoomName = roomName;
            BytePlc = bytePlc;
            DoublePlc = doublePlc;
            Time = time;
        }

        public bool GetIsNewDiagramId()
        {
            if ((int)BytePlc > 0)
                return true;
            else
                return false;

        }

        public Byte GetByte()
        {
            return BytePlc;
        }

        public float GetFloat()
        {
            return DoublePlc;
        }

        public RoomNameEnum getNameRoom()
        {
            return RoomName;
        }

        public int GetTime()
        {
            return Time;
        }

    }
}