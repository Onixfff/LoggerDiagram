using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace LoggerDiagram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PlcConnector plc1 = new PlcConnector(ConfigurationManager.AppSettings["PlcOdd"]);
            PlcConnector plc2 = new PlcConnector(ConfigurationManager.AppSettings["PlcEven"]);
            while (true)
            {
                try
                {
                    
                    //Получение данных с PLC
                    var data2 = plc2.ShowLog(plc2.TryTakesData());
                    plc2.CheckUpdate(data2);
                    plc2.UpdatOldInfo(data2);

                    var data1 = plc1.ShowLog(plc1.TryTakesData());
                    plc1.CheckUpdate(data1);
                    plc1.UpdatOldInfo(data1);



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message +"\nСтрока - " + ex.StackTrace);
                }
                finally {Thread.Sleep(5000); }
            }
        }
    }
}
