using Autofac;
using LoggerDiagram.Application;
using LoggerDiagram.DependencyInjection;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

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

                var cts = new CancellationTokenSource();

                try
                {
                    await application.ProcessAsync(cts.Token);
                }
                catch(ArgumentNullException ex)
                {
                    logger.Error(ex, "Пойман null exception");
                }
                catch (OperationCanceledException ex)
                {
                    logger.Error(ex,"Операция была отменена.");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Неизвестая ошибка.");
                }
            }
        }
    }
}
