using System.Configuration;
using S7.Net;

namespace LoggerDiagram
{
    public class PlcConnection
    {
        private string _ip;
        public PlcConnection(string ip)
        {
            _ip = ip;
        }

        public S7.Net.Plc GetConnection() //ConfigurationManager.AppSettings["PlcEven"]
        {
            S7.Net.Plc plc = new S7.Net.Plc(CpuType.S71200, _ip ,0,1);
            return plc;
        }
    }
}