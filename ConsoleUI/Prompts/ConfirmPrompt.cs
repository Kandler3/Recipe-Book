/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Spectre.Console;

namespace ConsoleUI.Prompts;

/// <summary>
/// Класс для отображения запроса подтверждения с выбором "Да"/"Нет".
/// </summary>
/// <param name="console">Интерфейс консоли для отображения запроса.</param>
/// <param name="message">Сообщение, отображаемое в запросе.</param>
public class ConfirmPrompt(IAnsiConsole console, string message)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;

    private SelectionPrompt<string> Prompt => new SelectionPrompt<string>()
        .Title(Message)
        .AddChoices("Да", "Нет");

    /// <summary>
    /// Выводит запрос подтверждения и возвращает результат выбора.
    /// </summary>
    /// <returns>True, если пользователь выбрал "Да", иначе false.</returns>
    public bool Ask()
    {
        Console.Clear();
        return Console.Prompt(Prompt).Equals("Да");
    }
}