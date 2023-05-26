using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TimeSheet.Entities;
using TimeSheet.Enums;
using System.Data;

namespace TimeSheet.Entities
{
    /// <summary>
    /// Класс представляющий хранилище данных об учете посещаемости сотрудников
    /// </summary>
    public class AttendanceRecordsStorage
    {
        public AttendanceRecordsStorage()
        {
            _data = new();
        }
        /// <summary>
        /// Добавить запись
        /// </summary>
        /// <param name="employeeId">Идентификатор сотрудника</param>
        /// <param name="day">Номер дня месяца</param>
        /// <param name="markerId">Идентификатор вида отметки с кодировкой на работе</param>
        public void Add(int employeeId, int day, int markerId)
        {
            _data.Add(new(employeeId, day), markerId);
        }
        /// <summary>
        /// Получить идентификатор отметки с кодировкой на работе о сотрудника
        /// </summary>
        /// <param name="employee">Сотрудника о котором нужно получить информацию по учету посещаемости</param>
        /// <param name="day">День месяца в который нужно получить информацию по учету посещаемости</param>
        /// <returns>Идентификатор отметки с кодировкой на работе. Если отметка не найдена вернет -1</returns>
        public int GetMarkerId(Employee employee, int day)
        {
            if (_data.TryGetValue(new(employee.Id, day), out int markerId))
            {
                return markerId;
            }
            return -1;
        }
        /// <summary>
        /// Очистить локально хранилище данных об учете посещаемости сотрудников
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }
        /// <summary>
        /// Количество записей в хранилище
        /// </summary>
        public int Count => _data.Count;
        private Dictionary<KeyValuePair<int, int>, int> _data;
    }
}

namespace TimeSheet
{
    public partial class DataBase
    {
        /// <summary>
        /// Получить журнал учета посещаемости
        /// </summary>
        /// <param name="year">Год для филтрации журнала</param>
        /// <param name="month">Номер месяца для фильтрации журнала в диапазоне [1, 12]</param>
        /// <param name="employees">Список работников о которых нужно получить информацию о посещении</param>
        /// <returns></returns>
        /// <exception cref="Exception">Если в переданной дате допущена ошибка</exception>
        public static async Task<AttendanceRecordsStorage> GetAttendanceRecords(int year, int month, List<Employee> employees)
        {
            var data = new AttendanceRecordsStorage();

            var formatedString = $"{year:D4}-{month:D2}";
            var formatForParams = "yyyy-MM";
            var format = "yyyy-MM-dd";

            bool success = DateTime.TryParseExact(formatedString, formatForParams, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
            if (!success) throw new Exception(); // TODO: Create an exception and process it in all code sections

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var lowerBoundDate = new DateTime(year, month, 1).ToString(format);
            var upperBoundDate = new DateTime(year, month, daysInMonth).ToString(format);

            using (var connection = Connection())
            {
                await OpenAsync(connection);

                foreach (var employee in employees)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT * FROM {AttendanceRecordsField.TableName} WHERE {AttendanceRecordsField.Date} >= @lowerbound AND {AttendanceRecordsField.Date} <= @upperbound " +
                            $"AND {AttendanceRecordsField.EmployeeId} = @{AttendanceRecordsField.EmployeeId}";
                        command.Parameters.AddWithValue($"@lowerbound", lowerBoundDate);
                        command.Parameters.AddWithValue($"@upperbound", upperBoundDate);
                        command.Parameters.AddWithValue($"@{AttendanceRecordsField.EmployeeId}", employee.Id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                dynamic temp = ReadAttendanceRecord(reader);
                                data.Add(temp.EmployeeId, temp.DateTime.Day, temp.MarkerId);
                            }
                            reader.Close();
                        }
                    }
                }
            }

            return data;
        }
        /// <summary>
        /// Считать запись о посещении с пришедшего ответа на SQL запроса
        /// </summary>
        /// <param name="reader">Ответ пришедший с SQL запроса</param>
        /// <returns>Объект с информацией о посещении содрудника</returns>
        private static dynamic ReadAttendanceRecord(DbDataReader reader)
        {
            var dateTime = reader.GetDateTime(AttendanceRecordsField.Date);
            return new
            {
                Id = reader.GetInt32(AttendanceRecordsField.Id),
                EmployeeId = reader.GetInt32(AttendanceRecordsField.EmployeeId),
                DateTime = dateTime,
                MarkerId = reader.GetInt32(AttendanceRecordsField.MarkerId)
            };
        }
    }
}
