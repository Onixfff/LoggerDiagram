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

        public async Task<List<PlcaData>> TryTakesData()
        {
            if(plc == null) { return null; }
            List<PlcaData> plcaDatas = new List<PlcaData>();

            try
            {
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph1,
                    (byte)plc.Read(DataType.DataBlock, 1, 0, VarType.Bit, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 1, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph2,
                    (byte)plc.Read(DataType.DataBlock, 1, 2, VarType.Bit, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 3, VarType.Real, 1)));
                plcaDatas.Add(new PlcaData(RoomNameEnum.graph3,
                    (byte)plc.Read(DataType.DataBlock, 1, 0, VarType.Bit, 1),
                    (double)plc.Read(DataType.DataBlock, 1, 1, VarType.Real, 1)));

                //slot.Add((byte)plcEven.Read(DataType.DataBlock, 1, 0, VarType.Bit, 1), 
                //    (double)plcEven.Read(DataType.DataBlock, 1, 1, VarType.Real, 1));
                //slot.Add((byte)plcEven.Read(DataType.DataBlock, 1, 2, VarType.Bit, 1),
                //    (double)plcEven.Read(DataType.DataBlock, 1, 3, VarType.Real, 1));
                //slot.Add((byte)plcEven.Read(DataType.DataBlock, 1, 0, VarType.Bit, 1),
                //    (double)plcEven.Read(DataType.DataBlock, 1, 1, VarType.Real, 1));
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
        }
    }
}
