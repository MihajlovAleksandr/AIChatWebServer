namespace AIChatWebServer.Repositories.Constants
{
    public static class UserAiModelQueries
    {
        public const string Insert = @"
            INSERT INTO user_ai_models (id, user_id, model, payment_item_id, created_at)
            VALUES (@id, @user_id, @model, @payment_item_id, @created_at)";

        public const string GetById = @"
            SELECT id, user_id, model, payment_item_id, created_at
            FROM user_ai_models
            WHERE id = @id";

        public const string GetByUserIdAndModel = @"
            SELECT id, user_id, model, payment_item_id, created_at
            FROM user_ai_models
            WHERE user_id = @user_id AND model = @model";

        public const string ExistsByUserIdAndModel = @"
            SELECT EXISTS(
                SELECT 1 
                FROM user_ai_models 
                WHERE user_id = @user_id AND model = @model
            )";

        public const string GetAllByUserId = @"
            SELECT id, user_id, model, payment_item_id, created_at
            FROM user_ai_models
            WHERE user_id = @user_id
            ORDER BY created_at DESC";
    }
}