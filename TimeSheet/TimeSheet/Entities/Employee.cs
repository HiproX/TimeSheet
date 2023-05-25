using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheet.Entities;
using TimeSheet.Enums;
using System.Data;

namespace TimeSheet.Entities
{
    /// <summary>
    /// Класс представляющий информацию о сотруднике
    /// </summary>
    public class Employee
    {
        public Employee()
        {
            Id = 1;
            Surname = FirstName = FathersName = string.Empty;
            Position = PositionType.Intern;
        }
        public Employee(int id, string surname, string firstName, string fathersName, int departmentId, PositionType position)
        {
            Id = id;
            Surname = surname;
            FirstName = firstName;
            FathersName = fathersName;
            DepartmentId = departmentId;
            Position = position;
        }
        public bool IsValidId => Id > 0 && !String.IsNullOrEmpty(Surname);
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; private set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; private set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string FathersName { get; private set; }
        /// <summary>
        ///  Должность
        /// </summary>
        public PositionType Position { get; private set; }
        /// <summary>
        /// Идентификатор департамента
        /// </summary>
        public int DepartmentId { get; private set; }
        /// <summary>
        /// Полное ФИО
        /// </summary>
        public string FullName => $"{Surname} {FirstName} {FathersName}".Trim();
    }
}

namespace TimeSheet
{
    public partial class DataBase
    {
        /// <summary>
        /// Получить работников в депаратаменте
        /// </summary>
        /// <param name="department_id">Идентификатор департамента</param>
        /// <returns>Список работников в департаменте</returns>
        public static async Task<List<Employee>> GetEmployeesByDepartment(int department_id)
        {
            var data = new List<Employee>();

            using (var connection = Connection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM {EmployeeField.TableName} WHERE `{EmployeeField.DepartmentId}` = @{EmployeeField.DepartmentId}";
                    command.Parameters.AddWithValue($"@{EmployeeField.DepartmentId}", department_id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var temp = ReadEmployee(reader);
                            data.Add(temp);
                        }
                        reader.Close();
                    }
                }
            }

            return data;
        }
        /// <summary>
        /// Считать данные работника с пришедшего ответа на SQL запроса
        /// </summary>
        /// <param name="reader">Ответ пришедший с SQL запроса</param>
        /// <returns>Объект с информацией о работнике</returns>
        private static Employee ReadEmployee(DbDataReader reader)
        {
            return new(reader.GetInt32(EmployeeField.Id),
                        reader.GetString(EmployeeField.Surname),
                        reader.GetString(EmployeeField.FirstName),
                        reader.GetString(EmployeeField.FathersName),
                        reader.GetInt32(EmployeeField.DepartmentId),
                        (PositionType)reader.GetInt32(EmployeeField.Position));
        }
    }
}
