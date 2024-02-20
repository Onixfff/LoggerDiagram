using LoggerDiagram.DB;
using LoggerDiagram.Plc;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace LoggerDiagram
{
    internal class PlcConnector
    {
        private S7.Net.Plc plc;
        private List<OldPlcData> oldPlcDatas = new List<OldPlcData>();
        private List<byte> bytes = new List<byte>();

        public PlcConnector(string ipEven)
        {
            plc = new PlcConnection(ipEven).GetConnection();
        }

        public List<PlcData> TryTakesData()
        {
            List<PlcData> plcaDatas = new List<PlcData>();

            try
            {
                lock (plc)
                {
                    plc.Open();
                    plcaDatas = AddPlcData(19);
                    
                }
            }
            catch (Exception ex)
            {
                //TODO Создать текстовый документ который будет говорить мне что случилось.
                Console.WriteLine(ex.Message + "\n" + ex.Data);
            }
            finally
            {
                plc.Close();
            }

            return plcaDatas;
        }

        private List<PlcData> AddPlcData(int count) //+2
        {
            RoomNameEnum roomNameEnum = 0;
            List<PlcData> plcaDatas = new List<PlcData>();
            int byteCount = 0;
            int doubleCount = 2;
            int timeCount = 6;
            try
            {
                if (plc.IsConnected != true)
                    plc.Open();

                for (int i = 0; i < count; i++)
                {
                    plcaDatas.Add
                        (
                            new PlcData
                            (
                                roomNameEnum,
                                (byte)plc.Read(DataType.DataBlock, 1, byteCount, VarType.Byte, 1),
                                (float)plc.Read(DataType.DataBlock, 1, doubleCount, VarType.Real, 1),
                                (short)plc.Read(DataType.DataBlock,1, timeCount,VarType.Int,1)
                            )
                        );

                    byteCount += 8;
                    doubleCount += 8;
                    timeCount += 8;
                    roomNameEnum++;
                }
            }
            catch (PlcException ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.TargetSite);
            }
            finally
            {
                plc.Close();
            }

            return plcaDatas;
        }

        public List<PlcData> ShowLog(List<PlcData> plcaDatas)
        {
            int coutOne = 0;
            int counZero = 0;
            Console.WriteLine("################################################");
            foreach (var item in plcaDatas)
            {
                Console.WriteLine($"Имя - {item.getNameRoom()}; Байт - {item.GetByte()}; Данные - {item.GetFloat()}; Time - {item.GetTime()}");
                if (item.GetByte() > (byte)0)
                    coutOne++;
                else
                    counZero++;
            }

            Console.WriteLine($"Ip - {plc.IP}\nСчётчик равный = 1 ({coutOne})\nСчетчик равный = 0 ({counZero})\nПоток - {Environment.CurrentManagedThreadId}/{Thread.CurrentThread.Name}");
            Console.WriteLine("################################################");
            return plcaDatas;
        }

        public void UpdatOldInfo(List<PlcData> plcaDatas)
        {
            oldPlcDatas.Clear();
            StatusByteEnum statusByteEnum;

            foreach (var item in plcaDatas)
            {
                switch (item.GetByte())
                {
                    case 0:
                        statusByteEnum = StatusByteEnum.Zero;
                        break;
                    case 1:
                        statusByteEnum = StatusByteEnum.One;
                        break;
                    case 22:
                        statusByteEnum = StatusByteEnum.One;
                        break;
                    default:
                        statusByteEnum = StatusByteEnum.Error;
                        break;
                }

                oldPlcDatas.Add(new OldPlcData(item.getNameRoom(), statusByteEnum));
            }
        }

        public void CheckUpdate(List<PlcData> plcaDatas)
        {
            bool isSendMessage = false;
            DataBase dataBase = new DataBase();

            foreach (var item in plcaDatas)
            {
                switch (item.GetByte())
                {
                    case 0:
                        break;
                    case 1:
                        isSendMessage = dataBase.SendData(GetOldStatusByteEnum(item.getNameRoom()), item.getNameRoom(), item.GetFloat(), item.GetTime());
                        break;
                    case 22:
                        isSendMessage =dataBase.SendData(GetOldStatusByteEnum(item.getNameRoom()), item.getNameRoom(), item.GetFloat(), item.GetTime());
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine($"isSendMessage - {isSendMessage}");
        }

        private StatusByteEnum GetOldStatusByteEnum(RoomNameEnum room)
        {
            StatusByteEnum statusByte = StatusByteEnum.Error;
            foreach (var item in oldPlcDatas)
            {
                if(item.GetRoomName() == room)
                {
                    return statusByte = item.GetStatusByte();
                }
            }
            return statusByte;
        }
    }
}
