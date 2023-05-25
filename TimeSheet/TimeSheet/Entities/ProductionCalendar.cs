using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheet.Entities;
using TimeSheet.Enums;
using System.Data;

namespace TimeSheet.Entities
{
    /// <summary>
    /// Класс описывающий производственный календарь
    /// </summary>
    public class ProductionCalendar
    {
        public ProductionCalendar()
        {
            Id = -1;
        }
        public ProductionCalendar(int id, string date, DayType type)
        {
            bool success = DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            if (!success) throw new Exception($"Date {date} {ProductionCalendarField.Id}={id} is invalid!");
            Id = id;
            Date = dateTime;
            Type = type;
        }
        public bool IsValid => Id != -1;
        /// <summary>
        /// Идентификатор записи в производственном календаре
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; private set; }
        /// <summary>
        /// Тип даты
        /// </summary>
        public DayType Type { get;private set; }
    }
}

namespace TimeSheet
{
    public partial class DataBase
    {
        /// <summary>
        /// Получить производственный календарь на месяц
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц в диапазоне [1, 12]</param>
        /// <returns>Список с записями производственного календарь</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<List<ProductionCalendar>> GetProductionCalendarForMonth(int year, int month)
        {
            var data = new List<ProductionCalendar>();

            using (var connection = Connection())
            {
                await connection.OpenAsync();

                var formatedString = $"{year:D4}-{month:D2}";
                var formatForParams = "yyyy-MM";
                var format = "yyyy-MM-dd";

                bool success = DateTime.TryParseExact(formatedString, formatForParams, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                if (!success) throw new Exception(); // TODO: создать исключение и обработку в коде

                var daysInMonth = DateTime.DaysInMonth(year, month);
                var lowerBoundDate = new DateTime(year, month, 1).ToString(format);
                var upperBoundDate = new DateTime(year, month, daysInMonth).ToString(format);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM {ProductionCalendarField.TableName} WHERE date >= @lowerbound AND date <= @upperbound ORDER BY date ASC";
                    command.Parameters.AddWithValue($"@lowerbound", lowerBoundDate);
                    command.Parameters.AddWithValue($"@upperbound", upperBoundDate);
                    await command.ExecuteNonQueryAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var temp = ReadProductionCalendar(reader);
                            data.Add(temp);
                        }
                        reader.Close();
                    }
                }
            }

            return data;
        }
        /// <summary>
        /// Считать запись производственного календаря с пришедшего ответа на SQL запроса
        /// </summary>
        /// <param name="reader">Ответ пришедший с SQL запроса</param>
        /// <returns>Запись производственного календаря</returns>
        private static ProductionCalendar ReadProductionCalendar(DbDataReader reader)
        {
            var dateString = reader.GetDateTime(ProductionCalendarField.Date).ToString("yyyy-MM-dd");
            return new(reader.GetInt32(ProductionCalendarField.Id),
                       dateString,
                       (DayType)reader.GetInt32(ProductionCalendarField.Type));
        }
    }
}
