using Autofac;
using LoggerDiagram.Application;
using LoggerDiagram.DataAccess;
using NLog;
using System.Configuration;

namespace LoggerDiagram.DependencyInjection
{
    internal class ContainerConfig
    {
        public IContainer Configure()
        {
            var builder = new ContainerBuilder();

            var connectionString = ConfigurationManager.ConnectionStrings["connLocal"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException("Строка подключения 'connLocal' не найдена в конфигурации.");
            }

            builder.RegisterType<DataBaseRepository>()
                .As<IDataBaseRepository>()
                .WithParameter("connectionString", connectionString);

            builder.Register(c => LogManager.GetCurrentClassLogger())
                .As<ILogger>();

            builder.RegisterType<PlcDataReaderFactory>()
                .As<IPlcDataReaderFactory>();

            builder.RegisterType<ApplicationService>()
                .AsSelf();

            return builder.Build();
        }
    }
}
