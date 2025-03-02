/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI;
using Contracts.Interfaces;
using dotenv.net;
using Serializers;
using Services;
using Spectre.Console;

namespace App;

/// <summary>
/// Класс Program, содержащий точку входа в приложение.
/// </summary>
public static class Program
{
    /// <summary>
    /// Точка входа в приложение. Инициализирует необходимые сервисы и запускает консольное приложение.
    /// </summary>
    public static void Main()
    {
        TxtRecipeSerializer txtSerializer = new();
        JsonRecipeSerializer jsonSerializer = new(false);
        CsvRecipeSerializer csvSerializer = new();
        TxtIngredientSerializer ingredientSerializer = new();

        var envs = DotEnv.Read();

        YandexDiskService yandexDiskService = new(
            envs.TryGetValue("YANDEX_DISK_CLIENT_ID", out var clientId) ? clientId : "",
            ".env"
        );

        IShoppingListService shoppingListService = new ShoppingListService(ingredientSerializer);

        GigaChatService gigaChatService = new(
            envs.TryGetValue("GIGACHAT_TOKEN", out var token) ? token : "",
            jsonSerializer
        );

        IRecipeService recipeService = new RecipeService(
            txtSerializer,
            jsonSerializer,
            csvSerializer,
            yandexDiskService
        );

        ConsoleApp app = new(
            AnsiConsole.Console,
            recipeService,
            shoppingListService,
            yandexDiskService,
            gigaChatService
        );

        app.Run();
    }
}