using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Final processed claim output from Demo11.
/// </summary>
public class ProcessedClaim
{
    [JsonPropertyName("claim_id")]
    public string ClaimId { get; set; } = string.Empty;

    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("contract_id")]
    public string ContractId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

