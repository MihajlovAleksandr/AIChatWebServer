namespace AIChatWebServer.Models.User
{
    public class UserData(Guid id, Gender gender, string name, int age)
    {
        public Guid Id { get; set; } = id;
        public Gender Gender { get; set; } = gender;
        public string Name { get; set; } = name;
        public int Age { get; set; } = age;

        public bool IsFits(Preference preference)
        {
            if (IsGenderMatchs(Gender, preference.Gender) || preference.Gender==PreferenceGender.Any)
            {
                if (Age >= preference.MinAge && Age <= preference.MaxAge)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsGenderMatchs(Gender gender, PreferenceGender preferenceGender)
        {
            return preferenceGender switch
            {
                PreferenceGender.Any => true,
                PreferenceGender.Male => gender == Gender.Male,
                PreferenceGender.Female => gender == Gender.Female,
                _ => false
            };
        }

        public override string ToString()
        {
            return $"UserData {{{Id}}}\n{Gender}{Age}\n{Name}";
        }
    }
}
