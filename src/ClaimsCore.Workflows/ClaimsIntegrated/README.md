# Claims Integrated - Demos with ClaimsCore.Common Models and ClaimsCoreMcp Tools

This folder contains integrated versions of the Claims demos that use:
- **ClaimsCore.Common** data models (CustomerInfo, ClaimDraft, ValidationResult, FraudAnalysisState, etc.)
- **ClaimsCoreMcp.Tools.ClaimsTools** for data access (GetCustomerProfile, GetContract, CheckOnlineMarketplaces, etc.)

## Overview

This folder contains two integrated demos:

1. **Demo11_ClaimsWorkflow_Integrated.cs** - Claims intake workflow
2. **Demo12_ClaimsFraudDetection_Integrated.cs** - Fraud detection workflow

Both maintain the same workflow structure as their originals while integrating with ClaimsCoreMcp server architecture.

### Key Differences from Original Demos

| Aspect | Original Demos | Integrated Demos |
|--------|----------------|------------------|
| **Namespace** | `ClaimsCore.Workflows.ClaimsDemo` | `ClaimsCore.Workflows.ClaimsIntegrated` |
| **Data Models** | `SharedClaimsData.cs` (local) | `ClaimsCore.Common.Models` (shared) |
| **Tools** | `ClaimsMockTools` (local) | `ClaimsCoreMcp.Tools.ClaimsTools` (shared) |
| **Data Source** | Hardcoded mock data | `MockClaimsDataService` (via ClaimsTools) |
| **Conversation History** | Tracked in state (Demo11 only) | Not tracked (simplified state) |

## Demos

### Demo11: Claims Intake Workflow (Integrated)

Conversational claims intake workflow that:
- Prompts users for claim information interactively
- Validates customer identity and contract coverage
- Uses natural language processing for data extraction
- Provides structured feedback loops

**Key Models Used**:
- `IntakeDecision` - Intake agent decision output
- `ValidationResult` - Validation agent output
- `ClaimWorkflowState` - Workflow state tracking

### Demo12: Fraud Detection Workflow (Integrated)

Multi-agent fraud detection pipeline that:
- Performs data quality review
- Analyzes claims through 3 parallel fraud detection agents (OSINT, User History, Transaction)
- Uses fan-out/fan-in pattern for parallel processing
- Generates professional fraud analysis reports

**Key Models Used**:
- `FraudAnalysisState` - Fraud workflow state
- `OSINTFinding` - Online marketplace validation
- `UserHistoryFinding` - Customer claim history analysis
- `TransactionFraudFinding` - Transaction-level fraud scoring
- `FraudDecision` - Final fraud determination

## Architecture

```
ClaimsCore.Workflows.ClaimsIntegrated
?
??? Uses Models from: ClaimsCore.Common
?   ??? CustomerInfo, ClaimDraft, ValidationResult
?   ??? IntakeDecision, ClaimReadinessStatus, ClaimWorkflowState
?   ??? FraudAnalysisState, OSINTFinding, UserHistoryFinding,
?       TransactionFraudFinding, FraudDecision
?
??? Calls Tools from: ClaimsCoreMcp.Tools
    ??? ClaimsTools.GetCustomerProfile()
    ??? ClaimsTools.GetContract()
    ??? ClaimsTools.GetCurrentDate()
    ??? ClaimsTools.CheckOnlineMarketplaces()
    ??? ClaimsTools.GetClaimHistory()
    ??? ClaimsTools.GetTransactionRiskProfile()
```

## Workflow Structures

### Demo11: Claims Intake Workflow

```
????????????????????????
? UserInput (Executor) ? ? Prompts user for information
????????????????????????
           ?
????????????????????????
? ClaimsUserFacing     ? ? Gathers customer & claim details
????????????????????????
           ?
   [Has enough info?]
       ?? Yes ? ClaimsReadyForProcessing
       ?? No  ? UserInput (loop for more details)
           ?
??????????????????????????
? ClaimsReadyForProc     ? ? Validates & enriches claim
??????????????????????????
           ?
   [Claim complete?]
       ?? Yes ? ClaimsProcessing
       ?? No  ? ClaimsIntake (feedback + more details)
           ?
????????????????????????
? ClaimsProcessing     ? ? Final confirmation & handoff
????????????????????????
```

### Demo12: Fraud Detection Workflow

