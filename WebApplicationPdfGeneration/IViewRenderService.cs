namespace APIASWAN.Utilidad
{
    public interface IViewRenderService
    {
        Task<string> RederToStringAsync<TModel>(string viewName, TModel model);
    }
}