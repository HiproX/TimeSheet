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
    /// Класс представляющий отметку с кодировкой на работе
    /// </summary>
    public class Marker
    {
        public Marker(int id, string code, string description)
        {
            Id = id;
            Code = code;
            Description = description;
        }
        /// <summary>
        /// Идентификатор отметки
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Кодировка отметки
        /// </summary>
        public string Code { get; private set; }
        /// <summary>
        /// Описание кодировки отметки
        /// </summary>
        public string Description { get; private set; }
    }

    /// <summary>
    /// Класс представляющий хранилище для существующих видов отметок с кодировкой на работе
    /// (предварительно отметки нужно загружать из БД)
    /// </summary>
    public class MarkerStorage
    {
        public MarkerStorage()
        {
            markersById = new Dictionary<int, Marker>();
            markersByCode = new Dictionary<string, Marker>();
        }
        /// <summary>
        /// Добавить отметку с кодировкой на работе в хранилище
        /// </summary>
        /// <param name="marker">Отметка с кодировкой на работе</param>
        public void AddMarker(Marker marker)
        {
            markersById.Add(marker.Id, marker);
            markersByCode.Add(marker.Code, marker);
        }
        /// <summary>
        /// Получить отметку с кодировкой на работе по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор отметки с кодировкой на работе</param>
        /// <returns></returns>
        public Marker GetMarkerById(int id)
        {
            if (markersById.ContainsKey(id))
            {
                return markersById[id];
            }
            return null; // Отметка с указанным id не найдена
        }
        /// <summary>
        /// Получить отметку с кодировкой на работе по кодировке
        /// </summary>
        /// <param name="code">Кодировка отметки с кодировкой на работе</param>
        /// <returns></returns>
        public Marker GetMarkerByCode(string code)
        {
            if (markersByCode.ContainsKey(code))
            {
                return markersByCode[code];
            }
            return null; // Отметка с указанным кодом не найдена
        }
        /// <summary>
        /// Очистить локально хранилище существующих видов отметок с кодировкой на работе
        /// </summary>
        public void Clear()
        {
            markersById.Clear();
            markersByCode.Clear();
        }
        private Dictionary<int, Marker> markersById;
        private Dictionary<string, Marker> markersByCode;
    }
}

namespace TimeSheet
{
    public partial class DataBase
    {
        /// <summary>
        /// Получить доступные виды отметок с кодировкой на работе
        /// </summary>
        /// <returns>Хранилище с доступными видами отметок с кодировкой на работе</returns>
        public static async Task<MarkerStorage> GetMarkers()
        {
            var data = new MarkerStorage();

            using (var connection = Connection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM {MarkersField.TableName}";
                    await command.ExecuteNonQueryAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var temp = ReadMarker(reader);
                            data.AddMarker(temp);
                        }
                        reader.Close();
                    }
                }
            }

            return data;
        }
        /// <summary>
        /// Считать запись отметки с кодировкой на работе с пришедшего ответа на SQL запроса
        /// </summary>
        /// <param name="reader">Ответ пришедший с SQL запроса</param>
        /// <returns>Отметка с кодировкой на работе</returns>
        private static Marker ReadMarker(DbDataReader reader)
        {
            return new(reader.GetInt32(MarkersField.Id),
                       reader.GetString(MarkersField.Code),
                       reader.GetString(MarkersField.Description));
        }
    }
}