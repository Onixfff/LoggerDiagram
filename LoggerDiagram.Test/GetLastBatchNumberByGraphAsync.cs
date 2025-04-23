using LoggerDiagram.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestClass]
public class GetLastBatchNumberByGraphAsync
{
    [TestMethod]
    public async Task GetAllSteamFlowAsync_ShouldReturnDataFromDatabase()
    {
        // Arrange
        var connectionString = "Database=spslogger; Server=localhost; port=3306; username=root; password=12345; charset=utf8";
        DataBaseRepository dataBaseServices = new DataBaseRepository(connectionString);

        // Act
        List<SteamFlow> result = null;
        await Task.Run(async () =>
        {
            result = await dataBaseServices.GetAllSteamFlowAsync();
        });

        // Assert
        Assert.IsNotNull(result); // Проверяем, что результат не null
        Assert.IsTrue(result.Count > 0); // Проверяем, что список не пустой
        Assert.AreEqual(37810, result.Count); // Проверяем количество записей
        Assert.AreEqual(1, result[0].Id); // Проверяем первую запись
        Assert.AreEqual(0, result[0].Data_672); // Проверяем значение Data_672
    }
}