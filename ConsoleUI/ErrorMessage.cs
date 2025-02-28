using ConsoleUI.Widgets;
using Spectre.Console;

namespace ConsoleUI;

public class ErrorMessage(IAnsiConsole console, string message)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;

    public void Show()
    {
        Console.Clear();
        Console.Write(new Text(Message + "\n", new Style(foreground: ConsoleColor.Red)));
        Console.Write(new HintText("Нажмите на любую кнопку чтобы продолжить"));

        System.Console.ReadKey(true);
    }
}