using ClaimsCoreMcp.Models;

namespace ClaimsCoreMcp.Services;

/// <summary>
/// Mock data service providing sample insurance claims and customer data.
/// Replace with real database/API calls in production.
/// </summary>
public class MockClaimsDataService
{
    private readonly Dictionary<string, CustomerProfile> _customers;
    private readonly Dictionary<string, ContractResponse> _contracts;
    private readonly Dictionary<string, ClaimHistoryResponse> _claimHistories;
    private readonly Dictionary<string, SuspiciousClaimsResponse> _suspiciousClaims;

    public MockClaimsDataService()
    {
        // Initialize mock data
        _customers = InitializeCustomers();
        _contracts = InitializeContracts();
        _claimHistories = InitializeClaimHistories();
        _suspiciousClaims = InitializeSuspiciousClaims();
    }

    public CustomerProfile? GetCustomerByName(string firstName, string lastName)
    {
        return _customers.Values.FirstOrDefault(c =>
            c.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
            c.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
    }

    public CustomerProfile? GetCustomerById(string customerId)
    {
        return _customers.TryGetValue(customerId, out var customer) ? customer : null;
    }

    public ContractResponse? GetContracts(string customerId)
    {
        return _contracts.TryGetValue(customerId, out var contracts) ? contracts : null;
    }

    public ClaimHistoryResponse? GetClaimHistory(string customerId)
    {
        return _claimHistories.TryGetValue(customerId, out var history) ? history : null;
    }

    public SuspiciousClaimsResponse? GetSuspiciousClaims(string customerId)
    {
        return _suspiciousClaims.TryGetValue(customerId, out var suspicious) ? suspicious : null;
    }

    private static Dictionary<string, CustomerProfile> InitializeCustomers()
    {
        return new Dictionary<string, CustomerProfile>
        {
            ["CUST-12345"] = new CustomerProfile
            {
                CustomerId = "CUST-12345",
                FirstName = "Jose",
                LastName = "Latorre",
                DateOfBirth = "1980-05-12",
                Segment = "Retail",
                Contact = new ContactInfo
                {
                    Email = "jose.latorre@example.com",
                    Phone = "+41 79 123 45 67"
                },
                Address = new AddressInfo
                {
                    Street = "Bahnhofstrasse 1",
                    PostalCode = "8000",
                    City = "Zürich",
                    Country = "CH"
                }
            },
            ["CUST-67890"] = new CustomerProfile
            {
                CustomerId = "CUST-67890",
                FirstName = "Maria",
                LastName = "Garcia",
                DateOfBirth = "1992-08-25",
                Segment = "Premium",
                Contact = new ContactInfo
                {
                    Email = "maria.garcia@example.com",
                    Phone = "+41 79 987 65 43"
                },
                Address = new AddressInfo
                {
                    Street = "Limmatquai 50",
                    PostalCode = "8001",
                    City = "Zürich",
                    Country = "CH"
                }
            },
            ["CUST-11111"] = new CustomerProfile
            {
                CustomerId = "CUST-11111",
                FirstName = "Hans",
                LastName = "Mueller",
                DateOfBirth = "1975-03-18",
                Segment = "Retail",
                Contact = new ContactInfo
                {
                    Email = "hans.mueller@example.com",
                    Phone = "+41 79 555 12 34"
                },
                Address = new AddressInfo
                {
                    Street = "Hauptstrasse 22",
                    PostalCode = "3000",
                    City = "Bern",
                    Country = "CH"
                }
            }
        };
    }

    private static Dictionary<string, ContractResponse> InitializeContracts()
    {
        return new Dictionary<string, ContractResponse>
        {
            ["CUST-12345"] = new ContractResponse
            {
                CustomerId = "CUST-12345",
                Contracts =
                [
                    new Contract
                    {
                        ContractId = "POL-98765",
                        ProductType = "Property",
                        SubType = "BikeTheft",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2024-01-01",
                            EndDate = "2025-01-01"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "Bike theft",
                            MaxAmount = 5000,
                            Currency = "CHF",
                            GeographicalScope = "Europe",
                            Conditions = ["Police report required", "Bike must be locked"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 200,
                            Currency = "CHF"
                        }
                    },
                    new Contract
                    {
                        ContractId = "POL-98766",
                        ProductType = "Property",
                        SubType = "Electronics",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2024-01-01",
                            EndDate = "2025-01-01"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "Electronics protection",
                            MaxAmount = 3000,
                            Currency = "CHF",
                            GeographicalScope = "Worldwide",
                            Conditions = ["Receipt required", "Device under 3 years old"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 100,
                            Currency = "CHF"
                        }
                    }
                ]
            },
            ["CUST-67890"] = new ContractResponse
            {
                CustomerId = "CUST-67890",
                Contracts =
                [
                    new Contract
                    {
                        ContractId = "POL-54321",
                        ProductType = "Property",
                        SubType = "HomeContents",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2023-06-01",
                            EndDate = "2024-06-01"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "Home contents insurance",
                            MaxAmount = 50000,
                            Currency = "CHF",
                            GeographicalScope = "Switzerland",
                            Conditions = ["Inventory list required", "Security measures in place"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 500,
                            Currency = "CHF"
                        }
                    }
                ]
            },
            ["CUST-11111"] = new ContractResponse
            {
                CustomerId = "CUST-11111",
                Contracts =
                [
                    new Contract
                    {
                        ContractId = "POL-77777",
                        ProductType = "Property",
                        SubType = "BikeTheft",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2024-03-01",
                            EndDate = "2025-03-01"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "Bike theft",
                            MaxAmount = 8000,
                            Currency = "CHF",
                            GeographicalScope = "Europe",
                            Conditions = ["Police report required", "Bike must be locked", "GPS tracker recommended"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 300,
                            Currency = "CHF"
                        }
                    }
                ]
            }
        };
    }

    private static Dictionary<string, ClaimHistoryResponse> InitializeClaimHistories()
    {
        return new Dictionary<string, ClaimHistoryResponse>
        {
            ["CUST-12345"] = new ClaimHistoryResponse
            {
                CustomerId = "CUST-12345",
                Claims =
                [
                    new Claim
                    {
                        ClaimId = "CLM-001",
                        ContractId = "POL-98765",
                        Type = "Property",
                        SubType = "BikeTheft",
                        DateOfLoss = "2023-08-10",
                        ReportedDate = "2023-08-11",
                        Status = "Approved",
                        PaidAmount = new AmountInfo { Amount = 1200, Currency = "CHF" },
                        FraudFlag = false
                    },
                    new Claim
                    {
                        ClaimId = "CLM-002",
                        ContractId = "POL-98765",
                        Type = "Property",
                        SubType = "Electronics",
                        DateOfLoss = "2022-03-05",
                        ReportedDate = "2022-03-06",
                        Status = "Rejected",
                        PaidAmount = new AmountInfo { Amount = 0, Currency = "CHF" },
                        FraudFlag = true
                    }
                ]
            },
            ["CUST-67890"] = new ClaimHistoryResponse
            {
                CustomerId = "CUST-67890",
                Claims =
                [
                    new Claim
                    {
                        ClaimId = "CLM-003",
                        ContractId = "POL-54321",
                        Type = "Property",
                        SubType = "HomeContents",
                        DateOfLoss = "2023-12-15",
                        ReportedDate = "2023-12-16",
                        Status = "Approved",
                        PaidAmount = new AmountInfo { Amount = 2500, Currency = "CHF" },
                        FraudFlag = false
                    }
                ]
            },
            ["CUST-11111"] = new ClaimHistoryResponse
            {
                CustomerId = "CUST-11111",
                Claims = [] // Clean customer with no claim history
            }
        };
    }

    private static Dictionary<string, SuspiciousClaimsResponse> InitializeSuspiciousClaims()
    {
        return new Dictionary<string, SuspiciousClaimsResponse>
        {
            ["CUST-12345"] = new SuspiciousClaimsResponse
            {
                CustomerId = "CUST-12345",
                SuspiciousClaims =
                [
                    new SuspiciousClaim
                    {
                        ClaimId = "CLM-002",
                        ContractId = "POL-98765",
                        Type = "Property",
                        SubType = "Electronics",
                        DateOfLoss = "2022-03-05",
                        Status = "Rejected",
                        SuspicionScore = 0.86,
                        ReasonCodes = ["FREQUENT_HIGH_VALUE_CLAIMS", "INCONSISTENT_STORY", "MISSING_DOCUMENTATION"],
                        InvestigationStatus = "Closed"
                    }
                ],
                Summary = new SuspicionSummary
                {
                    TotalSuspiciousClaims = 1,
                    MaxSuspicionScore = 0.86,
                    HasPriorFraudInvestigation = true
                }
            },
            ["CUST-67890"] = new SuspiciousClaimsResponse
            {
                CustomerId = "CUST-67890",
                SuspiciousClaims = [],
                Summary = new SuspicionSummary
                {
                    TotalSuspiciousClaims = 0,
                    MaxSuspicionScore = 0.0,
                    HasPriorFraudInvestigation = false
                }
            },
            ["CUST-11111"] = new SuspiciousClaimsResponse
            {
                CustomerId = "CUST-11111",
                SuspiciousClaims = [],
                Summary = new SuspicionSummary
                {
                    TotalSuspiciousClaims = 0,
                    MaxSuspicionScore = 0.0,
                    HasPriorFraudInvestigation = false
                }
            }
        };
    }
}
