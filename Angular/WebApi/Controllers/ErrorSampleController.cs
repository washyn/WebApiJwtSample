using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;

namespace WebApp.Controllers
{
    [RemoteService(isEnabled: true)]
    public class ErrorSampleController : AbpControllerBase
    {
        public ErrorSampleController()
        {
        }

        [HttpGet]
        public void Error500()
        {
            throw new Exception("Internal error.");
        }

        // FIX
        [HttpGet]
        public void Error401()
        {
            throw new AbpAuthorizationException("Un autorized ex");
        }

        [HttpGet]
        public void Error403()
        {
            throw new UserFriendlyException("erro from back");
        }

        [HttpGet]
        public void Error40XXX(ModelSample modelSample)
        {
        }

        [HttpGet]
        public void Error404()
        {
            throw new EntityNotFoundException("No se encontro la entidad.");
        }

        [HttpGet]
        public void ErrorBusinessException()
        {
            throw new BusinessException();
        }


        [HttpGet]
        public async Task<object> LargeRequest()
        {
            await Task.Delay(3000);
            return new { };
        }

        [HttpGet]
        public async Task<object> LargeRequestSecondExample()
        {
            await Task.Delay(5000);
            return new { };
        }

        // FIX
        [Authorize]
        [HttpGet]
        public async Task RequireAuth()
        {
        }

        [HttpGet]
        public void Error400()
        {
            throw new AbpValidationException("message 400");
        }

        [HttpGet]
        public void Error501()
        {
            throw new NotImplementedException();
        }
    }

    public class ModelSample : IValidatableObject
    {
        public string? TestValue { get; set; }
        public string? SecondValue { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(TestValue))
            {
                yield return new ValidationResult("El campo es requerido 1.", new[] { nameof(TestValue) });
            }

            if (string.IsNullOrEmpty(SecondValue))
            {
                yield return new ValidationResult("El campo es requerido 2.", new[] { nameof(SecondValue) });
            }
        }
    }
}
