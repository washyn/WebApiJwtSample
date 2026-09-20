using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPdfDocument;

public static class DocumentExtensions
{
    public static void ApplyWatermark(this PageDescriptor page, string text = "CONFIDENTIAL", bool enabled = true)
    {
        if (!enabled) return;

        page.Background()
            .AlignMiddle()
            .AlignCenter()
            .Rotate(-45f)
            .Text(text)
            .Bold()
            .FontSize(60)
            .FontColor(Colors.Red.Lighten4);
    }

    public static void ApplyWatermark(this PageDescriptor page, Action<WatermarkOptions> configure)
    {
        var options = new WatermarkOptions();
        configure(options);

        if (!options.Enabled) return;

        var component = page.Background()
            .AlignMiddle()
            .AlignCenter();

        if (options.Rotation.HasValue)
            component = component.Rotate(options.Rotation.Value);

        component
            .Text(options.Text)
            .Bold()
            .FontSize(options.FontSize)
            .FontColor(options.Color);
    }

    public static void BuildHeader(this PageDescriptor page, Universidad university, float horizontalPadding = 30)
    {
        page.Header()
            .PaddingHorizontal(horizontalPadding)
            .Column(x =>
            {
                x.Item().Row(row =>
                {
                    row.Spacing(10);
                    row.RelativeItem(3)
                        .Width(80)
                        .Image(Convert.FromBase64String(university.LogoBase64));
                    row.RelativeItem(10)
                        .DefaultTextStyle(a => a.FontSize(9))
                        .Column(a =>
                        {
                            a.Item().Text(university.Nombre.ToUpper()).AlignCenter().FontColor(Colors.Grey.Darken1).Bold();
                            a.Item().Text(university.Facultad).AlignCenter().FontColor(Colors.Grey.Darken1);
                            a.Item().Text(university.Escuela).AlignCenter().FontColor(Colors.Grey.Darken1);
                            a.Item().Text(university.Unidad).AlignCenter().FontColor(Colors.Grey.Darken1).Italic();
                        });
                    row.RelativeItem(3)
                        .Width(80)
                        .Image(Convert.FromBase64String(university.LogoBase64));
                });

                x.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);
            });
    }

    public static void BuildFooter(this PageDescriptor page, Universidad university, float horizontalPadding = 30, string? footerMessage = null)
    {
        page.Footer()
            .Column(x =>
            {
                x.Item().PaddingVertical(5)
                    .PaddingHorizontal(horizontalPadding)
                    .LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);

                if (!string.IsNullOrWhiteSpace(footerMessage))
                {
                    x.Item()
                        .PaddingHorizontal(horizontalPadding)
                        .AlignCenter()
                        .Text(footerMessage)
                        .FontSize(8)
                        .Italic()
                        .FontColor(Colors.Grey.Lighten1);
                }

                x.Item()
                    .AlignCenter()
                    .Text($"{university.Direccion} Teléfono {university.Telefono} - {university.Ubicacion}")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);

                x.Item()
                    .AlignRight()
                    .Text(a =>
                    {
                        a.CurrentPageNumber().FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                        a.Span("/").FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                        a.TotalPages().FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                    });
            });
    }

    public static void ConfigureBasePage(this PageDescriptor page, Action<PageOptions>? configure = null)
    {
        var options = new PageOptions();
        configure?.Invoke(options);

        page.Size(options.PageSize);
        page.MarginHorizontal(options.HorizontalMargin, options.MarginUnit);
        page.MarginVertical(options.VerticalMargin, options.MarginUnit);
        page.PageColor(options.BackgroundColor);
        page.DefaultTextStyle(x => x.FontFamily(options.DefaultFont));
        page.DefaultTextStyle(x => x.FontSize(options.DefaultFontSize));
    }
}

public class WatermarkOptions
{
    public bool Enabled { get; set; } = true;
    public string Text { get; set; } = "CONFIDENTIAL";
    public float FontSize { get; set; } = 60;
    public string Color { get; set; } = Colors.Red.Lighten4;
    public float? Rotation { get; set; } = -45f;
}

public class PageOptions
{
    public PageSize PageSize { get; set; } = PageSizes.A4;
    public float HorizontalMargin { get; set; } = 1;
    public float VerticalMargin { get; set; } = 0.75f;
    public Unit MarginUnit { get; set; } = Unit.Centimetre;
    public string BackgroundColor { get; set; } = Colors.White;
    public string DefaultFont { get; set; } = "Calibri";
    public float DefaultFontSize { get; set; } = 11;
}
