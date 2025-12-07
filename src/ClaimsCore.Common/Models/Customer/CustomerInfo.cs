using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Customer identification and contact information.
/// Shared between Demo11 (Claims Intake) and Demo12 (Fraud Detection).
/// This is a simplified version that matches the workflow's CustomerInfo contract.
/// </summary>
public class CustomerInfo
{
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
