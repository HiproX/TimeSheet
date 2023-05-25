using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TimeSheet.Entities;

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
        private DataBase()
        {
            
        }
        /// <summary>
        /// Получить строку для подключения к БД
        /// </summary>
        /// <returns></returns>
        private static string GetConnectionString()
        {
            return $"SERVER={Host}; DATABASE={DataBaseName}; UID={Username}; Password={Password};";
        }
        /// <summary>
        /// Создать подключение к БД
        /// </summary>
        /// <returns>Созданное подключение к БД</returns>
        public static MySqlConnection Connection()
        {
            return new MySqlConnection(GetConnectionString());
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
        public static EmployeeField TableName { get; } = new("employees");
        public static EmployeeField Id { get; } = new ("id");
        public static EmployeeField DepartmentId { get; } = new ("department_id");
        public static EmployeeField Surname { get; } = new ("surname");
        public static EmployeeField FirstName { get; } = new("first_name");
        public static EmployeeField FathersName { get; } = new("fathers_name");
        public static EmployeeField Position { get; } = new ("position");
    }

    /// <summary>
    /// Представляет описанией полей БД для Департамента. Наследуется от абстрактного класса TemplateField
    /// </summary>
    public class DepartmentField : TemplateField
    {
        private DepartmentField() : base() { }
        private DepartmentField(string value) : base(value) { }
        public static DepartmentField TableName { get; } = new("departments");
        public static DepartmentField Id { get; } = new ("id");
        public static DepartmentField Name { get; } = new ("name");
    }

    /// <summary>
    /// Представляет описанией полей БД для Производственного календаря. Наследуется от абстрактного класса TemplateField
    /// </summary>
    public class ProductionCalendarField : TemplateField
    {
        private ProductionCalendarField() : base() { }
        private ProductionCalendarField(string value) : base(value) { }
        public static ProductionCalendarField TableName { get; } = new("production_calendar");
        public static ProductionCalendarField Id { get; } = new("id");
        public static ProductionCalendarField Date { get; } = new("date");
        public static ProductionCalendarField Type { get; } = new("type");
    }

    /// <summary>
    /// Представляет описанией полей БД по учету посещений. Наследуется от абстрактного класса TemplateField
    /// </summary>
    public class AttendanceRecordsField : TemplateField
    {
        private AttendanceRecordsField() : base() { }
        private AttendanceRecordsField(string value) : base(value) { }
        public static AttendanceRecordsField TableName { get; } = new("attendance_records");
        public static AttendanceRecordsField Id { get; } = new("id");
        public static AttendanceRecordsField EmployeeId { get; } = new("employee_id");
        public static AttendanceRecordsField Date { get; } = new("date");
        public static AttendanceRecordsField MarkerId { get; } = new("marker_id");
    }

    /// <summary>
    /// Представляет описанией полей БД для видов отметок. Наследуется от абстрактного класса TemplateField
    /// </summary>
    public class MarkersField : TemplateField
    {
        private MarkersField() : base() { }
        private MarkersField(string value) : base(value) { }
        public static MarkersField TableName { get; } = new("markers");
        public static MarkersField Id { get; } = new("id");
        public static MarkersField Code { get; } = new("code");
        public static MarkersField Description { get; } = new("description");
    }
}
