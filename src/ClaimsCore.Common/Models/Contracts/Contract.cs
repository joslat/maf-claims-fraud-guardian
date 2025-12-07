using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Response containing customer contracts and policy information.
/// </summary>
public class ContractResponse
{
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("contracts")]
    public List<Contract> Contracts { get; set; } = [];
}

public class Contract
{
    [JsonPropertyName("contract_id")]
    public string ContractId { get; set; } = string.Empty;

    [JsonPropertyName("product_type")]
    public string ProductType { get; set; } = string.Empty;

    [JsonPropertyName("sub_type")]
    public string SubType { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("period")]
    public ContractPeriod Period { get; set; } = new();

    [JsonPropertyName("coverage")]
    public CoverageInfo Coverage { get; set; } = new();

    [JsonPropertyName("deductible")]
    public DeductibleInfo Deductible { get; set; } = new();
}

public class ContractPeriod
{
    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("end_date")]
    public string EndDate { get; set; } = string.Empty;
}

public class CoverageInfo
{
    [JsonPropertyName("coverage_name")]
    public string CoverageName { get; set; } = string.Empty;

    [JsonPropertyName("max_amount")]
    public decimal MaxAmount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("geographical_scope")]
    public string GeographicalScope { get; set; } = string.Empty;

    [JsonPropertyName("conditions")]
    public List<string> Conditions { get; set; } = [];
}

public class DeductibleInfo
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
}
