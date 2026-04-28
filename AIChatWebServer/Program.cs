using AIChatWebServer.Hubs.Implementations;
using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Integrations.AI.Configuration;
using AIChatWebServer.Integrations.AI.Implementations;
using AIChatWebServer.Integrations.AI.Interfaces;
using AIChatWebServer.Integrations.Email.Implementations;
using AIChatWebServer.Integrations.Email.Interfaces;
using AIChatWebServer.Middlewares;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Repositories.Implementations;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Context.Implementations;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Implementations;
using AIChatWebServer.Services.Implementations.AI;
using AIChatWebServer.Services.Implementations.Chats;
using AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator;
using AIChatWebServer.Services.Implementations.Chats.Matchmaking;
using AIChatWebServer.Services.Implementations.Chats.Matchmaking.Predicates;
using AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies;
using AIChatWebServer.Services.Implementations.Chats.RandomChatGame;
using AIChatWebServer.Services.Implementations.Messages;
using AIChatWebServer.Services.Implementations.Messages.Processors;
using AIChatWebServer.Services.Implementations.Notifications;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;
using AIChatWebServer.Services.Interfaces.Messages;
using AIChatWebServer.Services.Interfaces.Notifications;
using AIChatWebServer.Utils.Errors;
using AIChatWebServer.Utils.Implementations;
using AIChatWebServer.Utils.Implementations.Mappers;
using AIChatWebServer.Utils.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMappers();

var redisConnectionString =
    builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException("Redis connection string is not configured.");

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.Configure<DeepSeekSettings>(
    builder.Configuration.GetSection("AISettings:DeepSeek"));

builder.Services.Configure<OllamaSettings>(
    builder.Configuration.GetSection("AISettings:Ollama"));

builder.Services.Configure<AISettings>(
    builder.Configuration.GetSection("AISettings"));

builder.Services.AddSingleton<BackgroundJobService>();
builder.Services.AddSingleton<IBackgroundJobService>(sp => sp.GetRequiredService<BackgroundJobService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<BackgroundJobService>());

builder.Services.AddHttpClient<DeepSeekController>();
builder.Services.AddHttpClient<OllamaController>();
builder.Services.AddScoped<IAIControllerFactory, AIControllerFactory>();

builder.Services.AddScoped<IAIMessageSender, AIMessageSender>();
builder.Services.AddScoped<IQueryTagParser, QueryTagParser>();
builder.Services.AddScoped<IQueryClassifier, QueryClassifier>();
builder.Services.AddScoped<IUserProfileStore, RedisUserProfileStore>();
builder.Services.AddScoped<IUserProfileGenerator, UserProfileGenerator>();

builder.Services.AddScoped<IAIMessageCompressor, AIMessageCompressor>();

builder.Services.AddScoped<IAIMessageDispatcherFactory, AIMessageDispatcherFactory>();

builder.Services.AddScoped<IAIService, AIService>();

builder.Services.AddScoped<IDialogAnalysisParser, DialogAnalysisParser>();


builder.Services.AddSingleton<IHasher, Hasher>();
builder.Services.AddSingleton<ITokenReplayGuard, RedisTokenReplayGuard>();
builder.Services.AddSingleton<FirebaseApp>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    string path = configuration["Google:FirebasePath"]
        ?? throw new ArgumentException("Google:FirebasePath is not configured.");

    var credential = CredentialFactory
        .FromFile<ServiceAccountCredential>(path)
        .ToGoogleCredential();

    var app = FirebaseApp.Create(new AppOptions()
    {
        Credential = credential
    });

    return app;
});
builder.Services.AddScoped<IMessageNotificationService, FirebaseMessageNotificationService>();
builder.Services.AddScoped<INotificationSender, NotificationSender>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserContextAccessor, UserContextAccessor>();
builder.Services.AddSingleton<IConnectionStore, ConnectionStore>();

builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IMessageDispatcher, MessageDispatcher>();
builder.Services.AddScoped<IChatEventsDispatcher, ChatEventsDispatcher>();
builder.Services.AddScoped<IHubGroupDispatcher, HubGroupDispatcher>();

builder.Services.AddScoped<IStringChanger, StringChanger>();
builder.Services.AddScoped<IHtmlContentBuilder, HtmlContentBuilder>();

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailTextGetter, EmailTextGetter>();
builder.Services.AddScoped<IVerificationCodeSender, VerificationCodeSender>();
builder.Services.AddScoped<NotificationSettingsMapper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
builder.Services.AddScoped<IConnectionRepository, ConnectionRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddScoped<IMatchmakingRepository, MatchmakingRepository>();
builder.Services.AddScoped<IGroupChatSearchRepository, GroupChatSearchRepository>();
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IUploadSessionRepository, UploadSessionRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IAIMessageRepository, AIMessageRepository>();
builder.Services.AddScoped<IAISettingsRepository, AISettingsRepository>();
builder.Services.AddScoped<IChatGameRepository, ChatGameRepository>();
builder.Services.AddScoped<IUserProfileStore, RedisUserProfileStore>();

builder.Services.AddScoped<IAuthLoginService, AuthLoginService>();
builder.Services.AddScoped<IAuthRegistrationService, AuthRegistrationService>();
builder.Services.AddScoped<IAuthOAuthService, AuthOAuthService>();

