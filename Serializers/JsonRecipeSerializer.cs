using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Contracts;
using Contracts.Interfaces;
using Models;

namespace Serializers;

public class JsonRecipeSerializer(bool escapeCyrillic) : IRecipeSerializer
{
    private JsonSerializerOptions Options { get; } = new()
    {
        Encoder = escapeCyrillic ? JavaScriptEncoder.Default : JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public void FileSerialize(IEnumerable<Recipe> recipes, string outputFilepath)
    {
        using StreamWriter writer = new(outputFilepath);
        writer.Write(JsonSerializer.Serialize(recipes, Options));
    }

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
}