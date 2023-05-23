using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheet
{
    public class Department
    {
        public Department()
        {
            Id = -1;
            Name = string.Empty;
        }
        public Department(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public bool IsValid => Id != -1 && !String.IsNullOrEmpty(Name);
        public string Name { get; private set; }
        public int Id { get; private set; }
    }

    public partial class DataBase
    {
        public static List<Department> GetDepartments()
        {
            var data = new List<Department>();

            var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM departments";
            cmd.ExecuteNonQuery();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var temp = ReadDepartment(reader);
                    data.Add(temp);
                }
            }

            return data;
        }
        private static Department ReadDepartment(MySqlDataReader reader)
        {
            return new(reader.GetInt32(DepartmentField.Id),
                       reader.GetString(DepartmentField.Name));
        }
    }
}
