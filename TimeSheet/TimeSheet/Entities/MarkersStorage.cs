using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheet.Entities;
using System.Data;
using System.Xml.Linq;

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
            _markersById = new Dictionary<int, Marker>();
            _markersByCode = new Dictionary<string, Marker>();
        }
        /// <summary>
        /// Добавить отметку с кодировкой на работе в хранилище
        /// </summary>
        /// <param name="marker">Отметка с кодировкой на работе</param>
        public void AddMarker(Marker marker)
        {
            _markersById.Add(marker.Id, marker);
            _markersByCode.Add(marker.Code, marker);
        }
        /// <summary>
        /// Получить отметку с кодировкой на работе по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор отметки с кодировкой на работе</param>
        /// <returns></returns>
        public Marker GetMarkerById(int id)
        {
            if (_markersById.ContainsKey(id))
            {
                return _markersById[id];
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
            if (_markersByCode.ContainsKey(code))
            {
                return _markersByCode[code];
            }
            return null; // Отметка с указанным кодом не найдена
        }
        /// <summary>
        /// Очистить локально хранилище существующих видов отметок с кодировкой на работе
        /// </summary>
        public void Clear()
        {
            _markersById.Clear();
            _markersByCode.Clear();
        }
        /// <summary>
        /// Количество записей в хранилище
        /// </summary>
        public int Count => _markersById.Count;
        private Dictionary<int, Marker> _markersById;
        private Dictionary<string, Marker> _markersByCode;
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
                await OpenAsync(connection);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM {MarkersField.TableName}";

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