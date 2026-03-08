using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Integrations.Email.Implementations;
using AIChatWebServer.Integrations.Email.Interfaces;
using AIChatWebServer.Middlewares;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Models.Notification;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Implementations;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Context.Implementations;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Implementations;
using AIChatWebServer.Services.Implementations.Chats;
using AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Utils.Errors;
using AIChatWebServer.Utils.Implementations;
using AIChatWebServer.Utils.Implementations.Mappers;
using AIChatWebServer.Utils.Interfaces;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var redisConnectionString =
    builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException("Redis connection string is not configured.");

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddSingleton<IHasher, Hasher>();
builder.Services.AddSingleton<ITokenReplayGuard, RedisTokenReplayGuard>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IStringChanger, StringChanger>();
builder.Services.AddScoped<IHtmlContentBuilder, HtmlContentBuilder>();

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailTextGetter, EmailTextGetter>();
builder.Services.AddScoped<IVerificationCodeSender, VerificationCodeSender>();

builder.Services.AddScoped<IRequestMapper<UserDataRequest, UserData>, UserDataMapper>();
builder.Services.AddScoped<IRequestMapper<PreferenceRequest, Preference>, PreferenceMapper>();
builder.Services.AddScoped<IResponseMapper<UserBan, BanResponse>, BanResponseMapper>();
builder.Services.AddScoped<IResponseMapper<ChatWithUserContext, ChatResponse>, ChatResponseMapper>();
builder.Services.AddScoped<IResponseMapper<AIChatWebServer.Models.Connection.ConnectionInfo, ConnectionResponse>, ConnectionResponseMapper>();
builder.Services.AddScoped<ICollectionResponseMapper<AIChatWebServer.Models.Connection.ConnectionInfo, ConnectionResponse>, CollectionResponseMapper<AIChatWebServer.Models.Connection.ConnectionInfo, ConnectionResponse>>();
builder.Services.AddScoped<NotificationSettingsMapper>();
builder.Services.AddScoped<IMapper<NotificationSettingsRequest, NotificationSettings, NotificationSettingsResponse>>(sp => sp.GetRequiredService<NotificationSettingsMapper>());
builder.Services.AddScoped<IRequestMapper<NotificationSettingsRequest, NotificationSettings>>(sp => sp.GetRequiredService<NotificationSettingsMapper>());
builder.Services.AddScoped<IResponseMapper<NotificationSettings, NotificationSettingsResponse>>(sp => sp.GetRequiredService<NotificationSettingsMapper>());


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
builder.Services.AddScoped<IConnectionRepository, ConnectionRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();

builder.Services.AddScoped<IAuthLoginService, AuthLoginService>();
builder.Services.AddScoped<IAuthRegistrationService, AuthRegistrationService>();
builder.Services.AddScoped<IAuthOAuthService, AuthOAuthService>();

builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IGeoIpService, MaxMindGeoIpService>();
builder.Services.AddScoped<IRegionGetter, RegionGetter>();
builder.Services.AddScoped<IOAuthValidator, GoogleAuthValidator>();
builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
builder.Services.AddScoped<IEntryCodeService, EntryCodeService>();
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();
builder.Services.AddScoped<IConnectionValidator, ConnectionValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IChatPolicyFactory, ChatPolicyFactory>();
builder.Services.AddScoped<IChatSettingsFactory, ChatSettingsFactory>();
builder.Services.AddScoped<IUserSettingsFactory, UserSettingsFactory>();
builder.Services.AddScoped<IConversationActionValidator, ConversationActionValidator>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IChatLinkService, ChatLinkService>();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddScoped<IWorkTokenFactory, WorkTokenFactory>();
builder.Services.AddScoped<IRegistrationTokenFactory, RegistrationTokenFactory>();
builder.Services.AddScoped<IEntryTokenFactory, EntryTokenFactory>();

builder.Services.AddScoped<IClientContext, ClientContext>();

builder.Services.AddScoped<ITokenContextFactory, TokenContextFactory>();

builder.Services.AddScoped<IWorkTokenContext>(sp =>
{
    var factory = sp.GetRequiredService<ITokenContextFactory>();
    var context = factory.Create();

    if (context is not IWorkTokenContext workContext)
    {
        throw new AuthTokenException(
            TokenErrors.InvalidType);
    }

    return workContext;
});


builder.Services.AddScoped<IRegistrationTokenContext>(sp =>
{
    var factory = sp.GetRequiredService<ITokenContextFactory>();
    var context = factory.Create();

    if (context is not IRegistrationTokenContext regContext)
    {
        throw new AuthTokenException(
            RegisterErrors.InvalidContext);
    }

    return regContext;
});


builder.Services.AddScoped<IEntryTokenContext>(sp =>
{
    var factory = sp.GetRequiredService<ITokenContextFactory>();
    var context = factory.Create();

    if (context is not IEntryTokenContext entryContext)
    {
        throw new AuthTokenException(
            CodeErrors.ContextInvalid);
    }

    return entryContext;
});


builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBrowserClients", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
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
                var accessToken =
                    context.Request.Query["access_token"];

                var path =
                    context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/ws"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowBrowserClients");

app.UseRouting();

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseMiddleware<UserBanExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
