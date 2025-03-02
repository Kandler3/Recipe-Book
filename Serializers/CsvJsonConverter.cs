/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using System.Text.Encodings.Web;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Serializers;

/// <summary>
/// Преобразователь для конвертации объектов в JSON и обратно при работе с CSV.
/// </summary>
/// <typeparam name="T">Тип объекта для сериализации и десериализации.</typeparam>
public class CsvJsonConverter<T> : DefaultTypeConverter
{
    /// <summary>
    /// Параметры сериализации JSON.
    /// </summary>
    private JsonSerializerOptions Options { get; } = new()
    {
        // Позволяет сохранять кириллицу в читаемом виде.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Десериализует строку JSON в объект типа T.
    /// </summary>
    /// <returns>Объект типа T, десериализованный из JSON.</returns>
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return JsonSerializer.Deserialize<T>(text, Options);
    }

    /// <summary>
    /// Сериализует объект в строку JSON.
    /// </summary>
    /// <returns>Строка JSON, представляющая объект.</returns>
    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return JsonSerializer.Serialize(value, Options);
    }
}