using System;
using System.Configuration;
using System.Threading;

namespace LoggerDiagram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PLCConnector plc1 = new PLCConnector(ConfigurationManager.AppSettings["PlcEven"]);
            PLCConnector plc2 = new PLCConnector(ConfigurationManager.AppSettings["PlcOdd"]);
            while (true)
            {
                try
                {
                    Thread data1 = new Thread(x=>plc1.ShowLog(plc1.TryTakesData()));
                    data1.Start();
                    plc2.ShowLog(plc2.TryTakesData());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message +"\nСтрока - " + ex.StackTrace);
                    throw;
                }
                finally {Thread.Sleep(5000); }
            }
        }
    }
}
