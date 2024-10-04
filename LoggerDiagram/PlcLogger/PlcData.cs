using System;

namespace LoggerDiagram
{
    public class PlcData
    {
        private RoomNameEnum _roomName;
        private Byte _bytePlc;
        private float _doublePlc;
        private int _time;

        public PlcData(RoomNameEnum roomName, Byte bytePlc, float doublePlc, int time)
        {
            _roomName = roomName;
            _bytePlc = bytePlc;
            _doublePlc = doublePlc;
            _time = time;
        }

        public bool GetIsNewDiagramId()
        {
            if ((int)_bytePlc > 0)
                return true;
            else
                return false;

        }

        public Byte GetByte()
        {
            return _bytePlc;
        }

        public float GetFloat()
        {
            return _doublePlc;
        }

        public RoomNameEnum getNameRoom()
        {
            return _roomName;
        }

        public int GetTime()
        {
            return _time;
        }

    }
}