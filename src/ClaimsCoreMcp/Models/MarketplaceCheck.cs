using System.Text.Json.Serialization;

namespace ClaimsCoreMcp.Models;

/// <summary>
/// Response from online marketplace check for stolen items (OSINT).
/// </summary>
public class MarketplaceCheckResponse
{
    [JsonPropertyName("marketplaces_checked")]
    public List<string> MarketplacesChecked { get; set; } = [];

    [JsonPropertyName("item_found")]
    public bool ItemFound { get; set; }

    [JsonPropertyName("matching_listings")]
    public List<string> MatchingListings { get; set; } = [];

    [JsonPropertyName("fraud_indicator")]
    public int FraudIndicator { get; set; }
}
