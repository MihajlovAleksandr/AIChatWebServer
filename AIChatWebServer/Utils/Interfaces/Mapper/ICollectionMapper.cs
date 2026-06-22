namespace AIChatWebServer.Utils.Interfaces.Mapper
{
    public interface ICollectionMapper<TRequest, TModel, TResponse> : ICollectionRequestMapper<TRequest, TModel>, ICollectionResponseMapper<TModel, TResponse>
    {
    }
}
