using Autofac;

namespace LoggerDiagram.DependencyInjection
{
    internal class ContainerConfig
    {
        public IContainer Configure()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<>().As<>();

            return builder.Build();
        }
    }
}
