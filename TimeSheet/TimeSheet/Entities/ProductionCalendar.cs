using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheet.Enums;

namespace TimeSheet
{
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
        public int Id { get; private set; }
        public DateTime Date { get; private set; }
        public DayType Type { get;private set; }
    }
    public partial class DataBase
    {
        public static List<ProductionCalendar> GetProductionCalendarForMonth(int year, int month)
        {
            var data = new List<ProductionCalendar>();

            var formatedString = $"{year:D4}-{month:D2}";
            var formatForParams = "yyyy-MM";
            var format = "yyyy-MM-dd";

            bool success = DateTime.TryParseExact(formatedString, formatForParams, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
            if (!success) throw new Exception(); // TODO: Create an exception and process it in all code sections

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var lowerBoundDate = new DateTime(year, month, 1).ToString(format);
            var upperBoundDate = new DateTime(year, month, daysInMonth).ToString(format);

            var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM production_calendar WHERE date >= @lowerbound AND date <= @upperbound ORDER BY date ASC";
            cmd.Parameters.AddWithValue($"@lowerbound", lowerBoundDate);
            cmd.Parameters.AddWithValue($"@upperbound", upperBoundDate);
            cmd.ExecuteNonQuery();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var temp = ReadProductionCalendar(reader);
                    data.Add(temp);
                }
            }

            return data;
        }
        private static ProductionCalendar ReadProductionCalendar(MySqlDataReader reader)
        {
            var dateString = reader.GetDateTime(ProductionCalendarField.Date).ToString("yyyy-MM-dd");
            return new(reader.GetInt32(ProductionCalendarField.Id),
                       dateString,
                       (DayType)reader.GetInt32(ProductionCalendarField.Type));
        }
    }
}
