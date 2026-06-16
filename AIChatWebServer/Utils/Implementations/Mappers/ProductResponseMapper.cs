using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class ProductResponseMapper : IResponseMapper<Product, ProductResponse>
    {
        public ProductResponse ToResponse(Product model)
        {
            return new ProductResponse(model.Id, model.Name, model.Type, model.Price, model.Currency, model.Description);
        }
    }
}
