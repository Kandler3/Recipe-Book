using ConsoleUI.Widgets;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class ErrorPage(IAnsiConsole console, string message)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;

    public void Show()
    {
        Console.Clear();
        Console.Write(new ErrorText(Message + "\n"));
        Console.Write(new HintText("Нажмите на любую кнопку чтобы продолжить"));

        System.Console.ReadKey(true);
    }
}