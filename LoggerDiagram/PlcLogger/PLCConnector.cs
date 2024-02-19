using S7.Net;
using System;
using System.Collections.Generic;

namespace LoggerDiagram
{
    internal class PLCConnector
    {
        private S7.Net.Plc plc;

        public PLCConnector(string ipEven)
        {
            plc = new PlcConnection(ipEven).GetConnection();
        }

        public List<PlcaData> TryTakesData()
        {
            List<PlcaData> plcaDatas = new List<PlcaData>();

            try
            {
                lock (plc)
                {
                    plc.Open();
                    var plcDatas = AddPlcData(19);
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

        private List<PlcaData> AddPlcData(int count) //+2
        {
            List<PlcaData> plcaDatas = new List<PlcaData>();
            int byteCount = 0;
            int doubleCount = 2;
            try
            {
                if (plc.IsConnected != true)
                    plc.Open();

                for (int i = 0; i < count; i++)
                {
                    plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, byteCount, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, doubleCount, VarType.Real, 1)));
                    byteCount += 2;
                    doubleCount += 2;
                }
            }
            catch(PlcException ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.TargetSite);
            }
            finally 
            { 
                plc.Close();
            }

            return plcaDatas;
        }

        public void ShowLog(List<PlcaData> plcaDatas)
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

            Console.WriteLine($"Ip - {plc.IP}\nСчётчик равный = 1 ({coutOne})\nСчетчик равный = 0 ({counZero})\nПоток - {Environment.CurrentManagedThreadId}");
            Console.WriteLine("################################################");
        }
    }
}
