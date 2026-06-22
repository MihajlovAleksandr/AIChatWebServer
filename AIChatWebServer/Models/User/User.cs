using AIChatWebServer.Models.User;

namespace AIChatWebServer.Models.User
{
    public sealed class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = null!;
        public string RegionCode { get; private set; } = null!;
        public RegistrationState RegistrationState { get; private set; }

        public List<UserPremium> Premium { get; private set; } = new();
        public UserData? UserData { get; set; }
        public Preference? Preference { get; set; }

        public ICollection<AuthIdentity> AuthIdentities { get; private set; } = new List<AuthIdentity>();
        public Dictionary<LanguageContext, string> Language { get;  set; } = new();

        private User() { }

        public User(Guid id, string email, string regionCode)
        {
            Id = id;
            Email = email;
            RegionCode = regionCode;
            RegistrationState = RegistrationState.Created;
        }

        public void VerifyEmail()
        {
            if (RegistrationState != RegistrationState.Created)
                throw new InvalidOperationException("Email can only be verified from Created state.");

            RegistrationState = RegistrationState.EmailVerified;
        }

        public void CompleteUserData(UserData userData)
        {
            if (RegistrationState != RegistrationState.EmailVerified)
                throw new InvalidOperationException("Email must be verified before completing user data.");

            UserData = userData;
            RegistrationState = RegistrationState.UserDataCompleted;
        }

        public void CompletePreference(Preference preference)
        {
            if (RegistrationState != RegistrationState.UserDataCompleted)
                throw new InvalidOperationException("User data must be completed before setting preferences.");

            Preference = preference;
            RegistrationState = RegistrationState.PreferenceCompleted;
        }

        public void CompleteRegistration()
        {
            if (RegistrationState != RegistrationState.PreferenceCompleted)
                throw new InvalidOperationException("Preference must be completed before finishing registration.");

            RegistrationState = RegistrationState.Completed;
        }

        public void AddAuthIdentity(AuthIdentity identity)
        {
            if (identity.UserId != Id)
                throw new InvalidOperationException("AuthIdentity belongs to another user.");

            AuthIdentities.Add(identity);
        }

        public AuthIdentity? GetAuthIdentity(string identityProviderCode)
        {
            foreach(AuthIdentity identity in AuthIdentities)
            {
                if(identity.Provider.Code == identityProviderCode) return identity;
            }
            return null;
        }

        public bool IsPremium()
        {
            if (Premium == null || Premium.Count == 0)
                return false;

            var now = DateTime.UtcNow;

            return Premium.Any(p => p.IsActive(now));
        }
    }
}
