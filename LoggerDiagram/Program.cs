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
                var application = score.Resolve<ApplicationService>();
                var logger = score.Resolve<ILogger>();

                var cts = new CancellationTokenSource();

                try
                {
                    await application.RunAsync(cts.Token);
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
