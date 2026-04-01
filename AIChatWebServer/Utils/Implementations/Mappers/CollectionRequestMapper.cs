using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class CollectionRequestMapper<TRequest, TModel>(IRequestMapper<TRequest, TModel> mapper) : ICollectionRequestMapper<TRequest, TModel>
    {
        private readonly IRequestMapper<TRequest, TModel> _mapper = mapper;

        public IReadOnlyCollection<TModel> ToModel(IEnumerable<TRequest> requests)
        {
            List<TModel> models = new List<TModel>();

            foreach (TRequest request in requests)
            {
                models.Add(_mapper.ToModel(request));
            }

            return models;
        }
    }
}
