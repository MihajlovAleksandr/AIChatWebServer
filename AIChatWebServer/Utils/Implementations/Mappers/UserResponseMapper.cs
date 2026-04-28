using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class UserResponseMapper(
        IResponseMapper<Preference, PreferenceResponse> preferenceMapper,
        IResponseMapper<UserData, UserDataResponse> userDataMapper
        ) : IResponseMapper<User, UserResponse>
    {
        private readonly IResponseMapper<Preference, PreferenceResponse> _preferenceMapper = preferenceMapper;
        private readonly IResponseMapper<UserData, UserDataResponse> _userDataMapper = userDataMapper;

        public UserResponse ToResponse(User model)
        {
            if(model.UserData == null)
                throw new ArgumentException(nameof(model.UserData));
            if (model.Preference == null)
                throw new ArgumentException(nameof(model.Preference));
            if (!model.Language.TryGetValue(LanguageContext.Account,out string? language))
                throw new ArgumentException(nameof(language));
            return new UserResponse(model.Id, model.Email, model.RegionCode, model.IsPremium(), _userDataMapper.ToResponse(model.UserData), _preferenceMapper.ToResponse(model.Preference), language);
        }
    }
}
