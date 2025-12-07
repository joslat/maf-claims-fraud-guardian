using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Decision output from Demo11's ClaimsUserFacingAgent (intake agent).
/// Determines if enough information has been gathered to proceed to validation.
/// </summary>
[Description("Result from the intake agent deciding if ready to proceed")]
public class IntakeDecision
{
    [JsonPropertyName("ready_for_validation")]
    [Description("True if enough information gathered to start validation")]
    public bool ReadyForValidation { get; set; }

    [JsonPropertyName("response_to_user")]
    [Description("Message to show the user (question if more info needed, or confirmation if ready)")]
    public string ResponseToUser { get; set; } = string.Empty;

    [JsonPropertyName("customer_id")]
    [Description("Customer ID if provided")]
    public string? CustomerId { get; set; }

    [JsonPropertyName("first_name")]
    [Description("Customer first name if provided")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    [Description("Customer last name if provided")]
    public string? LastName { get; set; }

    [JsonPropertyName("claim_draft")]
    [Description("Claim details extracted so far")]
    public ClaimDraft? ClaimDraft { get; set; }
}

