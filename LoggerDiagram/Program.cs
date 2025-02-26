﻿using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram
{
    internal class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Task.Run(async () => await RunAsync()).GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            PlcConnector plc1 = new PlcConnector(ConfigurationManager.AppSettings["PlcEven"]);
            PlcConnector plc2 = new PlcConnector(ConfigurationManager.AppSettings["PlcOdd"]);

            while (true)
            {
                try
                {
                    //Получение данных с PLC
                    var data2 = plc2.ShowLog(await plc2.TryTakesData(true));
                    plc2.CheckUpdate(data2);
                    plc2.UpdatOldInfo(data2);

                    logger.Trace("PlcOdd завершил выполнение кода\n\n");

                    var data1 = plc1.ShowLog(await plc1.TryTakesData(false));
                    plc1.CheckUpdate(data1);
                    plc1.UpdatOldInfo(data1);

                    logger.Trace("PlcEven завершил выполнение кода\n\n");

                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + "Строка -" + ex.StackTrace);
                    Console.WriteLine(ex.Message + "\nСтрока - " + ex.StackTrace);
                }
                finally { Thread.Sleep(5000); }
            }
        }
    }
}
