namespace AIChatWebServer.Repositories.Constants
{
    public static class UserQueries
    {
        public const string CreateUser = @"
INSERT INTO users (id, email, region_code, registration_state)
VALUES (@id, @email, @regionCode, 0);
";

        public const string AddAuthIdentity = @"
INSERT INTO auth_identities (id, user_id, provider_code, identifier, secret, created_at)
VALUES (@id, @userId, @providerCode, @identifier, @secret, NOW());
";

        public const string SaveUserData = @"
INSERT INTO user_data (id, user_id, name, gender, age)
VALUES (@id, @userId, @name, @gender, @age)
ON CONFLICT (user_id)
DO UPDATE SET name=@name, gender=@gender, age=@age;
";

        public const string SavePreference = @"
INSERT INTO preferences (id, user_id, min_age, max_age, preferred_gender)
VALUES (@id, @userId, @minAge, @maxAge, @preferredGender)
ON CONFLICT (user_id)
DO UPDATE SET min_age=@minAge, max_age=@maxAge, preferred_gender=@preferredGender;
";

        public const string UpdateRegistrationState = @"
UPDATE users SET registration_state = @state WHERE id = @userId;
";


        public const string SaveLanguage = @"
INSERT INTO user_languages (id, user_id, context, language_code, last_update)
VALUES (@id, @userId, @context, @languageCode, CURRENT_TIMESTAMP)
ON CONFLICT (user_id, context)
DO UPDATE SET 
    language_code = @languageCode,
    last_update = CURRENT_TIMESTAMP;";

        public const string GetRegistrationState = @"
SELECT registration_state FROM users WHERE id=@userId;
";

        public const string ExistsByEmail = @"
SELECT COUNT(1) FROM users WHERE email=@email;
";

        public const string DeleteUser = @"
DELETE FROM users WHERE id=@userId;
";

        public const string UpdateUser = @"
UPDATE users SET email=@email, region_code=@regionCode WHERE id=@id;
";

        public const string GetRegionByCode = @"
SELECT code, name FROM regions WHERE code=@regionCode;
";

        public const string GetUserBanById = @"
SELECT id, user_id, reason, reason_category, banned_at, banned_until
FROM users_bans
WHERE user_id=@userId
ORDER BY banned_at DESC
LIMIT 1;
";

        public const string GetUserFullBase = @"
SELECT
 u.id                     AS user_id,
 u.email,
 u.region_code,
 u.registration_state,

 ud.id                    AS user_data_id,
 ud.name                  AS user_name,
 ud.gender                AS user_gender,
 ud.age                   AS user_age,

 p.id                     AS pref_id,
 p.min_age,
 p.max_age,
 p.preferred_gender,

 up.id                    AS premium_id,
 up.start_at,
 up.end_at,

 ai.id                    AS auth_id,
 ai.identifier,
 ai.secret,
 ai.created_at,
 ap.code                  AS provider_code,
 ap.description           AS provider_description,

 ul.context               AS lang_context,
 ul.language_code

FROM users u
LEFT JOIN user_data ud       ON ud.user_id = u.id
LEFT JOIN preferences p      ON p.user_id = u.id
LEFT JOIN users_premium up   ON up.user_id = u.id
LEFT JOIN auth_identities ai ON ai.user_id = u.id
LEFT JOIN auth_providers ap  ON ap.code = ai.provider_code
LEFT JOIN user_languages ul  ON ul.user_id = u.id
";

        public const string GetUserById =
            GetUserFullBase + " WHERE u.id=@userId;";

        public const string GetUserByEmail =
            GetUserFullBase + " WHERE u.email=@email;";

        public const string GetUserByAuthIdentity =
            GetUserFullBase + @"
INNER JOIN auth_identities ai2 ON ai2.user_id=u.id
WHERE ai2.provider_code=@providerCode AND ai2.identifier=@identifier;
";

        public const string GetUsersByAuthProvider =
            GetUserFullBase + @"
INNER JOIN auth_identities ai2 ON ai2.user_id=u.id
WHERE ai2.provider_code=@providerCode;
";

        public const string GetUsersInSameChats =
            GetUserFullBase + @"
INNER JOIN user_chats uc1 ON uc1.user_id=@userId
INNER JOIN user_chats uc2 ON uc1.chat_id=uc2.chat_id AND uc2.user_id!=@userId
WHERE u.id=uc2.user_id;
";
    }
}
