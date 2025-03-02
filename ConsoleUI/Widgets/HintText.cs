using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class HintText(string message) : IRenderable
{
    private Text RenderText { get; } = new(message, new Style(ConsoleColor.DarkGray));

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RenderText).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RenderText).Render(options, maxWidth);
    }
}