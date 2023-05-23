using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TimeSheet
{
    /// <summary>
    /// Представляет класс для взаимодействия с СУБД MySQL
    /// </summary>
    public partial class DataBase
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
        /// <summary>
        /// Подключиться к БД
        /// </summary>
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
        /// <summary>
        /// Отключиться от БД
        /// </summary>
        public static void Close()
        {
            IsConnected = false;
            _connection.Close();
        }
        public static void InsertProductionCalendar()
        {
            var jsonString = File.ReadAllText("production_calendar.json");
            var productionCalendar = JsonConvert.DeserializeObject<List<dynamic>>(jsonString) ?? null;
            if (productionCalendar != null)
            {
                foreach (var day in productionCalendar)
                {
                    var command = _connection.CreateCommand();
                    command.CommandText = $"INSERT INTO production_calendar ({ProductionCalendarField.Date}, {ProductionCalendarField.Type}) VALUES " +
                        $"(@{ProductionCalendarField.Date}, @{ProductionCalendarField.Type})";
                    command.Parameters.AddWithValue($"{ProductionCalendarField.Date}", day.Date);
                    command.Parameters.AddWithValue($"{ProductionCalendarField.Type}", day.DateType);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    /// <summary>
    /// Представляет абстрактный класс для описания полей БД
    /// </summary>
    public abstract class TemplateField
    {
        protected TemplateField() { }
        protected TemplateField(string value) => Value = value;
        public string Value { get; }
        public override string ToString()
        {
            return Value;
        }
        public static implicit operator string(TemplateField field)
        {
            return field.Value;
        }
    }

    /// <summary>
    /// Представляет описанией полей БД для Сотрудника. Наследуется от абстрактного класса TemplateField
    /// </summary>
    public class EmployeeField : TemplateField
    {
        private EmployeeField() : base() { }
        private EmployeeField(string value) : base(value) { }
        public static EmployeeField Id { get; } = new ("employee_id");
        public static EmployeeField DepartmentId { get; } = new ("department_id");
        public static EmployeeField Name { get; } = new ("employee_name");
        public static EmployeeField Position { get; } = new ("employee_position");
        public override string ToString()
        {
            return base.ToString();
        }
        public static implicit operator string(EmployeeField field)
        {
            return field.Value;
        }
    }

    /// <summary>
    /// Представляет описанией полей БД для Департамента. Наследуется от абстрактного класса TemplateField
    /// </summary>
    public class DepartmentField : TemplateField
    {
        private DepartmentField() : base() { }
        private DepartmentField(string value) : base(value) { }
        public static DepartmentField Id { get; } = new ("department_id");
        public static DepartmentField Name { get; } = new ("department_name");
        public override string ToString()
        {
            return base.ToString();
        }
        public static implicit operator string(DepartmentField field)
        {
            return field.Value;
        }
    }

    /// <summary>
    /// Представляет описанией полей БД для Производственного календаря. Наследуется от абстрактного класса TemplateField
    /// </summary>
    public class ProductionCalendarField : TemplateField
    {
        private ProductionCalendarField() : base() { }
        private ProductionCalendarField(string value) : base(value) { }
        public static ProductionCalendarField Id { get; } = new("date_id");
        public static ProductionCalendarField Date { get; } = new("date");
        public static ProductionCalendarField Type { get; } = new("date_type");
        public override string ToString()
        {
            return base.ToString();
        }
        public static implicit operator string(ProductionCalendarField field)
        {
            return field.Value;
        }
    }
}
