/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Widgets;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница отображения сообщения с возможностью продолжения после нажатия клавиши.
/// </summary>
/// <param name="console">Консольный интерфейс для вывода сообщения.</param>
/// <param name="message">Рендерируемое сообщение, отображаемое на странице.</param>
public class MessagePage(IAnsiConsole console, IRenderable message)
{
    private IAnsiConsole Console { get; } = console;
    private IRenderable Message { get; } = message;

    /// <summary>
    /// Отображает сообщение и ожидает нажатия клавиши для продолжения.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        Console.Write(Message);
        Console.Write(new HintText("Нажмите на любую кнопку чтобы продолжить"));

        System.Console.ReadKey(true);
    }
}