using Spectre.Console;

namespace ConsoleUI;

public class ConfirmPrompt(IAnsiConsole console, string message)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;

    private SelectionPrompt<bool> Prompt => new SelectionPrompt<bool>().Title(Message).AddChoices(true, false);

    public bool Ask()
    {
        Console.Clear();
        return Console.Prompt(Prompt);
    }
}