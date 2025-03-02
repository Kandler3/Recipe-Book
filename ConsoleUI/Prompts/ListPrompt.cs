/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Widgets;
using Spectre.Console;

namespace ConsoleUI.Prompts;

/// <summary>
/// Запрашивает ввод списка элементов типа <typeparamref name="T"/> с последующим парсингом.
/// </summary>
/// <typeparam name="T">Тип элемента, который будет возвращен после парсинга строки.</typeparam>
/// <param name="console">Интерфейс консоли для взаимодействия с пользователем.</param>
/// <param name="message">Сообщение для отображения при запросе ввода.</param>
/// <param name="parser">Функция для парсинга введенной строки в тип <typeparamref name="T"/>.</param>
public class ListPrompt<T>(IAnsiConsole console, string message, Func<string, T> parser)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;
    private Func<string, T> Parser { get; } = parser;

    /// <summary>
    /// Запрашивает ввод элементов пользователем до тех пор, пока не будет введена пустая строка.
    /// </summary>
    /// <returns>Список элементов типа <typeparamref name="T"/>, полученных из пользовательского ввода.</returns>
    public List<T> Ask()
    {
        List<T> res = [];

        Console.WriteLine(Message);
        Console.Write(new HintText("Чтобы закончить ввод нажмите Enter два раза\n"));
        while (true)
        {
            var input = Console.Prompt(new TextPrompt<string>("").AllowEmpty());
            if (input == "")
                break;

            try
            {
                res.Add(Parser(input));
            }
            catch (FormatException)
            {
                Console.Write(
                    new Text("Неверный формат ввода. продолжайте\n", new Style(ConsoleColor.DarkYellow))
                );
            }
        }

        return res;
    }
}