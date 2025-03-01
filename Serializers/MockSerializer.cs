using Contracts;
using Contracts.Interfaces;
using Models;

namespace Serializers;

public class MockSerializer : IRecipeSerializer
{
    public void FileSerialize(IEnumerable<Recipe> recipes, string outputFilepath)
    {
        
    }

    public IEnumerable<Recipe> FileDeserialize(string inputFilepath)
    {
        return [];
    }
}