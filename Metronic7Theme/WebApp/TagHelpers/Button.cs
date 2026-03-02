using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApp.TagHelpers;

// revisar como esta hecho del de abp
[HtmlTargetElement("button")]
public class Button : TagHelper
{
    public ButtonType ButtonType { get; set; } = ButtonType.Primary;
    public ButtonSize Size { get; set; } = ButtonSize.Default;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        string size = string.Empty;
        if (Size == ButtonSize.Large)
        {
            size = " btn-lg";
        }

        if (Size == ButtonSize.Small)
        {
            size = " btn-sm";
        }

        output.Attributes.SetAttribute("type", "button");
        output.Attributes.SetAttribute("class", "btn btn-" + ButtonType.ToString().ToLower() + size);
    }
}

public enum ButtonType
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Dark,
    Link
}

// <button type="button" class="btn btn-primary">Primary</button>
// <button type="button" class="btn btn-secondary">Secondary</button>
// <button type="button" class="btn btn-success">Success</button>
// <button type="button" class="btn btn-danger">Danger</button>
// <button type="button" class="btn btn-warning">Warning</button>
// <button type="button" class="btn btn-info">Info</button>
// <button type="button" class="btn btn-light">Light</button>
// <button type="button" class="btn btn-dark">Dark</button>
// <button type="button" class="btn btn-link">Link</button>

// <button type="button" class="btn btn-outline-primary">Primary</button>
// <button type="button" class="btn btn-outline-secondary">Secondary</button>
// <button type="button" class="btn btn-outline-success">Success</button>
// <button type="button" class="btn btn-outline-danger">Danger</button>
// <button type="button" class="btn btn-outline-warning">Warning</button>
// <button type="button" class="btn btn-outline-info">Info</button>
// <button type="button" class="btn btn-outline-dark">Dark</button>

// <button type="button" class="btn btn-primary btn-sm">Small button</button>
// <button type="button" class="btn btn-primary">Default button</button>
// <button type="button" class="btn btn-primary btn-lg">Large button</button>
public enum ButtonSize
{
    Small,
    Default,
    Large,
}