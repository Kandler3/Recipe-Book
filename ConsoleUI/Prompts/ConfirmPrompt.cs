using Spectre.Console;

namespace ConsoleUI.Prompts;

public class ConfirmPrompt(IAnsiConsole console, string message)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;

    private SelectionPrompt<string> Prompt => new SelectionPrompt<string>().Title(Message).AddChoices("Да", "Нет");

    public bool Ask()
    {
        Console.Clear();
        return Console.Prompt(Prompt).Equals("Да");
    }
}