using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Interfaces.Payments;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class ProductAccessFilter(IEnumerable<IProductAccessRule> rules) : IProductAccessFilter
    {
        private readonly IEnumerable<IProductAccessRule> _rules = rules;

        public async Task<bool> ShouldInclude(Product product, Guid userId, CancellationToken ct = default)
        {
            foreach(var rule in _rules)
            {
                if (rule.CanHandle(product.Type))
                {
                    return await rule.Handle(product, userId, ct);
                }
            }
            return true;
        }

        public IProductAccessFilter WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new ProductAccessFilter(_rules.Select(rule => rule.WithTransaction(conn, tx)));
        }
    }
}
