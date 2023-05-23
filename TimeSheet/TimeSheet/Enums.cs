using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheet.Enums
{
    /// <summary>
    /// Перечисление должностей
    /// </summary>
    public enum PositionType : int
    {
        [Description("Стажер")]
        Intern,
        [Description("Повар")]
        Cook,
        [Description("IT-Директор")]
        ITDirector,
        [Description("Програмист")]
        Programmer,
    };
    /// <summary>
    /// Перечисление типов дней
    /// </summary>
    public enum DayType : int
    {
        [Description("Рабочий день")]
        WorkingDay,
        [Description("Выходной день")]
        DayOff,
        [Description("Предвыходной день")]
        PreHoliday,
        [Description("Выходной день")]
        Holiday
    };

    /// <summary>
    /// Расширение для enum
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Получить описание для перечисления, если доступно
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        /// <summary>
        /// Получить перечисление по доступному описанию
        /// </summary>
        public static TEnum GetValueFromDescription<TEnum>(string description)
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (TEnum)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (TEnum)field.GetValue(null);
                }
            }

            throw new ArgumentException($"No enum value with description '{description}' found.");
        }
    }
}
