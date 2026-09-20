using System.Linq.Expressions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPdfDocument;

public static class TableFactory
{
    public static void BuildTable<T>(
        this IContainer container,
        IEnumerable<T> source,
        Action<TableBuilder<T>> configureColumns,
        Action<TableOptions>? configureOptions = null,
        int? totalRows = null)
        where T : class
    {
        var options = new TableOptions();
        configureOptions?.Invoke(options);

        var builder = new TableBuilder<T>();
        configureColumns(builder);

        container.Table(table =>
        {
            table.ColumnsDefinition(definition =>
            {
                if (options.ShowRowNumbers)
                    definition.ConstantColumn(options.RowNumberColumnWidth, Unit.Centimetre);

                foreach (var column in builder.Columns)
                {
                    switch (column.WidthType)
                    {
                        case WidthType.Fixed when column.WidthUnit.HasValue:
                            definition.ConstantColumn(column.WidthValue, column.WidthUnit.Value);
                            break;
                        case WidthType.Fixed:
                            definition.ConstantColumn(column.WidthValue);
                            break;
                        default:
                            definition.RelativeColumn(column.WidthValue);
                            break;
                    }
                }
            });

            if (options.ShowHeader)
            {
                if (options.ShowRowNumbers)
                    RenderHeaderCell(table, "N°", options);

                foreach (var column in builder.Columns)
                {
                    RenderHeaderCell(table, column.Title, options, column.Alignment);
                }
            }

            var rowIndex = 0;
            var list = source.ToList();
            var expectedRows = totalRows ?? list.Count;

            foreach (var element in list)
            {
                rowIndex++;
                var rowBackground = rowIndex % 2 == 0
                    ? options.EvenRowColor
                    : options.OddRowColor;

                if (options.ShowRowNumbers)
                    RenderDataCell(table, rowIndex.ToString(), rowBackground, options, alignment: HorizontalAlignment.Center);

                foreach (var column in builder.Columns)
                {
                    var value = column.GetValue(element);
                    var formattedText = column.Formatter != null
                        ? column.Formatter(value)
                        : value?.ToString() ?? string.Empty;

                    var textColor = column.GetTextColor(element, value);
                    var isBold = column.IsBold(element, value);
                    var fontSize = column.FontSize ?? options.CellFontSize;

                    RenderDataCell(table, formattedText, rowBackground, options,
                        alignment: column.Alignment,
                        textColor: textColor,
                        isBold: isBold,
                        fontSize: fontSize,
                        isUrl: column.IsUrl);
                }
            }

            while (rowIndex < expectedRows)
            {
                rowIndex++;
                var rowBackground = rowIndex % 2 == 0
                    ? options.EvenRowColor
                    : options.OddRowColor;

                if (options.ShowRowNumbers)
                    RenderDataCell(table, rowIndex.ToString(), rowBackground, options,
                        alignment: HorizontalAlignment.Center,
                        textColor: Colors.Grey.Lighten1);

                foreach (var _ in builder.Columns)
                {
                    RenderDataCell(table, string.Empty, rowBackground, options,
                        textColor: Colors.Grey.Lighten1);
                }
            }
        });
    }

    private static void RenderHeaderCell(TableDescriptor table, string text, TableOptions options,
        HorizontalAlignment alignment = HorizontalAlignment.Center)
    {
        var cell = table.Cell()
            .Border(options.BorderThickness)
            .BorderColor(options.BorderColor)
            .Background(options.HeaderBackground)
            .Padding(options.HeaderCellPadding)
            .AlignMiddle();

        cell = alignment switch
        {
            HorizontalAlignment.Left => cell.AlignLeft(),
            HorizontalAlignment.Right => cell.AlignRight(),
            _ => cell.AlignCenter()
        };

        cell.Text(text)
            .Bold()
            .FontSize(options.HeaderFontSize)
            .FontColor(options.HeaderTextColor);
    }

    private static void RenderDataCell(TableDescriptor table, string text, string background, TableOptions options,
        HorizontalAlignment alignment = HorizontalAlignment.Left,
        string? textColor = null, bool isBold = false, float? fontSize = null, bool isUrl = false)
    {
        var cell = table.Cell()
            .Border(options.BorderThickness)
            .BorderColor(options.BorderColor)
            .Background(background)
            .Padding(options.CellPadding)
            .AlignMiddle();

        cell = alignment switch
        {
            HorizontalAlignment.Center => cell.AlignCenter(),
            HorizontalAlignment.Right => cell.AlignRight(),
            _ => cell.AlignLeft()
        };

        var textDescriptor = cell.Text(text)
            .FontSize(fontSize ?? options.CellFontSize);

        if (!string.IsNullOrWhiteSpace(textColor))
            textDescriptor = textDescriptor.FontColor(textColor);

        if (isBold)
            textDescriptor = textDescriptor.Bold();

        if (isUrl)
            textDescriptor = textDescriptor.Italic().Underline();
    }
}

