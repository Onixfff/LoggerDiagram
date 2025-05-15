using LoggerDiagram.Models.Plc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public class PlcReaderService : IPlcReaderService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="reader"></param>
        /// <param name="token"></param>
        /// <returns> <see cref="List{PlcSensorReading}"/></returns>
        public async Task<List<PlcSensorReading>> GetPlcSensorReadingsAsync(List<int> ids, IPlcDataReader reader, CancellationToken token)
        {
            var result = new List<PlcSensorReading>();
            int offsetByte = 0;
            int offsetByteNameRoom = 1;
            int offsetDouble = 2;
            int offsetTime = 6;

            foreach (var id in ids)
            {
                var PlcSensorData = await reader.GetDataWithRetryAsync(offsetByte, offsetByteNameRoom, offsetDouble, offsetTime, token);
                
                if(PlcSensorData != null)
                {
                    result.Add(PlcSensorData);
                }

                offsetByte += 8;
                offsetByteNameRoom += 8;
                offsetDouble += 8;
                offsetTime += 8;
            }

            if (result.Count > 0)
                return result;
            else 
                return null;
        }
    }
}
