using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("book")]
[ApiController]
public class BookController : ControllerBase
{
    public static List<BookViewModel> Data = new List<BookViewModel>();
    
    [Route("{id}")]
    [HttpGet]
    public IActionResult Get(Guid id)
    {
        return Ok(Data.FirstOrDefault(a => a.Id == id));
    }
    
    
    [HttpGet]
    public IActionResult GetList()
    {
        return Ok(Data);
    }
    
    [HttpPost]
    public IActionResult Create(CreateUpdateBookInputModel model)
    {
        Data.Add(new BookViewModel()
        {
            Description = model.Description,
            ISBN = model.ISBN,
            Title = model.Title,
            Id = Guid.NewGuid()
        });
        return Ok();
    }
    
    [Route("{id}")]
    [HttpDelete]
    public IActionResult Remove(Guid id)
    {
        Data.Remove(Data.First(a => a.Id == id));
        return Ok();
    }
    
    [Route("{id}")]
    [HttpPut]
    public IActionResult Update(Guid id,CreateUpdateBookInputModel model)
    {
        var a = Data.First(a => a.Id == id);
        Data.Remove(a);
        Data.Add(new BookViewModel()
        {
            Description = model.Description,
            ISBN = model.ISBN,
            Title = model.Title,
            Id = id
        });
        return Ok();
    }
}

#region Models
public class CreateUpdateBookInputModel : IValidatableObject
{
    [Required]
    public string Title { get; set; }

    [MaxLength(200)]
    public string Description { get; set; }
    
    public string ISBN { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Title.Contains("1"))
        {
            yield return new ValidationResult("No deberia contener el numero 1.", new []{nameof(Title)});
        }
    }
}


public class BookViewModel
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string Title { get; set; }

    [MaxLength(200)]
    public string Description { get; set; }
    
    public string ISBN { get; set; }
}
#endregion