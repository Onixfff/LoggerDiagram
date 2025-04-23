using LoggerDiagram.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task GetLastBatchNumberByGraphAsync()
        {
            // Arrange
            int itemId = 1;
            var connectionString = "Database=diagramrooms; Server=localhost; port=3306; username=root; password=12345; charset=utf8";
            var moqLogger = new Mock<ILogger>();

            moqLogger.Setup(logger => logger.Info(It.IsAny<string>()))
                .Callback<string>(message => Console.WriteLine($"Лог: {message}"));

            DataBaseRepository dataBaseServices = new DataBaseRepository(connectionString, moqLogger.Object);

            // Act
            int result = default;

            var cts = new CancellationTokenSource();

            await Task.Run(async () =>
            {
                result = await dataBaseServices.GetLastBatchNumberByGraphAsync(itemId, cts.Token);
            });

            // Assert
            Assert.IsNotNull(result); // Проверяем, что результат не null
        }
    }
}