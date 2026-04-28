using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.Chats.RandomChat
{
    public sealed class UserProfile
    {
        public string Name { get; init; } = default!;
        public int Age { get; init; }
        public string Gender { get; init; } = default!;
        public string Country { get; init; } = default!;
        public string City { get; init; } = default!;

        public Dictionary<string, string> Languages { get; init; } = [];

        public string Education { get; init; } = default!;
        public string Occupation { get; init; } = default!;
        public string IncomeLevel { get; init; } = default!;
        public string LivingSituation { get; init; } = default!;

        public string[] Interests { get; init; } = [];
        public string[] Hobbies { get; init; } = [];

        public string DailyRoutine { get; init; } = default!;

        public string[] PersonalityTraits { get; init; } = [];
        public string[] Quirks { get; init; } = [];

        public string CommunicationStyle { get; init; } = default!;

        public SocialContext SocialContext { get; init; } = new();
        public AdaptationToOther AdaptationToOther { get; init; } = new();
        public KnowledgeScope KnowledgeScope { get; init; } = new();
        public Opinions Opinions { get; init; } = new();

        public string[] Limitations { get; init; } = [];

        public string Background { get; init; } = default!;

        public string CoreBlock => BuildCore();
        public string BehaviorBlock => BuildBehavior();
        public string KnowledgeBlock => BuildKnowledge();
        public string SocialBlock => BuildSocial();

        private string BuildCore()
        {
            return $"""
                Name: {Name}, {Age}
                Style: {CommunicationStyle}
                Personality: {string.Join(", ", PersonalityTraits)}
                Quirks: {string.Join(", ", Quirks)}
                """;
        }

        private string BuildBehavior()
        {
            return $"""
                Behavior: {AdaptationToOther.Behavior}
                Tone: {AdaptationToOther.Tone}
                """;
        }

        private string BuildKnowledge()
        {
            return $"""
                Knowledge: strong in {string.Join(", ", KnowledgeScope.StrongAreas)}, weak in {string.Join(", ", KnowledgeScope.WeakAreas)}
                Limits: {string.Join(", ", Limitations)}
                """;
        }

        private string BuildSocial()
        {
            return $"""
                Social: {SocialContext.SocialCircle}
                Family: {SocialContext.FamilyRelationship}
                Relationship: {SocialContext.RelationshipStatus}
                """;
        }

        public string BuildContext(QueryTags tags)
        {
            var parts = new List<string>
            {
                CoreBlock
            };

            if (tags.HasFlag(QueryTags.Casual) || tags.HasFlag(QueryTags.SmallTalk))
            {
                parts.Add(BehaviorBlock);
            }

            if (tags.HasFlag(QueryTags.Personal) || tags.HasFlag(QueryTags.Emotional))
            {
                parts.Add(BehaviorBlock);
                parts.Add(SocialBlock);
            }

            if (tags.HasFlag(QueryTags.Knowledge) || tags.HasFlag(QueryTags.Technical))
            {
                parts.Add(KnowledgeBlock);
            }

            if (tags.HasFlag(QueryTags.Complex))
            {
                parts.Add(KnowledgeBlock);
                parts.Add(BehaviorBlock);
            }

            return string.Join("\n", parts.Distinct());
        }
    }

    public sealed class SocialContext
    {
        [JsonPropertyName("socialCircle")]
        public string SocialCircle { get; init; } = default!;
        [JsonPropertyName("relationshipStatus")]
        public string RelationshipStatus { get; init; } = default!;
        [JsonPropertyName("familyRelationship")]
        public string FamilyRelationship { get; init; } = default!;
    }

    public sealed class AdaptationToOther
    {
        [JsonPropertyName("tone")]
        public string Tone { get; init; } = default!;
        [JsonPropertyName("behavior")]
        public string Behavior { get; init; } = default!;
        [JsonPropertyName("whatTheyHide")]
        public string WhatTheyHide { get; init; } = default!;
        [JsonPropertyName("whatTheyEmphasize")]
        public string WhatTheyEmphasize { get; init; } = default!;
    }

    public sealed class KnowledgeScope
    {
        [JsonPropertyName("strongAreas")]
        public string[] StrongAreas { get; init; } = [];
        [JsonPropertyName("weakAreas")]
        public string[] WeakAreas { get; init; } = [];
    }

    public sealed class Opinions
    {
        [JsonPropertyName("technology")]
        public string Technology { get; init; } = default!;
        [JsonPropertyName("education")]
        public string Education { get; init; } = default!;
        [JsonPropertyName("socialMedia")]
        public string SocialMedia { get; init; } = default!;
    }
}