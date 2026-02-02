using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebAppEnum.Ebooks;
// NOTA: lo ideal seria usar string cuando sea necesario
// el enum se puede mapear a string en base de datos
// para el front se puede usar el attributo de [Display(Name = "En espera")] sobre el member del enum para mostrar en el front
public class Ebook
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;


    // custom attribute for enum validation is in range of enum values
    // por defecto no se valida por que se puede usar los enums como flags
    [EnumValid]
    [BindProperty]
    [EnumDataType(typeof(BookType))]
    public BookType Type { get; set; }
    public DateTime PublishDate { get; set; }
    public float Price { get; set; }
}

public enum BookType
{
    [Display(Name = "No definido")] // localization name
    Undefined,
    Adventure,
    Biography,
    Dystopia,
    Fantastic,
    Horror,
    Science,
    ScienceFiction,
    Poetry
}


public class EnumValidAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
        => value != null && Enum.IsDefined(value.GetType(), value);
}
