namespace AIChatWebServer.Models.AI
{
    public class UserAiModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public AIModel Model { get; set; }
        public Guid PaymentItemId { get; set; }
        public DateTime CreatedAt { get; set; }

        public static UserAiModel Create(Guid userId, AIModel aIModel, Guid paymentItemId)
        {
            return new UserAiModel() {
                Id = Guid.NewGuid(),
                UserId = userId,
                Model = aIModel,
                PaymentItemId = paymentItemId,
                CreatedAt = DateTime.UtcNow 
            };
        } 
    }
}