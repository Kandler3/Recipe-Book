/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using System.Globalization;
using Contracts.Interfaces;
using CsvHelper;
using Models;

namespace Serializers;

/// <summary>
/// Сериализатор рецептов в формате CSV.
/// </summary>
public class CsvRecipeSerializer : IRecipeSerializer
{
    /// <summary>
    /// Сериализует коллекцию рецептов в CSV-файл.
    /// </summary>
    /// <param name="recipes">Коллекция рецептов для сериализации.</param>
    /// <param name="outputFilepath">Путь к выходному CSV-файлу.</param>
    public void FileSerialize(IEnumerable<Recipe> recipes, string outputFilepath)
    {
        using StreamWriter streamWriter = new(outputFilepath);
        using CsvWriter writer = new(streamWriter, CultureInfo.InvariantCulture);
        writer.Context.RegisterClassMap<CsvRecipeMap>();
        writer.WriteRecords(recipes);
    }

    /// <summary>
    /// Десериализует коллекцию рецептов из CSV-файла.
    /// </summary>
    /// <param name="inputFilepath">Путь к входному CSV-файлу.</param>
    /// <returns>Коллекция десериализованных рецептов.</returns>
    /// <exception cref="FormatException">Выбрасывается, если CSV-файл имеет неверный формат.</exception>
    public IEnumerable<Recipe> FileDeserialize(string inputFilepath)
    {
        using StreamReader streamReader = new(inputFilepath);
        using CsvReader reader = new(streamReader, CultureInfo.InvariantCulture);
        reader.Context.RegisterClassMap<CsvRecipeMap>();
        try
        {
            return reader.GetRecords<Recipe>().ToList();
        }
        catch (CsvHelperException e)
        {
            throw new FormatException("Unable to parse the CSV file.", e);
        }
    }
}