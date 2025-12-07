using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Customer history and fraud score analysis from Demo12's UserHistoryAgent.
/// </summary>
[Description("Customer history and fraud score analysis")]
public class UserHistoryFinding
{
    [JsonPropertyName("previous_claims_count")]
    [Description("Number of previous claims filed")]
    public int PreviousClaimsCount { get; set; }

    [JsonPropertyName("suspicious_activity_detected")]
    [Description("Whether suspicious patterns were detected")]
    public bool SuspiciousActivityDetected { get; set; }

    [JsonPropertyName("customer_fraud_score")]
    [Description("Historical fraud score for customer 0-100")]
    public int CustomerFraudScore { get; set; }

    [JsonPropertyName("claim_history")]
    [Description("Summary of past claims and their outcomes")]
    public List<string> ClaimHistory { get; set; } = [];

    [JsonPropertyName("summary")]
    [Description("Summary of user history analysis")]
    public string Summary { get; set; } = string.Empty;
}

