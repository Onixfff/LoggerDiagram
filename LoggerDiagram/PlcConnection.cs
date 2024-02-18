using System.Configuration;
using S7.Net;

namespace LoggerDiagram
{
    public class PlcConnection
    {
        private string _ip;
        public PlcConnection()
        {
            
        }
        public Plc GetConnectionEven(string ip) //ConfigurationManager.AppSettings["PlcEven"]
        {
            Plc plc = new Plc(CpuType.S71200,ip ,0,1);
            plc.Open();
            return plc;
        }
    }
}