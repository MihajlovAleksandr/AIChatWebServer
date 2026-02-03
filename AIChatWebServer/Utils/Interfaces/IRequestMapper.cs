namespace AIChatWebServer.Utils.Interfaces
{
    public interface IRequestMapper<TRequest, TModel>
    {
        TModel ToModel(TRequest request);
    }
}
