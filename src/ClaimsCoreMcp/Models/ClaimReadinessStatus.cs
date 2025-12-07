namespace ClaimsCoreMcp.Models;

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
