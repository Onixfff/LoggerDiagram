using Autofac;
using LoggerDiagram.DependencyInjection;

namespace LoggerDiagram
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ContainerConfig containerConfig = new ContainerConfig();
            IContainer container = containerConfig.Configure();
        }
    }
}
