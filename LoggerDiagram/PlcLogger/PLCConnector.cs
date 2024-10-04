using LoggerDiagram.DB;
using LoggerDiagram.Plc;
using NLog;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram
{
    internal class PlcConnector
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        
        private S7.Net.Plc plc;
        private List<OldPlcData> oldPlcDatas = new List<OldPlcData>();
        private List<byte> bytes = new List<byte>();

        public PlcConnector(string ipEven)
        {
            plc = new PlcConnection(ipEven).GetConnection();
        }

        public async Task<List<PlcData>> TryTakesData(bool isEven)
        {
            List<PlcData> plcaDatas = new List<PlcData>();

            int countRoom = 19;

            try
            {
                await plc.OpenAsync();
            }
            catch(Exception ex)
            {
                logger.Error("Ошибка открытия порта \n" + ex);
                throw new Exception("Ошибка подключения", ex);
            }
            finally { plc.Close(); }

            try
            {
                plcaDatas = await AddPlcData(countRoom, isEven);

                if(isEven)
                    logger.Trace($"Снял данные из 103 ip; Кол-во запросов {countRoom}");
                else
                    logger.Trace($"Снял данные из 104 ip; Кол-во запросов {countRoom}");
            }
            catch (Exception ex)
            {
                //TODO Создать текстовый документ который будет говорить мне что случилось.
                Console.WriteLine(ex.Message + "\n" + ex.Data);

                if(isEven)
                    logger.Error($"Ошибка получения данных из 103 ip;{ex.Message}\n{ex.StackTrace}\n{ex.Data}");
                else
                    logger.Error($"Ошибка получения данных из 104 ip;{ex.Message}\n{ex.StackTrace}\n{ex.Data}");

                throw new Exception("Ошибка подключения", ex);

            }
            finally
            {
                plc.Close();
            }

            return plcaDatas;
        }

        private async Task<List<PlcData>> AddPlcData(int count, bool isEven) //+2
        {
            RoomNameEnum roomNameEnum;

            if (isEven)
                roomNameEnum = 0;
            else
                roomNameEnum = (RoomNameEnum)1;

            List<PlcData> plcaDatas = new List<PlcData>();
            int byteCount = 0;
            int doubleCount = 2;
            int timeCount = 6;

            try
            {
                if (plc.IsConnected != true)
                    await plc.OpenAsync();

                for (int i = 0; i < count; i++)
                {
                    plcaDatas.Add
                        (
                            new PlcData
                            (
                                roomNameEnum,
                                (byte)plc.Read(DataType.DataBlock, 1, byteCount, VarType.Byte, 1),
                                (float)plc.Read(DataType.DataBlock, 1, doubleCount, VarType.Real, 1),
                                (short)plc.Read(DataType.DataBlock,1, timeCount,VarType.Int,1)
                            )
                        );

                    byteCount += 8;
                    doubleCount += 8;
                    timeCount += 8;
                    roomNameEnum += 2;
                }
            }
            catch (PlcException ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.TargetSite);

                if(isEven)
                    logger.Error($"Ошибка получения данных из 103 ip;{ex.Message}\n{ex.StackTrace}\n{ex.Data}");
                else
                    logger.Error($"Ошибка получения данных из 104 ip;{ex.Message}\n{ex.StackTrace}\n{ex.Data}");
            }
            finally
            {
                plc.Close();
            }

            return plcaDatas;
        }

        public List<PlcData> ShowLog(List<PlcData> plcaDatas)
        {
            int coutMoreZero = 0;
            int counZero = 0;
            
            Console.WriteLine("################################################");
            
            foreach (var item in plcaDatas)
            {
                logger.Trace($"Имя - {item.getNameRoom()}; Байт - {item.GetByte()}; Данные - {item.GetFloat()}; Time - {item.GetTime()}");
                Console.WriteLine($"Имя - {item.getNameRoom()}; Байт - {item.GetByte()}; Данные - {item.GetFloat()}; Time - {item.GetTime()}");
                
                if (item.GetByte() > (byte)0)
                    coutMoreZero++;
                else
                    counZero++;
            }
            
            logger.Trace($"Ip - {plc.IP}\nСчётчик равный = 1 ({coutMoreZero})\nСчетчик равный = 0 ({counZero})\nПоток - {Environment.CurrentManagedThreadId}/{Thread.CurrentThread.Name}");
            Console.WriteLine($"Ip - {plc.IP}\nСчётчик равный = 1 ({coutMoreZero})\nСчетчик равный = 0 ({counZero})\nПоток - {Environment.CurrentManagedThreadId}/{Thread.CurrentThread.Name}");
            Console.WriteLine("################################################");
            return plcaDatas;
        }

        public void UpdatOldInfo(List<PlcData> plcaDatas)
        {
            oldPlcDatas.Clear();
            StatusByteEnum statusByteEnum;

            foreach (var item in plcaDatas)
            {
                switch (item.GetByte())
                {
                    case 0:
                        statusByteEnum = StatusByteEnum.Zero;
                        break;
                    case 1:
                        statusByteEnum = StatusByteEnum.One;
                        break;
                    case 22:
                        statusByteEnum = StatusByteEnum.One;
                        break;
                    default:
                        statusByteEnum = StatusByteEnum.Error;
                        break;
                }

                oldPlcDatas.Add(new OldPlcData(item.getNameRoom(), statusByteEnum));
            }
        }

        public void CheckUpdate(List<PlcData> plcaDatas)
        {
            bool isSendMessage;
            DataBase dataBase = new DataBase();

            foreach (var item in plcaDatas)
            {
                switch (item.GetByte())
                {
                    case 0:
                        break;
                    case 1:
                        isSendMessage = dataBase.SendData(GetOldStatusByteEnum(item.getNameRoom()), item.getNameRoom(), item.GetFloat(), item.GetTime());
                        logger.Trace($"Отправка в комнату {item.getNameRoom()} - завершилась ({isSendMessage})");
                        break;
                    case 22:
                        isSendMessage = dataBase.SendData(GetOldStatusByteEnum(item.getNameRoom()), item.getNameRoom(), item.GetFloat(), item.GetTime());
                        logger.Trace($"Отправка в комнату {item.getNameRoom()} - завершилась ({isSendMessage})");
                        break;
                    default:
                        break;
                }
            }

        }

        private StatusByteEnum GetOldStatusByteEnum(RoomNameEnum room)
        {
            StatusByteEnum statusByte = StatusByteEnum.Error;

            foreach (var item in oldPlcDatas)
            {
                if(item.GetRoomName() == room)
                {
                    return statusByte = item.GetStatusByte();
                }
            }

            return statusByte;
        }
    }
}
