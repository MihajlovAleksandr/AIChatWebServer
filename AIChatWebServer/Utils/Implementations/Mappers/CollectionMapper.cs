using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class CollectionMapper<TRequest, TModel, TResponse>(IMapper<TRequest, TModel, TResponse> mapper) : ICollectionMapper<TRequest, TModel, TResponse>
    {
        private readonly IMapper<TRequest, TModel, TResponse> mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));

        public IReadOnlyCollection<TModel> ToModel(IEnumerable<TRequest> requests)
        {
            List<TModel> models = new List<TModel>();

            foreach (TRequest request in requests)
            {
                models.Add(mapper.ToModel(request));
            }

            return models;
        }

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
