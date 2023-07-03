using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Controllers;

// DONE
// add app setting values
// add midleware
// token generation
// setting service scheme
// swagger configure for use...

[Route("account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly JwtBearer _options;

    public AccountController(IOptions<JwtBearer> options)
    {
        _options = options.Value;
    }
    
    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    public IActionResult Login(LoginInputViewModel model)
    {
        if (model.User.Equals("string", StringComparison.InvariantCultureIgnoreCase) 
            && model.Password.Equals("string", StringComparison.InvariantCultureIgnoreCase) )
        {
            var token = CreateAccessToken(new UserInformation()
            {
                Name = "a",
                Email = "b",
                Id = "id"
            });
            return Ok(new LoginOutputViewModel()
            {
                Token = new TokenModel()
                {
                    AccessToken = token
                },
                UserInformation = new UserInformation()
                {
                    Name = "nombre"
                }
            });   
        }
        else
        {
            ModelState.AddModelError(nameof(LoginInputViewModel.User), "Valor no valido.");
            ModelState.AddModelError(nameof(LoginInputViewModel.Password), "Valor no valido.");
            return ValidationProblem();
        }
    }
    
    private string CreateAccessToken(UserInformation user)
    {
        var claims = new List<Claim>();
        // NameIdentifier
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        // claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        // claims.Add(new Claim(ClaimTypes.Role, user.RolName)); // can be add multiple roles...
        claims.Add(new Claim(ClaimTypes.Email, user.Email));
        // claims.Add(new Claim(ClaimTypes.Surname, user.Surname));
        claims.Add(new Claim(ClaimTypes.GivenName, user.Name));
        
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
        var siginCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: siginCredentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}

#region Models
public class LoginInputViewModel
{
    [Required]
    public string User { get; set; }
    
    [Required]
    public string Password { get; set; }
}

public class LoginOutputViewModel
{
    public TokenModel Token { get; set; }
    public UserInformation UserInformation { get; set; }

    public LoginOutputViewModel()
    {
        Token = new TokenModel();
        UserInformation = new UserInformation();
    }
}

public class TokenModel
{
    public string AccessToken { get; set; }
}

public class UserInformation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    // public string Rol { get; set; } // can be add
}

public class JwtBearer
{
    public string SecurityKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
#endregion