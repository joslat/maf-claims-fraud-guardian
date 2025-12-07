// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

namespace ClaimsCore.Common.Models;

/// <summary>
/// Status enum for claim processing workflow.
/// </summary>
public enum ClaimReadinessStatus
{
    Draft,
    PendingValidation,
    Ready,
    NeedsMoreInfo
}

