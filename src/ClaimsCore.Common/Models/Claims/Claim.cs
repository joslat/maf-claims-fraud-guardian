// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Response containing customer claim history.
/// </summary>
public class ClaimHistoryResponse
{
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("claims")]
    public List<Claim> Claims { get; set; } = [];

    // ===== Computed Summary Fields (Workflow Compatibility) =====

    [JsonPropertyName("total_claims")]
    public int TotalClaims => Claims.Count;

    [JsonPropertyName("claims_last_12_months")]
    public int ClaimsLast12Months => Claims.Count(c =>
        DateTime.TryParse(c.ReportedDate, out var date) &&
        date > DateTime.Now.AddMonths(-12));

    [JsonPropertyName("previous_fraud_flags")]
    public int PreviousFraudFlags => Claims.Count(c => c.FraudFlag);

    [JsonPropertyName("customer_fraud_score")]
    public int CustomerFraudScore { get; set; }

    [JsonPropertyName("claim_history_summary")]
    public List<string> ClaimHistorySummary => Claims
        .Select(c => $"{c.ReportedDate}: {c.SubType} - {c.Status.ToUpperInvariant()} - {c.PaidAmount.Currency} {c.PaidAmount.Amount}")
        .ToList();

    // ===== Additional Workflow Compatibility Fields =====

    /// <summary>
    /// Alias for total_claims (workflow uses previous_claims_count).
    /// </summary>
    [JsonPropertyName("previous_claims_count")]
    public int PreviousClaimsCount => TotalClaims;

    /// <summary>
    /// Alias for claim_history_summary (workflow uses claim_history).
    /// </summary>
    [JsonPropertyName("claim_history")]
    public List<string> ClaimHistory => ClaimHistorySummary;

    /// <summary>
    /// Whether suspicious activity was detected (derived from fraud flags).
    /// </summary>
    [JsonPropertyName("suspicious_activity_detected")]
    public bool SuspiciousActivityDetected => PreviousFraudFlags > 0;

    /// <summary>
    /// Summary of user history analysis.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class Claim
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

    [JsonPropertyName("reported_date")]
    public string ReportedDate { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("paid_amount")]
    public AmountInfo PaidAmount { get; set; } = new();

    [JsonPropertyName("fraud_flag")]
    public bool FraudFlag { get; set; }
}

public class AmountInfo
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
}
