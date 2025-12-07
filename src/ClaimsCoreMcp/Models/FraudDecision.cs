using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClaimsCoreMcp.Models;

/// <summary>
/// Final fraud determination from Demo12's FraudDecisionAgent.
/// </summary>
[Description("Final fraud determination")]
public class FraudDecision
{
    [JsonPropertyName("is_fraud")]
    [Description("Final determination: Is this likely fraud?")]
    public bool IsFraud { get; set; }

    [JsonPropertyName("confidence_score")]
    [Description("Confidence in the decision 0-100")]
    public int ConfidenceScore { get; set; }

    [JsonPropertyName("combined_fraud_score")]
    [Description("Combined fraud score from all sources 0-100")]
    public int CombinedFraudScore { get; set; }

    [JsonPropertyName("recommendation")]
    [Description("Recommended action (APPROVE, INVESTIGATE, REJECT)")]
    public string Recommendation { get; set; } = string.Empty;

    [JsonPropertyName("reasoning")]
    [Description("Explanation of the decision")]
    public string Reasoning { get; set; } = string.Empty;

    [JsonPropertyName("key_factors")]
    [Description("Key factors that influenced the decision")]
    public List<string> KeyFactors { get; set; } = [];
}
