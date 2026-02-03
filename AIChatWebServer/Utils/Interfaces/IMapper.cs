namespace AIChatWebServer.Utils.Interfaces
{
    public interface IMapper<TRequest, TModel, TResponse> : IRequestMapper<TRequest, TModel>, IResponseMapper<TModel, TResponse>
    {
    }
}
