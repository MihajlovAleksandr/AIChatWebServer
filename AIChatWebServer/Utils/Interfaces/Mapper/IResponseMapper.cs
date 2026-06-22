namespace AIChatWebServer.Utils.Interfaces.Mapper
{
    public interface IResponseMapper<TModel, TResponse>
    {
        TResponse ToResponse(TModel model);
    }
}
