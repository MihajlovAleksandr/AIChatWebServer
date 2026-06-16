using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Payment
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DurationUnit
    {
        Day,
        Month,
        Year
    }

    [method: JsonConstructor]
    public class SubsAtributes(DurationUnit unit, int value)
    {
        [JsonPropertyName("unit")]
        public DurationUnit Unit { get; init; } = unit;
        [JsonPropertyName("value")] 
        public int Value { get; init; } = value;

        public DateTime Apply(DateTime date)
        {
            return Unit switch
            {
                DurationUnit.Day => date.AddDays(Value),
                DurationUnit.Month => date.AddMonths(Value),
                DurationUnit.Year => date.AddYears(Value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
