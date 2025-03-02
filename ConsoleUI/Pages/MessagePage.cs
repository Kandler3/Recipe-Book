using ConsoleUI.Widgets;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Pages;

public class MessagePage(IAnsiConsole console, IRenderable message)
{
    private IAnsiConsole Console { get; } = console;
    private IRenderable Message { get; } = message;

    public void Show()
    {
        Console.Clear();
        Console.Write(Message);
        Console.Write(new HintText("Нажмите на любую кнопку чтобы продолжить"));

        System.Console.ReadKey(true);
    }
}