using System;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
// using Volo.Abp.Identity;
using Xunit;

namespace Acme.BookStore.Samples;

/* This is just an example test class.
 * Normally, you don't test code of the modules you are using
 * (like IIdentityUserAppService here).
 * Only test your own application services.
 */
public class SampleAppServiceTests : BookStoreApplicationTestBase
{
    private readonly BookAppService _bookAppService;

    public SampleAppServiceTests()
    {
        _bookAppService = GetRequiredService<BookAppService>();
    }

    [Fact]
    public async Task Initial_Data_Should_Contain_Admin_User()
    {
        //Act
        var result = await _bookAppService.GetListAsync(new PagedAndSortedResultRequestDto());

        //Assert
        result.TotalCount.ShouldBe(0);
        // result.Items.ShouldContain(u => u.UserName == "admin");
    }
    
    [Fact]
    public async Task Create_List()
    {
        //Act
        await _bookAppService.CreateAsync(new CreateUpdateBookDto()
        {
            Name = "tre",
            Price = 34,
            Type = BookType.Adventure,
            PublishDate = DateTime.Today,
        });
        
        var result = await _bookAppService.GetListAsync(new PagedAndSortedResultRequestDto());

        //Assert
        result.TotalCount.ShouldBe(1);
        result.Items.ShouldContain(u => u.Name == "tre");
    }
}
