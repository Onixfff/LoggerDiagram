using S7.Net;
using S7.Net.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoggerDiagram
{
    internal class PLCConnector
    {
        private Plc plc;

        public PLCConnector(string ipEven)
        {
            plc = new Plc(CpuType.S71200, ipEven, 0, 1); //четные 192.168.37.104
            
            //Plc plcOdd = new Plc(CpuType.S71200, ipOdd, 0, 1); //нечетные 192.168.37.103
        }

        public List<PlcaData> TryTakesData()
        {
            if(plc == null) { }
            List<PlcaData> plcaDatas = new List<PlcaData>();

            try
            {
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph1,
                    (byte)plc.Read(DataType.DataBlock, 1, 0, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 1, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph2,
                    (byte)plc.Read(DataType.DataBlock, 1, 2, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 3, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 4, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 5, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph4,
                    (byte)plc.Read(DataType.DataBlock, 1, 6, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 7, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 8, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 9, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 10, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 11, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 12, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 13, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 14, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 15, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 16, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 17, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 18, VarType.Byte, 1), // 10
                    (double)plc.Read(DataType.DataBlock, 1, 19, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 20, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 21, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 22, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 23, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 24, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 25, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 26, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 27, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 28, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 29, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 30, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 31, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 32, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 33, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 34, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 35, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 36, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 37, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 38, VarType.Byte, 1), 
                    (double)plc.Read(DataType.DataBlock, 1, 39, VarType.Real, 1))); //20
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 40, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 41, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 42, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 43, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 44, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 45, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 46, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 47, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 48, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 49, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 50, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 51, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 52, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 53, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 54, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 55, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 56, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 57, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 58, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 59, VarType.Real, 1))); //30
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 60, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 61, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 62, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 63, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 64, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 65, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 66, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 67, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 68, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 69, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 70, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 71, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 72, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 73, VarType.Real, 1))); //37
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

        public void ShowLog(List<PlcaData> plcaDatas)
        {
            foreach (var item in plcaDatas)
            {
                Console.WriteLine($"Имя - {item.getNameRoom()}; Байт - {item.GetByte()}; Данные - {item.GetDouble()};\n");
            }
        }
    }
}
