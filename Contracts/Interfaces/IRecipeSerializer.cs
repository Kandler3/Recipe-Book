using Models;

namespace Contracts;

public interface IRecipeSerializer
{
    public IEnumerable<Recipe> Deserialize(string inputFilepath);
    public void Serialize(IEnumerable<Recipe> recipes, string outputFilepath);
}