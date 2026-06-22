namespace AIChatWebServer.Utils.Interfaces.Mapper
{
    public interface ICollectionRequestMapper<TRequest, TModer>
    {
        IReadOnlyCollection<TModer> ToModel(IEnumerable<TRequest> requests);
    }
}
