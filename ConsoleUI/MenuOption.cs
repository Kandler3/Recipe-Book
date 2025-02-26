using Spectre.Console;

namespace ConsoleUI;

public class MenuOption
{
    private string Title { get; }
    private event Action? OnSelect;
    public void InvokeSelect() => OnSelect?.Invoke();

    public MenuOption(string title, Action? onSelect)
    {
        Title = title;
        OnSelect += onSelect;
    }

    public override string ToString() => Title;
}

