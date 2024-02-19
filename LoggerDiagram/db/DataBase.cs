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

        public DataBase()
        {

        }

        public bool SendData(RoomNameEnum room, int idGraph, DateTime time, string value)
        {
            using (_connection = new MySqlConnection(_connectionString))
            {
                bool isSendMessage = false;
                var isState = _connection.State;
                if (isState == System.Data.ConnectionState.Closed)
                    _connection.Open();
                try
                {
                    var sqlDate = time.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    string sql = $"INSERT INTO `diagramrooms`.`{room}` (`idgraph`, `time`, `value`) VALUES('{idGraph}', '{sqlDate}', '{value}');";
                    MySqlCommand cmd = new MySqlCommand(sql, _connection);

                    int rowCount = cmd.ExecuteNonQuery();
                    Console.WriteLine("Row Count affected = " + rowCount);
                    if (rowCount > 1)
                        isSendMessage = true;
                    else
                        Console.WriteLine("Row Count affected = " + rowCount);
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
    }
}
