using LoggerDiagram.Models.Plc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public class PlcReaderService : IPlcReaderService
    {
        public async Task<List<PlcLogEntity>> ReadPlcLogsAsync(List<int> ids, IPlcDataReader reader, CancellationToken token)
        {
            var result = new List<PlcLogEntity>();
            int offsetByte = 0;
            int offsetByteNameRoom = 1;
            int offsetDouble = 2;
            int offsetTime = 6;

            foreach (var id in ids)
            {
                var entity = await reader.GetDataWithRetryAsync(offsetByte, offsetByteNameRoom, offsetDouble, offsetTime, token);
                result.Add(entity);

                offsetByte += 8;
                offsetByteNameRoom += 8;
                offsetDouble += 8;
                offsetTime += 8;
            }

            return result;
        }
    }
}
