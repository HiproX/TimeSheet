using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheet.Enums;

namespace TimeSheet
{
    public class Employee
    {
        public Employee()
        {
            Id = 1;
            Name = string.Empty;
            Position = PositionType.Intern;
        }
        public Employee(int id, string name, int departmentId, PositionType position)
        {
            Id = id;
            Name = name;
            DepartmentId = departmentId;
            Position = position;
        }
        public bool IsValidId => Id > 0 && !String.IsNullOrEmpty(Name);
        public int Id { get; private set; }
        public string Name { get; private set; }
        public PositionType Position { get; private set; }
        public int DepartmentId { get; private set; }
    }

    public partial class DataBase
    {
        public static List<Employee> GetEmployeesByDepartment(int department_id)
        {
            var data = new List<Employee>();

            var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM employees WHERE `{EmployeeField.DepartmentId}` = @{EmployeeField.DepartmentId}";
            cmd.Parameters.AddWithValue($"@{EmployeeField.DepartmentId}", department_id);
            cmd.ExecuteNonQuery();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var temp = ReadEmployee(reader);
                    data.Add(temp);
                }
            }

            return data;
        }
        private static Employee ReadEmployee(MySqlDataReader reader)
        {
            return new (reader.GetInt32(EmployeeField.Id),
                        reader.GetString(EmployeeField.Name),
                        reader.GetInt32(EmployeeField.DepartmentId),
                        (PositionType)reader.GetInt32(EmployeeField.Position));
        }
    }
}
