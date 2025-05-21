using NLog;
using System;
using Autofac;
using System.Threading;
using System.Threading.Tasks;
using LoggerDiagram.Application;
using LoggerDiagram.DependencyInjection;

namespace LoggerDiagram
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            ContainerConfig containerConfig = new ContainerConfig();
            IContainer container = containerConfig.Configure();

            using(var score = container.BeginLifetimeScope())
            {
                var application = score.Resolve<PlcDataProcessor>();
                var logger = score.Resolve<ILogger>();

                TimeSpan interval = TimeSpan.FromSeconds(60); // Интервал между запусками
                var cts = new CancellationTokenSource();

                while (!cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        await application.ProcessAsync(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Неизвестая ошибка.");
                    }

                    try
                    {
                        await Task.Delay(interval, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // Просто выйдем из задержки, если отменено
                    }
                }

                logger.Info("Программа завершила работу.");
            }
        }
    }
}
