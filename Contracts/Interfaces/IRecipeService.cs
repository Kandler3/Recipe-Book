﻿using Contracts.Enums;
using Models;

namespace Contracts.Interfaces;

public interface IRecipeService
{
    public IList<Recipe> GetRecipes(IRecipesQuery query);
    public IEnumerable<string> GetCategories();
    public IEnumerable<string> GetIngredients();
    public void AddRecipe(Recipe recipe);
    public void DeleteRecipe(Recipe recipe);
    public void AddRecipeImage(Recipe recipe, string imagePath);
    public void Import(string filepath, FileFormat format, bool local = true);
    public void Export(string filepath, FileFormat format, IRecipesQuery? query = null, bool local = true);
}