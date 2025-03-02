/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Models;

namespace Contracts.Interfaces;

/// <summary>
/// Интерфейс сервиса для генерации рецептов с использованием GigaChat.
/// </summary>
public interface IGigaChatService
{
    /// <summary>
    /// Генерирует рецепт на основе заданного запроса.
    /// </summary>
    /// <param name="prompt">Запрос для генерации рецепта.</param>
    /// <returns>Сгенерированный рецепт.</returns>
    public Recipe GenerateRecipe(string prompt);
}