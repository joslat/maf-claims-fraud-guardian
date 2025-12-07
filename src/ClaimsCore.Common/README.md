# ClaimsCore.Common

**Shared Data Models for Claims Processing System**

This library contains all shared data contracts used across the Claims Processing ecosystem, including:
- MCP Server (ClaimsCoreMcp)
- Workflow Demos (Demo11 - Claims Intake, Demo12 - Fraud Detection)
- Future integrations

---

## ?? Model Organization

Models are organized by domain in folders for clarity:

### `Models/Customer/`
Customer identification and profile information:
- `CustomerProfile` - Complete customer profile with contact and address
- `CustomerInfo` - Simplified customer identification
- `ContactInfo`, `AddressInfo` - Nested contact/address details

### `Models/Claims/`
Claim data structures:
- `Claim` - Individual claim record
- `ClaimHistoryResponse` - Customer claim history with computed fields
- `ClaimDraft` - Claim data gathered during intake
- `AmountInfo` - Monetary amount representation
- `SuspiciousClaim`, `SuspiciousClaimsResponse` - Fraud-flagged claims

### `Models/Contracts/`
Insurance contract structures:
- `Contract` - Insurance contract details
- `ContractResponse` - Customer contract collection
- `ContractPeriod` - Contract validity period
- `CoverageInfo` - Coverage details and conditions
- `DeductibleInfo` - Deductible information

### `Models/Workflow/`
Workflow orchestration models:
- `ValidationResult` - Validated claim ready for processing (Demo11?Demo12 handoff)
- `IntakeDecision` - Intake agent decision output
- `ProcessedClaim` - Final processed claim
- `ClaimReadinessStatus` - Status enum

### `Models/Fraud/`
Fraud detection models:
- `DataReviewResult` - Initial data quality review
- `OSINTFinding` - OSINT (marketplace) findings
- `UserHistoryFinding` - Customer history analysis
- `TransactionFraudFinding` - Transaction risk scoring
- `FraudDecision` - Final fraud determination
- `FraudAnalysisState` - Complete fraud analysis state
- `MarketplaceCheckResponse` - Marketplace check results
- `TransactionRiskProfile` - Transaction risk profile

---

## ?? Namespace

All models use a single flat namespace for simplicity:

```csharp
using ClaimsCore.Common.Models;
```

Folder organization is for file management only—no nested namespaces.

---

## ?? Usage

### Add Reference

In your project file (`.csproj`):

```xml
<ItemGroup>
  <ProjectReference Include="..\ClaimsCore.Common\ClaimsCore.Common.csproj" />
</ItemGroup>
```

### Import Models

```csharp
using ClaimsCore.Common.Models;

// Use models
var customer = new CustomerProfile
{
    CustomerId = "CUST-12345",
    FirstName = "John",
    LastName = "Smith",
    // ...
};

var claim = new ClaimDraft
{
    ClaimType = "Property",
    ClaimSubType = "BikeTheft",
    // ...
};
```

---

## ? Key Features

- **100% Workflow Compatible**: Matches all workflow data contracts exactly
- **JSON Serialization**: All models have `[JsonPropertyName]` attributes
- **Documentation**: XML documentation on all models and properties
- **Type Safety**: Nullable reference types enabled
- **Computed Properties**: Derived fields (e.g., `ClaimHistoryResponse.SuspiciousActivityDetected`)
- **Alias Properties**: Multiple property names for backward compatibility

---

## ?? Data Flow

```
Demo11 (Claims Intake)
  ? uses CustomerInfo, ClaimDraft, IntakeDecision
  ? produces ValidationResult
  ?
Demo12 (Fraud Detection)
  ? consumes ValidationResult
  ? uses OSINTFinding, UserHistoryFinding, TransactionFraudFinding
  ? produces FraudDecision
```

---

## ?? Model Count

- **Customer Models**: 4 (CustomerProfile, CustomerInfo, ContactInfo, AddressInfo)
- **Claims Models**: 4 (Claim, ClaimHistoryResponse, ClaimDraft, AmountInfo + Suspicious)
- **Contract Models**: 5 (Contract, ContractResponse, ContractPeriod, CoverageInfo, DeductibleInfo)
- **Workflow Models**: 4 (ValidationResult, IntakeDecision, ProcessedClaim, ClaimReadinessStatus)
- **Fraud Models**: 8 (DataReviewResult, OSINTFinding, UserHistoryFinding, TransactionFraudFinding, FraudDecision, FraudAnalysisState, MarketplaceCheckResponse, TransactionRiskProfile)

**Total**: **25+ models** across **5 domains**

---

## ?? Version

- **Target Framework**: .NET 10
- **Language Version**: C# 14
- **Compatibility**: 100% with MAFPlayground.Demos.ClaimsDemo workflows

---

## ?? License

Part of the MAF Claims Fraud Guardian project.
