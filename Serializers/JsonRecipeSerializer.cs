/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using System.Text.Encodings.Web;
using System.Text.Json;
using Contracts.Interfaces;
using Models;

namespace Serializers;

/// <summary>
/// Сериализатор рецептов в формате JSON.
/// </summary>
/// <param name="escapeCyrillic">Если true, используется стандартный кодировщик, иначе — небезопасный режим.</param>
public class JsonRecipeSerializer(bool escapeCyrillic) : IRecipeSerializer, ISingleRecipeSerializer
{
    private JsonSerializerOptions Options { get; } = new()
    {
        Encoder = escapeCyrillic ? JavaScriptEncoder.Default : JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Сериализует коллекцию рецептов в JSON и сохраняет в файл.
    /// </summary>
    /// <param name="recipes">Коллекция рецептов для сериализации.</param>
    /// <param name="outputFilepath">Путь к выходному файлу.</param>
    public void FileSerialize(IEnumerable<Recipe> recipes, string outputFilepath)
    {
        using StreamWriter writer = new(outputFilepath);
        writer.Write(JsonSerializer.Serialize(recipes, Options));
    }

    /// <summary>
    /// Десериализует коллекцию рецептов из JSON-файла.
    /// </summary>
    /// <param name="inputFilepath">Путь к входному файлу.</param>
    /// <returns>Коллекция рецептов.</returns>
    /// <exception cref="FormatException">Выбрасывается, если JSON имеет неверный формат.</exception>
    public IEnumerable<Recipe> FileDeserialize(string inputFilepath)
    {
        using StreamReader reader = new(inputFilepath);
        try
        {
            return JsonSerializer.Deserialize<List<Recipe>>(reader.ReadToEnd(), Options) ?? [];
        }
        catch (JsonException e)
        {
            throw new FormatException("Unable to parse the Json file", e);
        }
    }

    /// <summary>
    /// Десериализует рецепт из строки JSON.
    /// </summary>
    /// <param name="jsonString">Строковое представление рецепта в формате JSON.</param>
    /// <returns>Экземпляр рецепта.</returns>
    /// <exception cref="FormatException">Выбрасывается, если JSON имеет неверный формат.</exception>
    public Recipe DeserializeRecipe(string jsonString)
    {
        try
        {
            return JsonSerializer.Deserialize<Recipe>(jsonString, Options) ??
                   throw new FormatException("Unable to parse the Json file");
        }
        catch (JsonException e)
        {
            throw new FormatException("Unable to parse the Json file", e);
        }
    }
}
