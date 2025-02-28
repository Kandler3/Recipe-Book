using System.Text.Json.Serialization;
using CsvHelper.Configuration;
using Models;

namespace Serializers;

public class CsvRecipeMap : ClassMap<Recipe>
{
    public CsvRecipeMap()
    {
        Map(r => r.Title);
        Map(r => r.Category);
        Map(r => r.Ingredients).TypeConverter<CsvJsonConverter<List<Ingredient>>>();
        Map(r => r.Instructions).TypeConverter<CsvJsonConverter<List<string>>>();
        Map(r => r.Images).TypeConverter<CsvJsonConverter<List<string>>>();
    }
}