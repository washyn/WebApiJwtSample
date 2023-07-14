using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace WebApi.Controllers;

[Route("book")]
[ApiController]
public class BookController : ControllerBase
{
    public static List<BookViewModel> Data = new List<BookViewModel>();

    public BookController()
    {
    }
    
    [Authorize] // Can be use role or policy(politicas o roles)
    [Route("{id}")]
    [HttpGet]
    public IActionResult Get(Guid id)
    {
        return Ok(Data.FirstOrDefault(a => a.Id == id));
    }
    
    [HttpGet]
    public IActionResult GetList()
    {
        var conexionString =
            "Server=(localdb)\\mssqllocaldb;Database=AbpComercial723;Trusted_Connection=True;MultipleActiveResultSets=true";
        using (var conexion = new SqlConnection(conexionString))
        {
            var rowsAfected = conexion.Execute("select * from users");
        }
        
        return Ok(Data);
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    static bool Autenticate(string user, string password)
    {
        string userTEmp;
        string path = "LDAP://cmachyo";
        try
        {
            // TODO: copy example
            using (var directoryEntry = new DirectoryEntry(path, user, password, AuthenticationTypes.Secure))
            {
                using (var directorySearcher = new DirectorySearcher(directoryEntry))
                {
                    var result = directorySearcher.FindOne();
                    // var sr = new SearchResult()
                    return true;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
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
        var a = Data.FirstOrDefault(a => a.Id == id);
        if (a != null)
        {
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
        else
        {
            return NotFound();
        }
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