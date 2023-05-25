using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheet.Entities;
using System.Data;

namespace TimeSheet.Entities
{
    /// <summary>
    /// Класс представляющий информацию о департаменте
    /// </summary>
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
        /// <summary>
        /// Наименование департамента
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Идентификатор департамента
        /// </summary>
        public int Id { get; private set; }
    }
}

namespace TimeSheet
{
    public partial class DataBase
    {
        /// <summary>
        /// Получить департаменты
        /// </summary>
        /// <returns>Список сущесующих департаментов</returns>
        public static async Task<List<Department>> GetDepartments()
        {
            var data = new List<Department>();

            using (var connection = Connection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM {DepartmentField.TableName}";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var temp = ReadDepartment(reader);
                            data.Add(temp);
                        }
                        reader.Close();
                    }
                }
            }

            return data;
        }
        /// <summary>
        /// Считать запись с информацией о департаменте с пришедшего ответа на SQL запроса
        /// </summary>
        /// <param name="reader">Ответ пришедший с SQL запроса</param>
        /// <returns>Объект с информацией о департаменте</returns>
        private static Department ReadDepartment(DbDataReader reader)
        {
            return new(reader.GetInt32(DepartmentField.Id),
                       reader.GetString(DepartmentField.Name));
        }
    }
}
