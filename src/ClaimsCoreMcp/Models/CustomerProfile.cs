using System.Text.Json.Serialization;

namespace ClaimsCoreMcp.Models;

/// <summary>
/// Customer profile containing basic customer information and contact details.
/// </summary>
public class CustomerProfile
{
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("date_of_birth")]
    public string DateOfBirth { get; set; } = string.Empty;

    [JsonPropertyName("segment")]
    public string Segment { get; set; } = string.Empty;

    [JsonPropertyName("contact")]
    public ContactInfo Contact { get; set; } = new();

    [JsonPropertyName("address")]
    public AddressInfo Address { get; set; } = new();
}

public class ContactInfo
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
}

public class AddressInfo
{
    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}
