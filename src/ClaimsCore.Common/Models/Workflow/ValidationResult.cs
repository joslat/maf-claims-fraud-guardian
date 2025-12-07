// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Validation result from Demo11's ClaimsReadyForProcessingAgent.
/// This is the OUTPUT of Demo11 and the INPUT to Demo12 (Fraud Detection).
/// 
/// Contains validated and normalized claim data ready for fraud analysis.
/// </summary>
[Description("Validated claim ready for fraud detection or processing")]
public class ValidationResult
{
    [JsonPropertyName("ready")]
    [Description("True if claim is complete and ready for processing")]
    public bool Ready { get; set; }

    // ===== Claim Data Fields (from ClaimDraft) =====

    [JsonPropertyName("date_of_loss")]
    [Description("Date when the incident occurred")]
    public string? DateOfLoss { get; set; }

    [JsonPropertyName("date_reported")]
    [Description("Date when the claim was reported")]
    public string? DateReported { get; set; }

    [JsonPropertyName("short_description")]
    [Description("Brief 1-2 sentence description of the claim")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("item_description")]
    [Description("Specific description of the item (e.g., 'Trek X-Caliber 8, red mountain bike')")]
    public string? ItemDescription { get; set; }

    [JsonPropertyName("detailed_description")]
    [Description("Detailed description of what happened during the incident")]
    public string? DetailedDescription { get; set; }

    [JsonPropertyName("purchase_price")]
    [Description("Purchase price or value of the item")]
    public decimal? PurchasePrice { get; set; }

    // ===== Validation Metadata =====

    [JsonPropertyName("missing_fields")]
    [Description("List of missing or incomplete fields")]
    public List<string> MissingFields { get; set; } = [];

    [JsonPropertyName("blocking_issues")]
    [Description("Critical issues that block processing")]
    public List<string> BlockingIssues { get; set; } = [];

    [JsonPropertyName("suggested_questions")]
    [Description("Natural language questions to ask the user to fill gaps")]
    public List<string> SuggestedQuestions { get; set; } = [];

    // ===== Resolved/Normalized Data =====

    [JsonPropertyName("customer_id")]
    [Description("Resolved customer ID")]
    public string? CustomerId { get; set; }

    [JsonPropertyName("contract_id")]
    [Description("Resolved contract ID")]
    public string? ContractId { get; set; }

    [JsonPropertyName("normalized_claim_type")]
    [Description("Normalized claim type (e.g., 'Property', 'Auto')")]
    public string? NormalizedClaimType { get; set; }

    [JsonPropertyName("normalized_claim_sub_type")]
    [Description("Normalized claim sub-type (e.g., 'BikeTheft', 'WaterDamage')")]
    public string? NormalizedClaimSubType { get; set; }
}
