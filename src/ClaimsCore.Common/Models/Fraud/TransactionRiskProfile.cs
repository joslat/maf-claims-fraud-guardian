using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

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

    // ===== Workflow Compatibility Fields =====

    /// <summary>
    /// Anomaly score for transaction (workflow field).
    /// </summary>
    [JsonPropertyName("anomaly_score")]
    public int AnomalyScore { get; set; }

    /// <summary>
    /// Summary of transaction analysis.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Alias for transaction_risk_score (workflow uses transaction_fraud_score).
    /// </summary>
    [JsonPropertyName("transaction_fraud_score")]
    public int TransactionFraudScore => TransactionRiskScore;
}

