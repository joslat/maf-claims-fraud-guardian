# Claims Integrated - Demo11 with ClaimsCore.Common Models and ClaimsCoreMcp Tools

This folder contains an integrated version of Demo11 that uses:
- **ClaimsCore.Common** data models (CustomerInfo, ClaimDraft, ValidationResult, etc.)
- **ClaimsCoreMcp.Tools.ClaimsTools** for data access (GetCustomerProfile, GetContract, GetCurrentDate)

## Overview

`Demo11_ClaimsWorkflow_Integrated.cs` is a variant of the original Demo11 that demonstrates how to integrate with the ClaimsCoreMcp server architecture while maintaining the same workflow structure.

### Key Differences from Original Demo11

| Aspect | Original Demo11 | Demo11 Integrated |
|--------|----------------|-------------------|
| **Namespace** | `ClaimsCore.Workflows.ClaimsDemo` | `ClaimsCore.Workflows.ClaimsIntegrated` |
| **Data Models** | `SharedClaimsData.cs` (local) | `ClaimsCore.Common.Models` (shared) |
| **Tools** | `ClaimsMockTools` (local) | `ClaimsCoreMcp.Tools.ClaimsTools` (shared) |
| **Data Source** | Hardcoded mock data | `MockClaimsDataService` (via ClaimsTools) |
| **Conversation History** | Tracked in state | Not tracked (simplified state) |

## Architecture

```
ClaimsCore.Workflows.ClaimsIntegrated
?
??? Uses Models from: ClaimsCore.Common
?   ??? CustomerInfo
?   ??? ClaimDraft  
?   ??? ValidationResult
?   ??? IntakeDecision
?   ??? ClaimReadinessStatus
?   ??? ClaimWorkflowState
?
??? Calls Tools from: ClaimsCoreMcp.Tools
    ??? ClaimsTools.GetCustomerProfile()
    ??? ClaimsTools.GetContract()
    ??? ClaimsTools.GetCurrentDate()
```

## Workflow Structure

The workflow structure remains identical to Demo11:

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

await Demo11_ClaimsWorkflow_Integrated.Execute();
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

### Example Session

```
=== Demo 11 Integrated: Claims Processing Workflow ===

This demo uses ClaimsCore.Common models and ClaimsCoreMcp tools.
...

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
    // ... more tools
};
```

### State Management

The integrated version uses a simplified `ClaimWorkflowState` that doesn't track conversation history:

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

Conversation history tracking can be added back in the workflow executors if needed for audit purposes.

## Next Steps

### To Convert to Remote MCP Calls

When ClaimsCoreMcp is deployed as a server, replace direct method calls with MCP client calls:

```csharp
// Current (Direct):
ClaimsTools.GetCustomerProfile(customerId, firstName, lastName)

// Future (MCP Client):
await mcpClient.CallToolAsync("get_customer_profile", new { 
    customer_id = customerId,
    first_name = firstName,
    last_name = lastName
})
```

### To Add More Integrations

1. Create `Demo12_ClaimsFraudDetection_Integrated.cs` following the same pattern
2. Use `ClaimsTools.CheckOnlineMarketplaces()` and `ClaimsTools.GetTransactionRiskProfile()`
3. Leverage shared models from `ClaimsCore.Common.Models`

## Project References

- **ClaimsCore.Common**: Shared models (CustomerInfo, ClaimDraft, etc.)
- **ClaimsCoreMcp**: Tools and data services (ClaimsTools, MockClaimsDataService)

## See Also

- [Original Demo11](../ClaimsDemo/Demo11_ClaimsWorkflow.cs) - Original implementation with local models
- [ClaimsCoreMcp README](../../ClaimsCoreMcp/README.md) - MCP server documentation
- [ClaimsCore.Common Models](../../ClaimsCore.Common/Models/) - Shared data models

## Summary

? **Integrated with ClaimsCore.Common models**  
? **Uses ClaimsCoreMcp tools for data access**  
? **Maintains same workflow structure as Demo11**  
? **Ready for future MCP server integration**  
? **Simplified state management**  

This integrated version demonstrates how to build workflows that leverage shared models and tools, providing a foundation for scalable, maintainable claims processing systems.
