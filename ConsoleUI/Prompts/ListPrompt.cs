using ConsoleUI.Widgets;
using Spectre.Console;

namespace ConsoleUI.Prompts;

public class ListPrompt<T>(IAnsiConsole console, string message, Func<string, T> parser)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;
    private Func<string, T> Parser { get; } = parser;

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