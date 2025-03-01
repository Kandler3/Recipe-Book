using Models;

namespace Contracts.Interfaces;

public interface IGigaChatService
{
    public Recipe GenerateRecipe(string prompt);
}