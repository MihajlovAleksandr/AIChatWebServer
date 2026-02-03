namespace AIChatWebServer.Models.User
{
    public class Preference(Guid id, int minAge, int maxAge, PreferenceGender gender)
    {
        public Guid Id { get; set; } = id;
        public int MinAge { get; set; } = minAge;
        public int MaxAge { get; set; } = maxAge;
        public PreferenceGender Gender { get; set; } = gender;

        public override string ToString()
        {
            return $"Preference {{{Id}}}:\n{Gender}{MinAge}-{MaxAge}";
        }
    }
}
