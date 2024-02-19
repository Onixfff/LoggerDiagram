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
            PlcConnector plc1 = new PlcConnector(ConfigurationManager.AppSettings["PlcEven"]);
            PlcConnector plc2 = new PlcConnector(ConfigurationManager.AppSettings["PlcOdd"]);
            while (true)
            {
                try
                {
                    //List<PlcaData> data1 = new List<PlcaData>();
                    //Thread thread = new Thread(() => {plc1.ShowLog(data1 = plc1.TryTakesData()); });
                    //ThreadPool.QueueUserWorkItem(() => { data1 = plc1.TryTakesData(); });
                    
                    //Получение данных с PLC
                    var data1 = plc1.ShowLog(plc1.TryTakesData());
                    var data2 = plc2.ShowLog(plc2.TryTakesData());

                    //Проверка данных и составление данных для старого списка
                    plc1.CheckUpdate(data1);
                    plc2.CheckUpdate(data2);


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
