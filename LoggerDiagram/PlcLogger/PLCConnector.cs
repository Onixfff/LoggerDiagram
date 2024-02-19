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
            try
            {
                if (plc.IsConnected != true)
                    plc.Open();

                for (int i = 0; i < count; i++)
                {
                    plcaDatas.Add(new PlcData(roomNameEnum,
                    (byte)plc.Read(DataType.DataBlock, 1, byteCount, VarType.Byte, 1),
                    (float)plc.Read(DataType.DataBlock, 1, doubleCount, VarType.Real, 1)));
                    byteCount += 6;
                    doubleCount += 6;
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
                Console.WriteLine($"Имя - {item.getNameRoom()}; Байт - {item.GetByte()}; Данные - {item.GetDouble()};\n");
                if (item.GetByte() > (byte)0)
                    coutOne++;
                else
                    counZero++;
            }

            Console.WriteLine($"Ip - {plc.IP}\nСчётчик равный = 1 ({coutOne})\nСчетчик равный = 0 ({counZero})\nПоток - {Environment.CurrentManagedThreadId}/{Thread.CurrentThread.Name}");
            Console.WriteLine("################################################");
            return plcaDatas;
        }

        public void CheckUpdate(List<PlcData> plcaDatas)
        {
            DataBase dataBase = new DataBase();

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
                        //dataBase.SendData();
                        break;
                    default:
                        statusByteEnum = StatusByteEnum.Error;
                        break;
                }

                oldPlcDatas.Add(new OldPlcData(item.getNameRoom(), statusByteEnum));
            }
        }
    }
}
