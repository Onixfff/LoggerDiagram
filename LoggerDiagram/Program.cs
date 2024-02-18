using System;
using System.Threading;

namespace LoggerDiagram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PLCConnector plc1= new PLCConnector("192.168.37.104");
            PLCConnector plc1= new PLCConnector();
            PLCConnector plc2 = new PLCConnector("192.168.37.104");
            while (true)
            {
                try
                {
                    
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