```
????????????????????????
? DataReview           ? ? Quality check
????????????????????????
           ?
????????????????????????
? ClassificationRouter ? ? Route by claim type
????????????????????????
           ?
????????????????????????
? PropertyTheftFanOut  ? ? Dispatch to 3 agents (Fan-Out)
????????????????????????
           ?
    ??????????????????????????
    ?             ?          ?
??????????  ????????????  ????????????
? OSINT  ?  ? UserHist ?  ? TransFraud?
??????????  ????????????  ????????????
    ????????????????????????????
                 ?
    ??????????????????????
    ? FraudAggregator    ? ? Collect findings (Fan-In)
    ??????????????????????
             ?
    ??????????????????????
    ? FraudDecision      ? ? Final determination
    ??????????????????????
             ?
    ??????????????????????
    ? OutcomePresenter   ? ? Generate report
    ??????????????????????
```

## Running the Demo

### Prerequisites

1. .NET 10 SDK
2. Azure OpenAI account with deployed model
3. Environment variables set:
   ```bash
   setx AZURE_OPENAI_ENDPOINT "https://your-resource.openai.azure.com/"
   setx AZURE_OPENAI_API_KEY "your-api-key-here"
   ```

### Execution

```csharp
using ClaimsCore.Workflows.ClaimsIntegrated;

// Run Demo 11 Integrated
await Demo11_ClaimsWorkflow_Integrated.Execute();

// Run Demo 12 Integrated
await Demo12_ClaimsFraudDetection_Integrated.Execute();
```

Or use the main application menu:
```bash
cd src/ClaimsCore.Workflows
dotnet run

# Select:
# 2 - Demo 11 Integrated
# 4 - Demo 12 Integrated
```

### Test Customers

The integrated demo has access to 6 test customers via `MockClaimsDataService`:

#### Workflow Customers (John, Jane, Alice)

| Name | Customer ID | Fraud Score | Claims | Best For Testing |
|------|-------------|-------------|--------|------------------|
| John Smith | CUST-10001 | 65 (HIGH) | 5 (1 flagged) | High-risk scenario |
| Jane Doe | CUST-10002 | 20 (LOW) | 2 (clean) | Normal customer |
| Alice Johnson | CUST-10003 | 10 (VERY LOW) | 1 (pending) | First-time filer |

#### Original MCP Customers (Jose, Maria, Hans)

| Name | Customer ID | Fraud Score | Claims | Best For Testing |
|------|-------------|-------------|--------|------------------|
| Jose Latorre | CUST-12345 | 45 (MODERATE) | 2 (1 flagged) | Moderate risk |
| Maria Garcia | CUST-67890 | 10 (LOW) | 1 (clean) | Premium customer |
| Hans Mueller | CUST-11111 | 5 (VERY LOW) | 0 | New customer |

### Example Sessions

#### Demo11 Integrated - Claims Intake

```
=== Demo 11 Integrated: Claims Processing Workflow ===

?? Welcome to Claims Intake (Integrated Version)!
Please describe your situation, and I'll help you file a claim.

You: My bike was stolen today
Agent: I'm sorry to hear about your bike. Could you tell me your name?

You: John Smith
Agent: Thank you, Mr. Smith. I found your account. Can you describe the bike?

You: Trek X-Caliber 8, red mountain bike
...
? CLAIM PROCESSED SUCCESSFULLY
```

#### Demo12 Integrated - Fraud Detection

```
=== Demo 12 Integrated: Claims Fraud Detection Workflow ===

FRAUD DETECTION ANALYSIS (Integrated Version)
================================================================================

Analyzing claim:
  Customer: CUST-10001
  Type: Property - BikeTheft
  Amount: $1,200.00

=== Parallel Fraud Detection (Fan-Out) ===
=== OSINT Validation (Online Marketplaces) ===
? OSINT Check Complete - Fraud Score: 85/100

=== User History Analysis ===
? User History Check Complete - Customer Fraud Score: 65/100

=== Transaction Fraud Scoring ===
? Transaction Analysis Complete - Fraud Score: 50/100

=== All Fraud Findings Collected (Fan-In) ===
? All findings stored in shared state!

=== Final Fraud Decision ===
FRAUD DETERMINATION: LIKELY FRAUD
Confidence: 82%
Recommendation: INVESTIGATE

? FRAUD ANALYSIS COMPLETE
```

## Benefits of Integration

### 1. **Shared Data Models**
- Consistent data structures across ClaimsCore.Workflows and ClaimsCoreMcp
- No duplication of model definitions
- Type safety across project boundaries

### 2. **Reusable Tools**
- Same tools used by workflow and MCP server
- Single source of truth for business logic
- Easy to switch from mock to real data services

### 3. **Future-Ready Architecture**
- When ClaimsCoreMcp is deployed as a server, workflows can call it via HTTP/MCP
- For now, direct method calls provide the same interface
- Smooth transition from local to remote data access

## Implementation Notes

### Tool Registration

#### Demo11 Tools

Tools are registered as lambdas that call ClaimsTools directly:

