using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClaimsCoreMcp.Models;

/// <summary>
/// Core claim data gathered during intake process.
/// Contains all the details about the claim incident.
/// Shared between Demo11 (Claims Intake) and Demo12 (Fraud Detection).
/// </summary>
public class ClaimDraft
{
    [JsonPropertyName("claim_type")]
    [Description("Claim type (e.g., 'Property', 'Auto', 'Health')")]
    public string ClaimType { get; set; } = string.Empty;

    [JsonPropertyName("claim_sub_type")]
    [Description("Claim sub-type (e.g., 'BikeTheft', 'WaterDamage', 'Accident')")]
    public string ClaimSubType { get; set; } = string.Empty;

    [JsonPropertyName("date_of_loss")]
    [Description("Date when the incident occurred")]
    public string DateOfLoss { get; set; } = string.Empty;

    [JsonPropertyName("date_reported")]
    [Description("Date when the claim was reported")]
    public string DateReported { get; set; } = string.Empty;

    [JsonPropertyName("short_description")]
    [Description("Brief 1-2 sentence description of the claim")]
    public string ShortDescription { get; set; } = string.Empty;

    [JsonPropertyName("item_description")]
    [Description("MANDATORY: Specific description of the item (e.g., 'Trek X-Caliber 8, red mountain bike')")]
    public string ItemDescription { get; set; } = string.Empty;

    [JsonPropertyName("detailed_description")]
    [Description("What happened during the incident (circumstances, location, etc.)")]
    public string DetailedDescription { get; set; } = string.Empty;

    [JsonPropertyName("purchase_price")]
    [Description("Purchase price or value of the item")]
    public decimal? PurchasePrice { get; set; }
}
