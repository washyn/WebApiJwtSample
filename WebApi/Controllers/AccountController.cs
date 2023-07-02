using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("account")]
[ApiController]
public class AccountController : ControllerBase
{
    public IActionResult Login(LoginInputViewModel model)
    {
        var data = new LoginOutputViewModel();
        
        return Ok(data);
    }
}

#region Models
public class LoginInputViewModel
{
    public string User { get; set; }
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
    
}

public class UserInformation
{
    
}
#endregion