/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Enums;
using Spectre.Console;

namespace ConsoleUI.Prompts;

/// <summary>
/// Запрос для выбора формата файла.
/// </summary>
/// <param name="console">Консоль.</param>
/// <param name="message">Сообщение для отображения при запросе формата.</param>
public class SelectFormatPrompt(IAnsiConsole console, string message)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;

    /// <summary>
    /// Выводит запрос выбора формата файла и возвращает выбранный формат.
    /// </summary>
    /// <returns>Выбранный формат файла.</returns>
    public FileFormat Ask()
    {
        Console.Clear();
        return Console.Prompt(
            new SelectionPrompt<FileFormat>().AddChoices(
                FileFormat.Txt,
                FileFormat.Csv,
                FileFormat.Json
            )
        );
    }
}