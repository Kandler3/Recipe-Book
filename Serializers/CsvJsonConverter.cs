using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Serializers;

public class CsvJsonConverter<T> : DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return JsonSerializer.Deserialize<T>(text);
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return JsonSerializer.Serialize(value);
    }
}