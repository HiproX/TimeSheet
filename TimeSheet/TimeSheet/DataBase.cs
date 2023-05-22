using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheet
{
    public class DataBase
    {
        private static readonly Lazy<DataBase> _lazyInstance = new Lazy<DataBase>(new DataBase());
        public static DataBase Instance => _lazyInstance.Value;
        public static bool IsConnected { get; private set; } = false;
        private static string Host { get; } = "localhost";
        private static string? Username { get; } = "TimeSheet";
        private static string? Password { get; } = "12345";
        private static string DataBaseName { get; } = "TimeSheet";
        private static MySqlConnection _connection;
        private DataBase()
        {
            
        }
        private static string GetConnectionString()
        {
            return $"SERVER={Host}; DATABASE={DataBaseName}; UID={Username}; Password={Password}";
        }
        public static void Connect()
        {
            try
            {
                if (string.IsNullOrEmpty(Host) || string.IsNullOrEmpty(DataBaseName))
                {
                    throw new Exception("No database name or host specified!");
                }
                var connectionString = GetConnectionString();
                _connection = new MySqlConnection(connectionString);
                _connection.Open();
                IsConnected = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "MySQL connection error", MessageBoxButtons.OK);
                Environment.Exit(1);
            }
        }
    }
}
