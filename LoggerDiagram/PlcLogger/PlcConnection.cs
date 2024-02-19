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

        public Plc GetConnection() //ConfigurationManager.AppSettings["PlcEven"]
        {
            Plc plc = new Plc(CpuType.S71200, _ip ,0,1);
            return plc;
        }
    }
}