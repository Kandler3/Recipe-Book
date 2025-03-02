namespace ConsoleUI.Widgets;

public class MenuOption
{
    public MenuOption(string title, Action? onSelect)
    {
        Title = title;
        OnSelect += onSelect;
    }

    private string Title { get; }
    private event Action? OnSelect;

    public void InvokeSelect()
    {
        OnSelect?.Invoke();
    }

    public override string ToString()
    {
        return Title;
    }
}