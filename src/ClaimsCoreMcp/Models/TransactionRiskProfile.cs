using System.Text.Json.Serialization;

namespace ClaimsCoreMcp.Models;

/// <summary>
/// Transaction risk profile analysis for fraud detection.
/// </summary>
public class TransactionRiskProfile
{
    [JsonPropertyName("amount_percentile")]
    public int AmountPercentile { get; set; }

    [JsonPropertyName("timing_anomaly")]
    public bool TimingAnomaly { get; set; }

    [JsonPropertyName("red_flags")]
    public List<string> RedFlags { get; set; } = [];

    [JsonPropertyName("transaction_risk_score")]
    public int TransactionRiskScore { get; set; }
}
