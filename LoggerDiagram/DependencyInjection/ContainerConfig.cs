using Autofac;
using LoggerDiagram.Application;
using LoggerDiagram.DataAccess;
using LoggerDiagram.Services;
using LoggerDiagram.Services.Interfaces;
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

            //Repository
            builder.RegisterType<DataBaseRepository>()
                .As<IDataBaseRepository>()
                .WithParameter("connectionString", connectionString)
                .InstancePerLifetimeScope();

            //Logger
            builder.Register(c => LogManager.GetCurrentClassLogger())
                .As<ILogger>()
                .SingleInstance();

            //
            builder.RegisterType<PlcDataReaderFactory>()
                .As<IPlcDataReaderFactory>()
                .InstancePerLifetimeScope();

            //Запуск программы(входной процесс)
            builder.RegisterType<PlcDataProcessor>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<PlcReaderService>()
                .As<IPlcReaderService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PlcDataSender>()
                .As<IPlcDataSender>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PlcDataReader>()
                .As<IPlcDataReader>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PlcDataConverter>()
                .As<IPlcDataConverter>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GraphIdSplitter>()
                .As<IGraphIdSplitter>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BatchNumberAdjuster>()
                .As<IBatchNumberAdjuster>()
                .InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
