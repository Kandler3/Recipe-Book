/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using CsvHelper.Configuration;
using Models;

namespace Serializers;

/// <summary>
/// Отображение рецепта для CSV-сериализации.
/// </summary>
public class CsvRecipeMap : ClassMap<Recipe>
{
    /// <summary>
    /// Инициализирует правила маппинга для свойств рецепта.
    /// </summary>
    public CsvRecipeMap()
    {
        Map(r => r.Title);
        Map(r => r.Category);
        Map(r => r.Ingredients).TypeConverter<CsvJsonConverter<List<Ingredient>>>();
        Map(r => r.Instructions).TypeConverter<CsvJsonConverter<List<string>>>();
        Map(r => r.Images).TypeConverter<CsvJsonConverter<List<string>>>();
    }
}