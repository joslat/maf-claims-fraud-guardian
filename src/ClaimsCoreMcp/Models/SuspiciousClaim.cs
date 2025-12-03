using System.Text.Json.Serialization;

namespace ClaimsCoreMcp.Models;

/// <summary>
/// Response containing suspicious claims and fraud summary.
/// </summary>
public class SuspiciousClaimsResponse
{
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("suspicious_claims")]
    public List<SuspiciousClaim> SuspiciousClaims { get; set; } = [];

    [JsonPropertyName("summary")]
    public SuspicionSummary Summary { get; set; } = new();
}

public class SuspiciousClaim
{
    [JsonPropertyName("claim_id")]
    public string ClaimId { get; set; } = string.Empty;

    [JsonPropertyName("contract_id")]
    public string ContractId { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("sub_type")]
    public string SubType { get; set; } = string.Empty;

    [JsonPropertyName("date_of_loss")]
    public string DateOfLoss { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("suspicion_score")]
    public double SuspicionScore { get; set; }

    [JsonPropertyName("reason_codes")]
    public List<string> ReasonCodes { get; set; } = [];

    [JsonPropertyName("investigation_status")]
    public string InvestigationStatus { get; set; } = string.Empty;
}

public class SuspicionSummary
{
    [JsonPropertyName("total_suspicious_claims")]
    public int TotalSuspiciousClaims { get; set; }

    [JsonPropertyName("max_suspicion_score")]
    public double MaxSuspicionScore { get; set; }

    [JsonPropertyName("has_prior_fraud_investigation")]
    public bool HasPriorFraudInvestigation { get; set; }
}
