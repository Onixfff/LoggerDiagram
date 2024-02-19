using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerDiagram.DB
{
    internal class DataBase
    {

        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        MySqlConnection _connection;

        public bool SendData(RoomNameEnum room, float value)
        {
            using (_connection = new MySqlConnection(_connectionString))
            {
                bool isSendMessage = false;
                var isState = _connection.State;
                if (isState == System.Data.ConnectionState.Closed)
                    _connection.Open();
                try
                {
                    DateTime date = DateTime.Now;
                    var sqlDate = date.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    string sql = $"INSERT INTO `diagramrooms`.`{room}` (`idgraph`, `time`, `value`) VALUES('{GetLastDiagramId(room)}', '{sqlDate}', '{value}');";
                    MySqlCommand cmd = new MySqlCommand(sql, _connection);

                    cmd.ExecuteNonQuery();
                    isSendMessage = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    isSendMessage = false;
                }
                finally
                {
                    _connection.Close();
                    if (isSendMessage)
                        Console.WriteLine("Сообщение отправлено");
                    else
                        Console.WriteLine("Сообщение не доставлено");
                }
                return isSendMessage;
            }
        }

        public int GetLastDiagramId(RoomNameEnum roomName)
        {
            int lastIndex = -1;
            using (_connection = new MySqlConnection(_connectionString))
            {
                var isState = _connection.State;
                if (isState == System.Data.ConnectionState.Closed)
                    _connection.Open();

                try
                {
                    string sql = $"SELECT idgraph FROM diagramrooms.{roomName} ORDER BY id DESC LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lastIndex = (int)reader[1];
                    }

                }
                catch (Exception e)
                {

                }
                finally { _connection.Close(); }
                return lastIndex;
            }
        }
    }
}
