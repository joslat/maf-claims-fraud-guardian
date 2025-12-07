// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Initial data quality review result from Demo12's DataReviewAgent.
/// </summary>
[Description("Initial data quality review result")]
public class DataReviewResult
{
    [JsonPropertyName("data_complete")]
    [Description("Whether all required fields are present and well-formed")]
    public bool DataComplete { get; set; }

    [JsonPropertyName("quality_score")]
    [Description("Overall data quality score 0-100")]
    public int QualityScore { get; set; }

    [JsonPropertyName("concerns")]
    [Description("List of data quality concerns")]
    public List<string> Concerns { get; set; } = [];

    [JsonPropertyName("proceed")]
    [Description("Whether to proceed with fraud analysis")]
    public bool Proceed { get; set; }
}

