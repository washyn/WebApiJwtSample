using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

namespace Acme.BookStore.Controllers;

[Area("abp")]
[RemoteService(Name = "abp")]
[Route("api/abp/application-localization")]
public class AbpApplicationLocalizationController : AbpControllerBase
{
    public AbpApplicationLocalizationController()
    {
    }
    [HttpGet]
    public virtual async Task<ApplicationLocalizationDto> GetAsync(ApplicationLocalizationRequestDto input)
    {
        return new ApplicationLocalizationDto();
    }
}

#region DTOS
public class ApplicationLocalizationRequestDto
{
    [Required]
    public string CultureName { get; set; }
    
    public bool OnlyDynamics { get; set; }
}

[Serializable]
public class ApplicationLocalizationDto
{
    public Dictionary<string, ApplicationLocalizationResourceDto> Resources { get; set; }

    public ApplicationLocalizationDto()
    {
        Resources = new Dictionary<string, ApplicationLocalizationResourceDto>();
    }
}

[Serializable]
public class ApplicationLocalizationResourceDto
{
    public Dictionary<string, string> Texts { get; set; }
    
    public string[] BaseResources { get; set; }
}
#endregion
