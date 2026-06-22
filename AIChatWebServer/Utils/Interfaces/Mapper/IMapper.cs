namespace AIChatWebServer.Utils.Interfaces.Mapper
{
    public interface IMapper<TRequest, TModel, TResponse> : IRequestMapper<TRequest, TModel>, IResponseMapper<TModel, TResponse>
    {
    }
}
