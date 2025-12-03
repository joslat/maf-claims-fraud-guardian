using System.ComponentModel;
using System.Text.Json;
using ClaimsCoreMcp.Models;
using ClaimsCoreMcp.Services;
using ModelContextProtocol.Server;

namespace ClaimsCoreMcp.Tools;

/// <summary>
/// MCP Tools for Claims Core - exposes customer profile, contracts, claim history, and suspicious-claim signals.
/// </summary>
[McpServerToolType]
public static class ClaimsTools
{
    private static readonly MockClaimsDataService _dataService = new();
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    /// <summary>
    /// Resolve a customer and get their basic profile + the customer_id used in all other calls.
    /// Look up by customer_id OR by first_name + last_name.
    /// </summary>
    /// <param name="customerId">Optional: The customer ID (e.g., "CUST-12345")</param>
    /// <param name="firstName">Optional: Customer's first name</param>
    /// <param name="lastName">Optional: Customer's last name</param>
    /// <returns>Customer profile with customer_id, contact info, and address</returns>
    [McpServerTool(Name = "get_customer_profile")]
    [Description("Resolve a customer and get their basic profile + the customer_id used in all other calls. Look up by customer_id OR by first_name + last_name combination.")]
    public static string GetCustomerProfile(
        [Description("The customer ID (e.g., 'CUST-12345'). Use this OR first_name+last_name.")] string? customerId = null,
        [Description("Customer's first name. Use with last_name if customer_id is not known.")] string? firstName = null,
        [Description("Customer's last name. Use with first_name if customer_id is not known.")] string? lastName = null)
    {
        CustomerProfile? customer = null;

        if (!string.IsNullOrWhiteSpace(customerId))
        {
            customer = _dataService.GetCustomerById(customerId);
        }
        else if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
        {
            customer = _dataService.GetCustomerByName(firstName, lastName);
        }
        else
        {
            return JsonSerializer.Serialize(new
            {
                error = "Invalid input",
                message = "Please provide either customer_id OR both first_name and last_name."
            }, _jsonOptions);
        }

        if (customer == null)
        {
            return JsonSerializer.Serialize(new
            {
                error = "Customer not found",
                message = "No customer found with the provided criteria."
            }, _jsonOptions);
        }

        return JsonSerializer.Serialize(customer, _jsonOptions);
    }

    /// <summary>
    /// Return the active (and optionally past) contracts for a given customer, including coverage and deductible info.
    /// </summary>
    /// <param name="customerId">The customer ID to look up contracts for</param>
    /// <returns>List of contracts with coverage details, conditions, and deductibles</returns>
    [McpServerTool(Name = "get_contract")]
    [Description("Return the active (and optionally past) contracts for a given customer, including coverage and deductible info.")]
    public static string GetContract(
        [Description("The customer ID (e.g., 'CUST-12345') to look up contracts for.")] string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return JsonSerializer.Serialize(new
            {
                error = "Invalid input",
                message = "customer_id is required."
            }, _jsonOptions);
        }

        var contracts = _dataService.GetContracts(customerId);

        if (contracts == null)
        {
            return JsonSerializer.Serialize(new
            {
                error = "Customer not found",
                message = $"No contracts found for customer_id: {customerId}"
            }, _jsonOptions);
        }

        return JsonSerializer.Serialize(contracts, _jsonOptions);
    }

    /// <summary>
    /// Return all past claims for the customer with basic metadata and statuses. Used for risk and behavior analysis.
    /// </summary>
    /// <param name="customerId">The customer ID to look up claim history for</param>
    /// <returns>List of all claims with status, amounts, and fraud flags</returns>
    [McpServerTool(Name = "get_claim_history")]
    [Description("Return all past claims for the customer with basic metadata and statuses. Used for risk and behavior analysis.")]
    public static string GetClaimHistory(
        [Description("The customer ID (e.g., 'CUST-12345') to look up claim history for.")] string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return JsonSerializer.Serialize(new
            {
                error = "Invalid input",
                message = "customer_id is required."
            }, _jsonOptions);
        }

        var claimHistory = _dataService.GetClaimHistory(customerId);

        if (claimHistory == null)
        {
            return JsonSerializer.Serialize(new
            {
                error = "Customer not found",
                message = $"No claim history found for customer_id: {customerId}"
            }, _jsonOptions);
        }

        return JsonSerializer.Serialize(claimHistory, _jsonOptions);
    }

    /// <summary>
    /// Return only the claims that are considered suspicious or flagged for fraud, plus a simple summary for the Fraud agents.
    /// </summary>
    /// <param name="customerId">The customer ID to look up suspicious claims for</param>
    /// <returns>Suspicious claims with suspicion scores, reason codes, and summary</returns>
    [McpServerTool(Name = "get_suspicious_claims")]
    [Description("Return only the claims that are considered suspicious or flagged for fraud, plus a simple summary for the Fraud agents.")]
    public static string GetSuspiciousClaims(
        [Description("The customer ID (e.g., 'CUST-12345') to look up suspicious claims for.")] string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return JsonSerializer.Serialize(new
            {
                error = "Invalid input",
                message = "customer_id is required."
            }, _jsonOptions);
        }

        var suspiciousClaims = _dataService.GetSuspiciousClaims(customerId);

        if (suspiciousClaims == null)
        {
            return JsonSerializer.Serialize(new
            {
                error = "Customer not found",
                message = $"No data found for customer_id: {customerId}"
            }, _jsonOptions);
        }

        return JsonSerializer.Serialize(suspiciousClaims, _jsonOptions);
    }
}
