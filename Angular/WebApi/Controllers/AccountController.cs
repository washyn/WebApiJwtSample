using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Validation;

namespace Unaj.Payment.Public.Controllers
{
    [Route("api/app/account")]
    public class AccountController : AbpControllerBase
    {
        private readonly IOptions<UserOptions> _userOptions;
        private readonly IAuthService _authService;
        private readonly JwtBearerSettings _options;

        public AccountController(IOptions<JwtBearerSettings> options, IOptions<UserOptions> userOptions,
            IAuthService authService)
        {
            _userOptions = userOptions;
            _authService = authService;
            _options = options.Value;
        }

        /// <summary>
        /// Inicio de sesion.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<LoginOutput> Post([FromBody] LoginInput model)
        {
            var authResult = await _authService.AuthenticateAsync(model.User, model.Password);
            if (authResult)
            {
                return new LoginOutput() { AccessToken = CreateAccessToken(model.User) };
            }

            throw new AbpValidationException(new List<ValidationResult>()
            {
                new ValidationResult("El nombre de usuario o la contraseña son incorrectos."),
            });
        }

        /// <summary>
        /// Cerrar sesion.
        /// </summary>
        /// <param name="model"></param>
        [Authorize]
        [Route("logout")]
        [HttpPost]
        public async Task Post([FromBody] LoginOutput model)
        {
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
        }

        private string CreateAccessToken(string userName)
        {
            var id = Guid.Parse("29a558ff-a6d6-40fe-8711-4e09097bfe31").ToString();
            const int expirationMinutes = 5;
            var claims = new List<Claim>();

            claims.Add(new Claim(AbpClaimTypes.UserId, id));
            claims.Add(new Claim(AbpClaimTypes.UserName, "user"));

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
            var siginCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(issuer: _options.Issuer, audience: _options.Audience,
                claims: claims, expires: DateTime.Now.AddHours(8),
                signingCredentials: siginCredentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }

    public class FakeAuthService : IAuthService
    {
        public async Task<bool> AuthenticateAsync(string user, string password)
        {
            return true;
        }
    }

    public interface IAuthService : ITransientDependency
    {
        Task<bool> AuthenticateAsync(string user, string password);
    }

    [DisableAuditing]
    public class LoginInput
    {
        [Required] public string User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class LoginOutput
    {
        [Required] public string AccessToken { get; set; }
    }

    public class JwtBearerSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecurityKey { get; set; }
    }

    public class UserOptions
    {
        public string User { get; set; }
        public string PasswordHash { get; set; }
    }

    public class SwaggerOptions
    {
        public bool IsEnabled { get; set; }
    }
}
