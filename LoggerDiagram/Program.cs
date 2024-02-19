using System;
using System.Threading;

namespace LoggerDiagram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PLCConnector plc1= new PLCConnector("192.168.37.104");
            PLCConnector plc2 = new PLCConnector("192.168.37.103");
            while (true)
            {
                try
                {
                    var data = plc1.TryTakesData();
                    plc1.ShowLog(data);
                    var data2 = plc2.TryTakesData();
                    plc2.ShowLog(data2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message +"\nСтрока - " + ex.StackTrace);
                    throw;
                }
                Thread.Sleep(500);
            }
        }
    }
}
