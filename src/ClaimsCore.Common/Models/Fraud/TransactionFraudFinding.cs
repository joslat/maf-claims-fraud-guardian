// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClaimsCore.Common.Models;

/// <summary>
/// Transaction-level fraud scoring from Demo12's TransactionFraudAgent.
/// </summary>
[Description("Transaction-level fraud scoring")]
public class TransactionFraudFinding
{
    [JsonPropertyName("anomaly_score")]
    [Description("Transaction anomaly score 0-100")]
    public int AnomalyScore { get; set; }

    [JsonPropertyName("red_flags")]
    [Description("List of fraud red flags detected")]
    public List<string> RedFlags { get; set; } = [];

    [JsonPropertyName("transaction_fraud_score")]
    [Description("Overall transaction fraud score 0-100")]
    public int TransactionFraudScore { get; set; }

    [JsonPropertyName("summary")]
    [Description("Summary of transaction analysis")]
    public string Summary { get; set; } = string.Empty;
}

