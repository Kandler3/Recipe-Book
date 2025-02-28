using Contracts;
using Models;

namespace Serializers;

public class MockSerializer : IRecipeSerializer
{
    public void Serialize(IEnumerable<Recipe> recipes, string outputFilepath)
    {
        
    }

    public IEnumerable<Recipe> Deserialize(string inputFilepath)
    {
        return [];
    }
}