public class TableBuilder<T>
    where T : class
{
    public List<TableColumn<T>> Columns { get; } = new();

    public TableBuilder<T> Column<TProperty>(
        Expression<Func<T, TProperty>> selector,
        string title,
        Action<ColumnOptions<T>>? configure = null)
    {
        var func = selector.Compile();
        object? Selector(T x) => func(x);
        var column = new TableColumn<T>(title, Selector);
        var options = new ColumnOptions<T>();
        configure?.Invoke(options);
        column.ApplyOptions(options);
        Columns.Add(column);
        return this;
    }

    public TableBuilder<T> ConstantColumn(
        string title,
        Func<T, object> selector,
        Action<ColumnOptions<T>>? configure = null)
    {
        var column = new TableColumn<T>(title, x => selector(x));
        var options = new ColumnOptions<T>();
        configure?.Invoke(options);
        column.ApplyOptions(options);
        Columns.Add(column);
        return this;
    }
}

public class TableColumn<T> where T : class
{
    public string Title { get; }
    public Func<T, object?> Selector { get; }
    public WidthType WidthType { get; private set; } = WidthType.Relative;
    public float WidthValue { get; private set; } = 1;
    public Unit? WidthUnit { get; private set; }
    public HorizontalAlignment Alignment { get; internal set; } = HorizontalAlignment.Left;
    public Func<object?, string>? Formatter { get; internal set; }
    public Func<T, object?, string>? TextColorFunc { get; internal set; }
    public Func<T, object?, bool>? BoldFunc { get; internal set; }
    public float? FontSize { get; internal set; }
    public bool IsUrl { get; internal set; }

    public TableColumn(string title, Func<T, object?> selector)
    {
        Title = title;
        Selector = selector;
    }

    public object? GetValue(T element)
    {
        try { return Selector(element); }
        catch { return null; }
    }

    public string GetTextColor(T element, object? value)
    {
        return TextColorFunc?.Invoke(element, value!) ?? Colors.Black;
    }

    public bool IsBold(T element, object? value)
    {
        return BoldFunc?.Invoke(element, value!) ?? false;
    }

    internal void ApplyOptions(ColumnOptions<T> options)
    {
        if (options._width.HasValue)
        {
            WidthType = options._widthType ?? WidthType.Relative;
            WidthValue = options._width.Value;
            WidthUnit = options._widthUnit;
        }
        Alignment = options._alignment;
        Formatter = options._formatter;
        TextColorFunc = options._textColorFunc;
        BoldFunc = options._boldFunc;
        FontSize = options._fontSize;
        IsUrl = options._isUrl;
    }
}

public class ColumnOptions<T> where T : class
{
    internal WidthType? _widthType;
    internal float? _width;
    internal Unit? _widthUnit;
    internal HorizontalAlignment _alignment = HorizontalAlignment.Left;
    internal Func<object?, string>? _formatter;
    internal Func<T, object?, string>? _textColorFunc;
    internal Func<T, object?, bool>? _boldFunc;
    internal float? _fontSize;
    internal bool _isUrl;

    public ColumnOptions<T> RelativeWidth(float proportion = 1)
    {
        _widthType = WidthType.Relative;
        _width = proportion;
        _widthUnit = null;
        return this;
    }

    public ColumnOptions<T> FixedWidth(float value, Unit unit)
    {
        _widthType = WidthType.Fixed;
        _width = value;
        _widthUnit = unit;
        return this;
    }

    public ColumnOptions<T> Aligned(HorizontalAlignment alignment)
    {
        _alignment = alignment;
        return this;
    }

    public ColumnOptions<T> Format(Func<object?, string> formatter)
    {
        _formatter = formatter;
        return this;
    }

    public ColumnOptions<T> TextColor(string staticColor)
    {
        _textColorFunc = (_, _) => staticColor;
        return this;
    }

    public ColumnOptions<T> TextColor(Func<T, object?, string> colorFunc)
    {
        _textColorFunc = colorFunc;
        return this;
    }

    public ColumnOptions<T> Bold()
    {
        _boldFunc = (_, _) => true;
        return this;
    }

    public ColumnOptions<T> Bold(Func<T, object?, bool> condition)
    {
        _boldFunc = condition;
        return this;
    }

    public ColumnOptions<T> FontSize(float size)
    {
        _fontSize = size;
        return this;
    }

    public ColumnOptions<T> AsLink()
    {
        _isUrl = true;
        return this;
    }
}

public class TableOptions
{
    public bool ShowHeader { get; set; } = true;
    public bool ShowRowNumbers { get; set; } = true;
    public float RowNumberColumnWidth { get; set; } = 0.8f;

    public string HeaderBackground { get; set; } = Colors.Grey.Darken2;
    public string HeaderTextColor { get; set; } = Colors.White;
    public float HeaderFontSize { get; set; } = 8;
    public float HeaderCellPadding { get; set; } = 3;

    public string EvenRowColor { get; set; } = Colors.White;
    public string OddRowColor { get; set; } = Colors.Grey.Lighten5;
    public float CellFontSize { get; set; } = 8;
    public float CellPadding { get; set; } = 2;

    public float BorderThickness { get; set; } = 0.5f;
    public string BorderColor { get; set; } = Colors.Grey.Lighten2;
}

public enum WidthType
{
    Relative,
    Fixed
}
