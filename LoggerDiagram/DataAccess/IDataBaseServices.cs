using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.DataAccess
{
    internal interface IDataBaseServices
    {
        Task SendDataAsync(int idGraph, CancellationToken token);
    }
}
