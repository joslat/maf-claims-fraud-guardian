// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

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

    // ===== Workflow Compatibility =====

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    // Alias properties for workflow field name compatibility
    [JsonPropertyName("item_found_online")]
    public bool ItemFoundOnline => ItemFound;

    [JsonPropertyName("fraud_indicator_score")]
    public int FraudIndicatorScore => FraudIndicator;
}

