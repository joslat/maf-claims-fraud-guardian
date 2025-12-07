using System.Text.Json.Serialization;

// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

namespace ClaimsCore.Common.Models;

/// <summary>
/// Workflow state for Demo12 Fraud Detection.
/// Tracks the entire fraud analysis process including findings from multiple agents.
/// </summary>
public class FraudAnalysisState
{
    [JsonPropertyName("original_claim")]
    public ValidationResult? OriginalClaim { get; set; }

    [JsonPropertyName("data_review")]
    public DataReviewResult? DataReview { get; set; }

    [JsonPropertyName("claim_type")]
    public string ClaimType { get; set; } = string.Empty;

    [JsonPropertyName("claim_sub_type")]
    public string ClaimSubType { get; set; } = string.Empty;

    // Fraud detection findings (fan-in from 3 agents)
    [JsonPropertyName("osint_finding")]
    public OSINTFinding? OSINTFinding { get; set; }

    [JsonPropertyName("user_history_finding")]
    public UserHistoryFinding? UserHistoryFinding { get; set; }

    [JsonPropertyName("transaction_fraud_finding")]
    public TransactionFraudFinding? TransactionFraudFinding { get; set; }

    // Final decision
    [JsonPropertyName("fraud_decision")]
    public FraudDecision? FraudDecision { get; set; }
}

