// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// OSINT (Open Source Intelligence) validation result from Demo12's OSINTAgent.
/// Maps to MarketplaceCheckResponse but with workflow-compatible field names.
/// </summary>
[Description("OSINT (Open Source Intelligence) validation result")]
public class OSINTFinding
{
    [JsonPropertyName("item_found_online")]
    [Description("Whether the reported stolen item was found listed for sale")]
    public bool ItemFoundOnline { get; set; }

    [JsonPropertyName("marketplaces_checked")]
    [Description("List of online marketplaces checked")]
    public List<string> MarketplacesChecked { get; set; } = [];

    [JsonPropertyName("matching_listings")]
    [Description("Details of matching listings found")]
    public List<string> MatchingListings { get; set; } = [];

    [JsonPropertyName("fraud_indicator_score")]
    [Description("Score 0-100 indicating likelihood of fraud")]
    public int FraudIndicatorScore { get; set; }

    [JsonPropertyName("summary")]
    [Description("Summary of OSINT findings")]
    public string Summary { get; set; } = string.Empty;
}

