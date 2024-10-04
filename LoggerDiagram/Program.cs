using System;
using System.Configuration;
using System.Threading;

namespace LoggerDiagram
{
    internal class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            PlcConnector plc1 = new PlcConnector(ConfigurationManager.AppSettings["PlcEven"]);
            PlcConnector plc2 = new PlcConnector(ConfigurationManager.AppSettings["PlcOdd"]);
            while (true)
            {
                try
                {  
                    //Получение данных с PLC
                    
                    var data2 = plc2.ShowLog(plc2.TryTakesData(true));
                    plc2.CheckUpdate(data2);
                    plc2.UpdatOldInfo(data2);

                    logger.Debug("PlcOdd завершил выполнение кода\n\n");

                    var data1 = plc1.ShowLog(plc1.TryTakesData(false));
                    plc1.CheckUpdate(data1);
                    plc1.UpdatOldInfo(data1);

                    logger.Debug("PlcEven завершил выполнение кода\n\n");

                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + "Строка -" + ex.StackTrace);
                    Console.WriteLine(ex.Message +"\nСтрока - " + ex.StackTrace);
                }
                finally {Thread.Sleep(5000); }
            }
        }
    }
}
