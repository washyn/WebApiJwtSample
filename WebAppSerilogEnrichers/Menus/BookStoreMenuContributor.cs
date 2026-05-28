using Volo.Abp.UI.Navigation;
using System.Threading.Tasks;

namespace Washyn.BookStore.Menus
{
  public class BookStoreMenuContributor : IMenuContributor
  {
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
      if (context.Menu.Name == StandardMenus.Main)
      {
        await ConfigureMainMenuAsync(context);
      }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
      context.Menu.Items.Insert(
        0,
        new ApplicationMenuItem(
          BookStoreMenus.Home,
          "Home",
          "~/",
          icon: "fas fa-home",
          order: 0
        )
      );

      context.Menu.AddItem(
        new ApplicationMenuItem(
          "BooksStore",
          "BookStore",
          icon: "fa fa-book"
        ).AddItem(
          new ApplicationMenuItem(
            "BooksStore.Books",
            "Books",
            url: "/Books"
          )
        )
      );

      return Task.CompletedTask;
    }
  }
}