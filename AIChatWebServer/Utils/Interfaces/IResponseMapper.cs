namespace AIChatWebServer.Utils.Interfaces
{
    public interface IResponseMapper<TModel, TResponse>
    {
        TResponse ToResponse(TModel model);
    }
}
