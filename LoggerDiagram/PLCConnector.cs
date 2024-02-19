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
            List<PlcaData> plcaDatas = new List<PlcaData>();
            try
            {
                var plcDatas = AddPlcData(0, 74, 37);
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
            finally { plc.Close(); }
            return plcaDatas;
        }

        public void ShowLog(List<PlcaData> plcaDatas)
        {
            int coutOne = 0;
            int counZero = 0; 
            foreach (var item in plcaDatas)
            {
                Console.WriteLine($"Имя - {item.getNameRoom()}; Байт - {item.GetByte()}; Данные - {item.GetDouble()};\n");
                if (item.GetByte() > (byte)0)
                    coutOne++;
                else
                    counZero++;
            }
            Console.WriteLine($"Счётчик больше 0 = {coutOne} Счетчик == 0 = {counZero}");
        }
    }
}
