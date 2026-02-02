using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumExtensions
{
    public static IEnumerable<SelectListItem> GetEnumSelectListByName<TEnum>()
        where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => new SelectListItem
            {
                Value = e.ToString(), // 👈 STRING en vez de número
                Text = GetDisplayName(e)
            });
    }

    private static string GetDisplayName<TEnum>(TEnum enumValue)
    {
        var member = typeof(TEnum).GetMember(enumValue.ToString()).First();
        var displayAttr = member.GetCustomAttribute<DisplayAttribute>();
        return displayAttr?.Name ?? enumValue.ToString();
    }
}

public static class HtmlEnumExtensions
{
    public static IEnumerable<SelectListItem> GetEnumSelectListAsString<TEnum>(this IHtmlHelper html)
        where TEnum : Enum
        => EnumExtensions.GetEnumSelectListByName<TEnum>();
}

