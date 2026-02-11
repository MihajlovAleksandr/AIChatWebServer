namespace AIChatWebServer.Utils.Interfaces.Mapper
{
    public interface ICollectionResponseMapper<TModel, TResponse>
    {
        IReadOnlyCollection<TResponse> ToResponse(IEnumerable<TModel> models);
    }
}
