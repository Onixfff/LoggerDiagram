using S7.Net;
using S7.Net.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LoggerDiagram
{
    internal class PLCConnector
    {
        private Plc plc;

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
                    var plcDatas = AddPlcData(0, 74, 37);
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

        private List<PlcaData> AddPlcData(int from, int before, int count)
        {
            List<PlcaData> plcaDatas = new List<PlcaData>();

            try
            {
                if (plc.IsConnected != true)
                    plc.Open();

                for (int i = 0; i < count; i++)
                {
                    plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 70, VarType.Byte, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 71, VarType.Real, 1)));
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