```csharp
var tools = new List<AITool>
{
    AIFunctionFactory.Create(
        (string? customerId, string? firstName, string? lastName) => 
            ClaimsTools.GetCustomerProfile(customerId, firstName, lastName),
        name: "get_customer_profile",
        description: "..."
    ),
    AIFunctionFactory.Create(
        (string customerId) => ClaimsTools.GetContract(customerId),
        name: "get_contract",
        description: "..."
    ),
    AIFunctionFactory.Create(
        () => ClaimsTools.GetCurrentDate(),
        name: "get_current_date",
        description: "..."
    )
};
```

#### Demo12 Tools

Fraud detection tools registered similarly:

```csharp
var tools = new List<AITool>
{
    AIFunctionFactory.Create(
        (string itemDescription, decimal itemValue) => 
            ClaimsTools.CheckOnlineMarketplaces(itemDescription, itemValue),
        name: "check_online_marketplaces",
        description: "..."
    ),
    AIFunctionFactory.Create(
        (string customerId) => ClaimsTools.GetClaimHistory(customerId),
        name: "get_customer_claim_history",
        description: "..."
    ),
    AIFunctionFactory.Create(
        (decimal claimAmount, string dateOfLoss) => 
            ClaimsTools.GetTransactionRiskProfile(claimAmount, dateOfLoss),
        name: "get_transaction_risk_profile",
        description: "..."
    )
};
```

### State Management

Both integrated demos use simplified state models from `ClaimsCore.Common`:

#### Demo11 State

```csharp
public class ClaimWorkflowState
{
    public int IntakeIteration { get; set; }
    public ClaimReadinessStatus Status { get; set; }
    public CustomerInfo? Customer { get; set; }
    public ClaimDraft ClaimDraft { get; set; }
    public string? ContractId { get; set; }
}
```

#### Demo12 State

```csharp
public class FraudAnalysisState
{
    public ValidationResult? OriginalClaim { get; set; }
    public DataReviewResult? DataReview { get; set; }
    public string ClaimType { get; set; }
    public string ClaimSubType { get; set; }
    
    // Fraud findings from parallel agents
    public OSINTFinding? OSINTFinding { get; set; }
    public UserHistoryFinding? UserHistoryFinding { get; set; }
    public TransactionFraudFinding? TransactionFraudFinding { get; set; }
    
    // Final decision
    public FraudDecision? FraudDecision { get; set; }
}
```

**Note**: Conversation history tracking was removed from ClaimWorkflowState to avoid circular dependencies. It can be added back in the workflow executors if needed for audit purposes.

## Next Steps

### To Convert to Remote MCP Calls

When ClaimsCoreMcp is deployed as a server, replace direct method calls with MCP client calls:

```csharp
// Current (Direct):
ClaimsTools.GetCustomerProfile(customerId, firstName, lastName)
ClaimsTools.CheckOnlineMarketplaces(itemDescription, itemValue)

// Future (MCP Client):
await mcpClient.CallToolAsync("get_customer_profile", new { 
    customer_id = customerId,
    first_name = firstName,
    last_name = lastName
})
await mcpClient.CallToolAsync("check_online_marketplaces", new {
    item_description = itemDescription,
    item_value = itemValue
})
```

### To Add More Integrations

1. Create integrated versions of future demos following the same pattern
2. Use shared models from `ClaimsCore.Common.Models`
3. Call tools from `ClaimsCoreMcp.Tools.ClaimsTools`
4. Add to the menu in `Program.cs`

## Project References

- **ClaimsCore.Common**: Shared models (CustomerInfo, ClaimDraft, etc.)
- **ClaimsCoreMcp**: Tools and data services (ClaimsTools, MockClaimsDataService)

## See Also

- [Original Demo11](../ClaimsDemo/Demo11_ClaimsWorkflow.cs) - Original claims intake implementation
- [Original Demo12](../ClaimsDemo/Demo12_ClaimsFraudDetection.cs) - Original fraud detection implementation
- [ClaimsCoreMcp README](../../ClaimsCoreMcp/README.md) - MCP server documentation
- [ClaimsCore.Common Models](../../ClaimsCore.Common/Models/) - Shared data models

## Summary

? **Integrated with ClaimsCore.Common models**  
? **Uses ClaimsCoreMcp tools for data access**  
? **Demo11: Claims intake workflow**  
? **Demo12: Fraud detection workflow**  
? **Maintains same workflow structures as originals**  
? **Ready for future MCP server integration**  
? **Simplified state management**  

These integrated versions demonstrate how to build workflows that leverage shared models and tools, providing a foundation for scalable, maintainable claims processing and fraud detection systems.
