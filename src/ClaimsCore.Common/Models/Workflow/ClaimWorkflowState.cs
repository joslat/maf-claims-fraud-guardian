// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Workflow state for Demo11 Claims Intake.
/// Tracks the entire intake process.
/// Note: Conversation history is maintained in the workflow executor, not in this state.
/// </summary>
public class ClaimWorkflowState
{
    [JsonPropertyName("intake_iteration")]
    public int IntakeIteration { get; set; } = 1;

    [JsonPropertyName("status")]
    public ClaimReadinessStatus Status { get; set; } = ClaimReadinessStatus.Draft;

    [JsonPropertyName("customer")]
    public CustomerInfo? Customer { get; set; }

    [JsonPropertyName("claim_draft")]
    public ClaimDraft ClaimDraft { get; set; } = new();

    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }
}
