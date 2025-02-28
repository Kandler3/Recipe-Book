using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class HintText(string message) : IRenderable
{
    private Text RenderText { get; } = new(message, new Style(foreground: ConsoleColor.DarkGray));

    public Measurement Measure(RenderOptions options, int maxWidth) => ((IRenderable)RenderText).Measure(options, maxWidth);

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) => ((IRenderable)RenderText).Render(options, maxWidth);
}