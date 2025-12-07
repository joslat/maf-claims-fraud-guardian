// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using ClaimsCore.Common.Models;

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

    public MarketplaceCheckResponse CheckOnlineMarketplaces(string itemDescription, decimal itemValue)
    {
        // Mock logic: High-value items are "found" online (suspicious!)
        var found = itemValue > 1000;
        var fraudIndicator = found ? 85 : 15;

        var marketplaces = new List<string> { "ricardo.ch", "anibis.ch", "ebay.ch", "facebook_marketplace" };
        
        var matchingListings = found
            ? new List<string> { $"Ricardo.ch listing: Similar item, CHF {itemValue * 0.8m:F0} - Listed 3 days after reported theft" }
            : new List<string>();

        var summary = found
            ? $"ALERT: Potential match found online. Item similar to '{itemDescription}' listed on Ricardo.ch for CHF {itemValue * 0.8m:F0}, posted shortly after reported theft. High fraud indicator."
            : $"No suspicious listings found for '{itemDescription}' on checked marketplaces. Low fraud indicator.";

        return new MarketplaceCheckResponse
        {
            MarketplacesChecked = marketplaces,
            ItemFound = found,
            MatchingListings = matchingListings,
            FraudIndicator = fraudIndicator,
            Summary = summary
        };
    }

    public TransactionRiskProfile GetTransactionRiskProfile(decimal claimAmount, string dateOfLoss)
    {
        // Mock logic: High value + recent = high risk
        var redFlags = new List<string>();
        var highValue = claimAmount > 1000;
        var recent = DateTime.TryParse(dateOfLoss, out var lossDate) &&
                    (DateTime.Now - lossDate).TotalDays < 7;

        if (highValue) redFlags.Add("High value claim (>CHF 1000)");
        if (recent) redFlags.Add("Claim filed immediately after incident");
        if (highValue && recent) redFlags.Add("High value + immediate filing pattern");

        var riskScore = redFlags.Count * 25;
        var amountPercentile = highValue ? 85 : 35;
        var anomalyScore = riskScore; // Same as risk score for simplicity

        var summary = riskScore > 50
            ? $"HIGH RISK: Claim amount CHF {claimAmount} with {redFlags.Count} red flags detected. Immediate investigation recommended."
            : $"LOW RISK: Claim amount CHF {claimAmount} appears normal. No significant red flags detected.";

        return new TransactionRiskProfile
        {
            AmountPercentile = amountPercentile,
            TimingAnomaly = recent,
            RedFlags = redFlags,
            TransactionRiskScore = riskScore,
            AnomalyScore = anomalyScore,
            Summary = summary
        };
    }

    private static Dictionary<string, CustomerProfile> InitializeCustomers()
    {
        return new Dictionary<string, CustomerProfile>
        {
            // === Original MCP Customers ===
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
            },
            
            // === Workflow Customers (from Demo11/Demo12) ===
            ["CUST-10001"] = new CustomerProfile
            {
                CustomerId = "CUST-10001",
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = "1985-03-15",
                Segment = "Retail",
                Contact = new ContactInfo
                {
                    Email = "john.smith@example.com",
                    Phone = "+41 79 111 22 33"
                },
                Address = new AddressInfo
                {
                    Street = "Musterstrasse 10",
                    PostalCode = "8002",
                    City = "Zürich",
                    Country = "CH"
                }
            },
            ["CUST-10002"] = new CustomerProfile
            {
                CustomerId = "CUST-10002",
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = "1990-07-22",
                Segment = "Premium",
                Contact = new ContactInfo
                {
                    Email = "jane.doe@example.com",
                    Phone = "+41 79 444 55 66"
                },
                Address = new AddressInfo
                {
                    Street = "Seestrasse 45",
                    PostalCode = "8003",
                    City = "Zürich",
                    Country = "CH"
                }
            },
            ["CUST-10003"] = new CustomerProfile
            {
                CustomerId = "CUST-10003",
                FirstName = "Alice",
                LastName = "Johnson",
                DateOfBirth = "1988-11-08",
                Segment = "Retail",
                Contact = new ContactInfo
                {
                    Email = "alice.johnson@example.com",
                    Phone = "+41 79 777 88 99"
                },
                Address = new AddressInfo
                {
                    Street = "Bergstrasse 33",
                    PostalCode = "3001",
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
            },
            
            // === Workflow Customer Contracts ===
            ["CUST-10001"] = new ContractResponse
            {
                CustomerId = "CUST-10001",
                Contracts =
                [
                    new Contract
                    {
                        ContractId = "CONTRACT-P-5001",
                        ProductType = "Property",
                        SubType = "BikeTheft",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2025-01-01",
                            EndDate = "2027-01-01"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "BikeTheft",
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
                        ContractId = "CONTRACT-P-5001-WD",
                        ProductType = "Property",
                        SubType = "WaterDamage",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2023-01-01",
                            EndDate = "2027-01-01"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "WaterDamage",
                            MaxAmount = 10000,
                            Currency = "CHF",
                            GeographicalScope = "Switzerland",
                            Conditions = ["Damage assessment required", "Professional repair estimate"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 300,
                            Currency = "CHF"
                        }
                    },
                    new Contract
                    {
                        ContractId = "CONTRACT-P-5001-FIRE",
                        ProductType = "Property",
                        SubType = "Fire",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2023-01-01",
                            EndDate = "2027-01-01"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "Fire",
                            MaxAmount = 50000,
                            Currency = "CHF",
                            GeographicalScope = "Switzerland",
                            Conditions = ["Fire department report required"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 500,
                            Currency = "CHF"
                        }
                    }
                ]
            },
            ["CUST-10002"] = new ContractResponse
            {
                CustomerId = "CUST-10002",
                Contracts =
                [
                    new Contract
                    {
                        ContractId = "CONTRACT-A-5002",
                        ProductType = "Auto",
                        SubType = "Collision",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2022-06-15",
                            EndDate = "2025-06-15"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "Collision",
                            MaxAmount = 30000,
                            Currency = "CHF",
                            GeographicalScope = "Europe",
                            Conditions = ["Police report for claims >CHF 5000"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 1000,
                            Currency = "CHF"
                        }
                    },
                    new Contract
                    {
                        ContractId = "CONTRACT-A-5002-THEFT",
                        ProductType = "Auto",
                        SubType = "Theft",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2022-06-15",
                            EndDate = "2025-06-15"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "Theft",
                            MaxAmount = 30000,
                            Currency = "CHF",
                            GeographicalScope = "Europe",
                            Conditions = ["Police report required", "Vehicle tracking system recommended"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 1000,
                            Currency = "CHF"
                        }
                    }
                ]
            },
            ["CUST-10003"] = new ContractResponse
            {
                CustomerId = "CUST-10003",
                Contracts =
                [
                    new Contract
                    {
                        ContractId = "CONTRACT-P-5003",
                        ProductType = "Property",
                        SubType = "BikeTheft",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2023-03-10",
                            EndDate = "2025-03-10"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "BikeTheft",
                            MaxAmount = 4000,
                            Currency = "CHF",
                            GeographicalScope = "Europe",
                            Conditions = ["Police report required", "Bike must be locked"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 150,
                            Currency = "CHF"
                        }
                    },
                    new Contract
                    {
                        ContractId = "CONTRACT-P-5003-BURGLARY",
                        ProductType = "Property",
                        SubType = "Burglary",
                        Status = "Active",
                        Period = new ContractPeriod
                        {
                            StartDate = "2023-03-10",
                            EndDate = "2025-03-10"
                        },
                        Coverage = new CoverageInfo
                        {
                            CoverageName = "Burglary",
                            MaxAmount = 20000,
                            Currency = "CHF",
                            GeographicalScope = "Switzerland",
                            Conditions = ["Police report required", "Inventory list required", "Evidence of forced entry"]
                        },
                        Deductible = new DeductibleInfo
                        {
                            Amount = 400,
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
            // === Original MCP Customers ===
            ["CUST-12345"] = new ClaimHistoryResponse
            {
                CustomerId = "CUST-12345",
                CustomerFraudScore = 45, // Moderate - has 1 fraud flag
                Summary = "Moderate risk customer with 2 claims, 1 flagged for fraud. Previous electronics claim rejected due to inconsistent story.",
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
                CustomerFraudScore = 10, // Low - clean record
                Summary = "Low risk customer with clean claim history. Single approved home contents claim.",
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
                CustomerFraudScore = 5, // Very low - no history
                Summary = "New customer with no claim history. Very low risk profile.",
                Claims = [] // Clean customer with no claim history
            },
            
            // === Workflow Customers (matching Demo11/Demo12 data) ===
            ["CUST-10001"] = new ClaimHistoryResponse
            {
                CustomerId = "CUST-10001",
                CustomerFraudScore = 65, // High - multiple fraud flags and frequent claims
                Summary = "HIGH RISK: Frequent filer with 5 claims in 18 months, including 1 flagged for duplicate pattern. Notable pattern of bike theft claims. Recommend enhanced review.",
                Claims =
                [
                    new Claim
                    {
                        ClaimId = "CLM-10001-05",
                        ContractId = "CONTRACT-P-5001",
                        Type = "Property",
                        SubType = "BikeTheft",
                        DateOfLoss = "2024-12-01",
                        ReportedDate = "2024-12-02",
                        Status = "Approved",
                        PaidAmount = new AmountInfo { Amount = 800, Currency = "CHF" },
                        FraudFlag = false
                    },
                    new Claim
                    {
                        ClaimId = "CLM-10001-04",
                        ContractId = "CONTRACT-P-5001-WD",
                        Type = "Property",
                        SubType = "WaterDamage",
                        DateOfLoss = "2024-09-10",
                        ReportedDate = "2024-09-11",
                        Status = "Approved",
                        PaidAmount = new AmountInfo { Amount = 1500, Currency = "CHF" },
                        FraudFlag = false
                    },
                    new Claim
                    {
                        ClaimId = "CLM-10001-03",
                        ContractId = "CONTRACT-P-5001",
                        Type = "Property",
                        SubType = "BikeTheft",
                        DateOfLoss = "2024-06-15",
                        ReportedDate = "2024-06-16",
                        Status = "Rejected",
                        PaidAmount = new AmountInfo { Amount = 0, Currency = "CHF" },
                        FraudFlag = true // FLAGGED - duplicate pattern
                    },
                    new Claim
                    {
                        ClaimId = "CLM-10001-02",
                        ContractId = "CONTRACT-P-5001-FIRE",
                        Type = "Property",
                        SubType = "Burglary",
                        DateOfLoss = "2023-12-20",
                        ReportedDate = "2023-12-21",
                        Status = "Approved",
                        PaidAmount = new AmountInfo { Amount = 2000, Currency = "CHF" },
                        FraudFlag = false
                    },
                    new Claim
                    {
                        ClaimId = "CLM-10001-01",
                        ContractId = "CONTRACT-P-5001",
                        Type = "Property",
                        SubType = "BikeTheft",
                        DateOfLoss = "2023-08-05",
                        ReportedDate = "2023-08-06",
                        Status = "Approved",
                        PaidAmount = new AmountInfo { Amount = 600, Currency = "CHF" },
                        FraudFlag = false
                    }
                ]
            },
            ["CUST-10002"] = new ClaimHistoryResponse
            {
                CustomerId = "CUST-10002",
                CustomerFraudScore = 20, // Low - clean record, few claims
                Summary = "Low risk customer with 2 claims over 2+ years. Clean history, no fraud indicators.",
                Claims =
                [
                    new Claim
                    {
                        ClaimId = "CLM-10002-02",
                        ContractId = "CONTRACT-A-5002",
                        Type = "Auto",
                        SubType = "Collision",
                        DateOfLoss = "2024-10-05",
                        ReportedDate = "2024-10-06",
                        Status = "Approved",
                        PaidAmount = new AmountInfo { Amount = 3000, Currency = "CHF" },
                        FraudFlag = false
                    },
                    new Claim
                    {
                        ClaimId = "CLM-10002-01",
                        ContractId = "CONTRACT-A-5002-THEFT",
                        Type = "Auto",
                        SubType = "Theft",
                        DateOfLoss = "2022-03-10",
                        ReportedDate = "2022-03-11",
                        Status = "Approved",
                        PaidAmount = new AmountInfo { Amount = 500, Currency = "CHF" },
                        FraudFlag = false
                    }
                ]
            },
            ["CUST-10003"] = new ClaimHistoryResponse
            {
                CustomerId = "CUST-10003",
                CustomerFraudScore = 10, // Very low - first claim scenario
                Summary = "First-time claimant. No historical data for risk assessment. Standard review recommended.",
                Claims =
                [
                    new Claim
                    {
                        ClaimId = "CLM-10003-01",
                        ContractId = "CONTRACT-P-5003",
                        Type = "Property",
                        SubType = "BikeTheft",
                        DateOfLoss = "2024-11-01",
                        ReportedDate = "2024-11-02",
                        Status = "Pending",
                        PaidAmount = new AmountInfo { Amount = 0, Currency = "CHF" },
                        FraudFlag = false
                    }
                ]
            }
        };
    }

    private static Dictionary<string, SuspiciousClaimsResponse> InitializeSuspiciousClaims()
    {
        return new Dictionary<string, SuspiciousClaimsResponse>
        {
            // === Original MCP Customers ===
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
            },
            
            // === Workflow Customers ===
            ["CUST-10001"] = new SuspiciousClaimsResponse
            {
                CustomerId = "CUST-10001",
                SuspiciousClaims =
                [
                    new SuspiciousClaim
                    {
                        ClaimId = "CLM-10001-03",
                        ContractId = "CONTRACT-P-5001",
                        Type = "Property",
                        SubType = "BikeTheft",
                        DateOfLoss = "2024-06-15",
                        Status = "Rejected",
                        SuspicionScore = 0.75,
                        ReasonCodes = ["DUPLICATE_CLAIM_PATTERN", "FREQUENT_SAME_TYPE_CLAIMS", "SHORT_TIME_INTERVAL"],
                        InvestigationStatus = "Closed"
                    }
                ],
                Summary = new SuspicionSummary
                {
                    TotalSuspiciousClaims = 1,
                    MaxSuspicionScore = 0.75,
                    HasPriorFraudInvestigation = true
                }
            },
            ["CUST-10002"] = new SuspiciousClaimsResponse
            {
                CustomerId = "CUST-10002",
                SuspiciousClaims = [],
                Summary = new SuspicionSummary
                {
                    TotalSuspiciousClaims = 0,
                    MaxSuspicionScore = 0.0,
                    HasPriorFraudInvestigation = false
                }
            },
            ["CUST-10003"] = new SuspiciousClaimsResponse
            {
                CustomerId = "CUST-10003",
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
