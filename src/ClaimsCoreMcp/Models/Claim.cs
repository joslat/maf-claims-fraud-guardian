using System.Text.Json.Serialization;

namespace ClaimsCoreMcp.Models;

/// <summary>
/// Response containing customer claim history.
/// </summary>
public class ClaimHistoryResponse
{
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("claims")]
    public List<Claim> Claims { get; set; } = [];
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
