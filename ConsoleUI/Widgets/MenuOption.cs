/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

namespace ConsoleUI.Widgets;

/// <summary>
/// Элемент меню с названием и действием при выборе.
/// </summary>
public class MenuOption
{
    /// <summary>
    /// Создает новый элемент меню.
    /// </summary>
    /// <param name="title">Название элемента меню.</param>
    /// <param name="onSelect">Действие, выполняемое при выборе элемента.</param>
    public MenuOption(string title, Action? onSelect)
    {
        Title = title;
        OnSelect += onSelect;
    }

    private string Title { get; }
    private event Action? OnSelect;

    /// <summary>
    /// Вызывает действие, связанное с выбором элемента меню.
    /// </summary>
    public void InvokeSelect()
    {
        OnSelect?.Invoke();
    }

    /// <summary>
    /// Возвращает название элемента меню.
    /// </summary>
    /// <returns>Название элемента меню.</returns>
    public override string ToString()
    {
        return Title;
    }
}