builder.Services.AddScoped<IAISettingsService, AISettingsService>();
builder.Services.AddScoped<RandomMessageProcessor>();
builder.Services.AddScoped<AIChatMessageProcessor>();
builder.Services.AddScoped<IChatGameService, ChatGameService>();
builder.Services.AddScoped<IMessageVisibilityPolicy, MessageVisibilityPolicy>();
builder.Services.AddScoped<IUploadSessionTtlCalculator, UploadSessionTtlCalculator>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IConnectionEventDispatcher, ConnectionEventDispatcher>();
builder.Services.AddScoped<IChatGroupEventDispatcher, ChatGroupEventDispatcher>();
builder.Services.AddScoped<IMessageEventDispatcher, MessageEventDispatcher>();
builder.Services.AddScoped<IConnectionNotifier, ConnectionNotifier>();
builder.Services.AddScoped<IChatGroupNotifier, ChatGroupNotifier>();
builder.Services.AddScoped<IMessageNotifier, MessageNotifier>();
builder.Services.AddScoped<IGeoIpService, MaxMindGeoIpService>();
builder.Services.AddScoped<IRegionGetter, RegionGetter>();
builder.Services.AddScoped<IOAuthValidator, GoogleAuthValidator>();
builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
builder.Services.AddScoped<IEntryCodeService, EntryCodeService>();
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();
builder.Services.AddScoped<IConnectionValidator, ConnectionValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<INotificationService>(sp =>{
    return sp.GetRequiredService<NotificationService>();
});
builder.Services.AddScoped<INotificationTokenGetter>(sp =>{
    return sp.GetRequiredService<NotificationService>();
});
builder.Services.AddScoped<INotificationSender, NotificationSender>();
builder.Services.AddScoped<INotificationFacade, NotificationFacade>();
builder.Services.AddScoped<IFileChecksumService, FileChecksumService>();
builder.Services.AddScoped<IFileStorage, FileStorage>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddScoped<IUploadSessionService, UploadSessionService>();
builder.Services.AddSingleton<IChatPolicyFactory, ChatPolicyFactory>();
builder.Services.AddScoped<IChatSettingsFactory, ChatSettingsFactory>();
builder.Services.AddScoped<IUserSettingsFactory, UserSettingsFactory>();
builder.Services.AddScoped<IUserMatchPredicateFactory, UserMatchPredicateFactory>();
builder.Services.AddScoped<IConversationActionValidator, ConversationActionValidator>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IChatLinkService, ChatLinkService>();
builder.Services.AddSingleton<IRandomChatService, RandomChatService>();
builder.Services.AddScoped<IChatAddUserStrategy, AddUserStrategy>();
builder.Services.AddScoped<IChatMatchStrategiesHandlerFactory, ChatMatchStrategiesHandlerFactory>();
builder.Services.AddScoped<IChatCreateStrategiesHandlerFactory, ChatCreateStrategiesHandlerFactory>();
builder.Services.AddScoped<IDirectMatchmakingService, DirectMatchmakingService>();
builder.Services.AddScoped<IMessageProcessorFactory, MessageProcessorFactory>();
builder.Services.AddScoped<IMessageOrchestrator, MessageOrchestrator>();
builder.Services.AddScoped<IGroupMatchmakingService, GroupMatchmakingService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IDisconnectService, DisconnectService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


builder.Services.AddScoped<IWorkTokenFactory, WorkTokenFactory>();
builder.Services.AddScoped<IRegistrationTokenFactory, RegistrationTokenFactory>();
builder.Services.AddScoped<IEntryTokenFactory, EntryTokenFactory>();

builder.Services.AddScoped<IClientContext, ClientContext>();
builder.Services.AddScoped<ITokenContextFactory, TokenContextFactory>();


builder.Services.AddScoped<IWorkTokenContext>(sp =>
{
    var factory = sp.GetRequiredService<ITokenContextFactory>();
    var accessor = sp.GetRequiredService<IUserContextAccessor>();

    var context = factory.Create(accessor);

    if (context is not IWorkTokenContext workContext)
        throw new AuthTokenException(TokenErrors.InvalidType);

    return workContext;
});

builder.Services.AddScoped<IRegistrationTokenContext>(sp =>
{
    var factory = sp.GetRequiredService<ITokenContextFactory>();
    var accessor = sp.GetRequiredService<IUserContextAccessor>();

    var context = factory.Create(accessor);

    if (context is not IRegistrationTokenContext regContext)
        throw new AuthTokenException(RegisterErrors.InvalidContext);

    return regContext;
});

builder.Services.AddScoped<IEntryTokenContext>(sp =>
{
    var factory = sp.GetRequiredService<ITokenContextFactory>();
    var accessor = sp.GetRequiredService<IUserContextAccessor>();

    var context = factory.Create(accessor);

    if (context is not IEntryTokenContext entryContext)
        throw new AuthTokenException(CodeErrors.ContextInvalid);

    return entryContext;
});


builder.Services.AddControllers();

builder.Services.AddSignalR(options =>
{
    options.AddFilter<SignalRExceptionFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBrowserClients", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("X-File-Type", "Content-Disposition");
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],


        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt:Key is not configured.")
                ))
    };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/ws/chat"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

var app = builder.Build();

app.UseCors("AllowBrowserClients");

app.UseRouting();

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseMiddleware<UserBanExceptionMiddleware>();
app.UseMiddleware<PremiumRequiredExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/ws/chat");

app.Run();
