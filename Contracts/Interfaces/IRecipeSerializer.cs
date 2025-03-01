using Models;

namespace Contracts.Interfaces;

public interface IRecipeSerializer
{
    public IEnumerable<Recipe> FileDeserialize(string inputFilepath);
    public void FileSerialize(IEnumerable<Recipe> recipes, string outputFilepath);
}