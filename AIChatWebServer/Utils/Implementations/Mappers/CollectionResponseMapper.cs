using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class CollectionResponseMapper<TModel, TResponse>(IResponseMapper<TModel, TResponse> mapper) : ICollectionResponseMapper<TModel, TResponse>
    {
        private readonly IResponseMapper<TModel, TResponse> mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));

        public IReadOnlyCollection<TResponse> ToResponse(IEnumerable<TModel> models)
        {
            List<TResponse> responses = new List<TResponse>();

            foreach (TModel model in models)
            {
                responses.Add(mapper.ToResponse(model));
            }

            return responses;
        }
    }
}
