# Claims Demo - AI-Powered Workflow Orchestration

This folder contains self-contained demos for claims processing workflows using Microsoft Agents AI Framework.

## Overview

The ClaimsDemo suite demonstrates end-to-end claims processing with AI agents:

- **Demo 11**: Claims Intake Workflow - Conversational claim gathering and validation
- **Demo 12**: Fraud Detection Workflow - Multi-agent fraud analysis with fan-out/fan-in patterns

## Architecture

### Demo 11: Claims Intake Workflow

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

### Demo 12: Fraud Detection Workflow

```
????????????????????
? DataReview       ? ? Initial quality check
????????????????????
         ?
????????????????????
? Classification   ? ? Route by claim type
????????????????????
         ?
????????????????????
? PropertyTheft    ? ? Fan-out to 3 parallel agents
? FanOut           ?
????????????????????
     ???????????????????????????????????
     ?                ?                ?
???????????    ???????????    ???????????
? OSINT   ?    ? User    ?    ? Trans-  ?
? Agent   ?    ? History ?    ? action  ?
???????????    ???????????    ???????????
     ????????????????????????????????
                      ?
           ???????????????????
           ? Fraud           ? ? Collect findings (fan-in)
           ? Aggregator      ?
           ???????????????????
                    ?
           ???????????????????
           ? Fraud           ? ? Make decision
           ? Decision        ?
           ???????????????????
                    ?
           ???????????????????
           ? Outcome         ? ? Generate report
           ? Presenter       ?
           ???????????????????
```

## Files

### Core Files

- **`AzureOpenAIConfig.cs`**: Configuration helper for Azure OpenAI credentials
- **`SharedClaimsData.cs`**: Shared data contracts used across all demos
- **`ClaimsMockTools.cs`**: Mock tool implementations for customer/contract lookup

### Demo Files

- **`Demo11_ClaimsWorkflow.cs`**: Claims intake workflow implementation
- **`Demo12_ClaimsFraudDetection.cs`**: Fraud detection workflow implementation

## Setup

### Prerequisites

1. .NET 10 SDK
2. Azure OpenAI account with deployed model
3. Azure credentials configured (DefaultAzureCredential)

### Environment Variables

Set the following environment variables before running:

```bash
# Required
setx AZURE_OPENAI_ENDPOINT "https://your-resource.openai.azure.com/"
setx AZURE_OPENAI_API_KEY "your-api-key-here"
```

### Authentication

The demos use `AzureKeyCredential` for authentication. Simply provide your API key via the `AZURE_OPENAI_API_KEY` environment variable.

## Running the Demos

### From the Main Application

Run the main application and select from the menu:

```bash
cd src/ClaimsCore.Workflows
dotnet run
```

Then choose:
- `1` for Demo 11 (Claims Intake)
- `2` for Demo 12 (Fraud Detection)
- `q` to quit

### Programmatically

```csharp
using ClaimsCore.Workflows.ClaimsDemo;

// Run Demo 11
await Demo11_ClaimsWorkflow.Execute();

// Run Demo 12
await Demo12_ClaimsFraudDetection.Execute();
```

## Demo 11: Claims Intake

### Features

- ? Conversational claims intake
- ? Customer identification (by ID or name)
- ? Contract resolution and validation
- ? Structured feedback loops
- ? Self-contained workflow (no external chat loop)
- ? DevUI compatible

### Example Interaction

```
You: My bike was stolen today
Agent: I'm sorry to hear about that. Could you tell me your name?

You: John Smith
Agent: Thank you, John. I found your account. When did this happen?

You: This morning around 10am
Agent: Got it. Can you describe the bike?

You: Trek X-Caliber 8, red mountain bike, 21-speed
Agent: And where was it stolen from?

You: From outside the grocery store. It was locked with a cable lock.
Agent: ? Thank you. I have all the information needed. Let me validate this...
```

## Demo 12: Fraud Detection

### Features

- ? Parallel fraud analysis (fan-out/fan-in pattern)
- ? OSINT validation (online marketplace checks)
- ? Customer history analysis
- ? Transaction fraud scoring
- ? Polymorphic aggregator (handles multiple finding types)
- ? AI-powered fraud decision with confidence scores
- ? Professional email generation for case handlers

### Analysis Components

1. **Data Review**: Initial quality and completeness check
2. **OSINT Agent**: Checks if stolen items are listed online
3. **User History Agent**: Analyzes claim patterns and fraud score
4. **Transaction Agent**: Scores transaction for fraud indicators
5. **Fraud Decision Agent**: Makes final determination with reasoning
6. **Outcome Presenter**: Generates professional case summary

### Example Output

```
FRAUD DETERMINATION: LIKELY FRAUD
Confidence: 87%
Recommendation: INVESTIGATE

Key Factors:
- High customer fraud score (65)
- Item found on online marketplaces (fraud indicator: 85)
- Pattern of frequent bike theft claims
- High value claim filed immediately after incident
```

## Mock Tools

Both demos use mock tools from `ClaimsMockTools.cs`:

### Claims Intake Tools (Demo 11)

- `GetCurrentDate()`: Returns current date/time
- `GetCustomerProfile(firstName, lastName)`: Looks up customer by name
- `GetContract(customerId)`: Retrieves customer contracts

### Fraud Detection Tools (Demo 12)

- `CheckOnlineMarketplaces(itemDescription, itemValue)`: OSINT marketplace check
- `GetCustomerClaimHistory(customerId)`: Returns claim history with fraud scoring
- `GetTransactionRiskProfile(claimAmount, dateOfLoss)`: Transaction fraud analysis

### Mock Customer Data

The demos include 3 test customers:

| Customer ID | Name | Fraud Score | Notes |
|-------------|------|-------------|-------|
| CUST-10001 | John Smith | 65 (HIGH) | 5 claims, 1 flagged |
| CUST-10002 | Jane Doe | 20 (LOW) | 2 claims, clean record |
| CUST-10003 | Alice Johnson | 10 (VERY LOW) | First-time filer |

## Integration with MCP Server

These demos can be integrated with the ClaimsCoreMcp server for production use. The MCP server provides:

- Real customer data from database
- Actual contract information
- Historical claim records
- Production fraud detection logic

See `../ClaimsCoreMcp/README.md` for MCP server setup.

## Key Patterns Demonstrated

### Demo 11 Patterns

1. **Self-Contained Workflow**: No external chat loop needed
2. **UserInputExecutor**: Handles all user interaction within workflow
3. **Conversational State Management**: Maintains context across iterations
4. **Structured Feedback Loops**: Clear routing between agents
5. **Safety Caps**: Max iteration limits to prevent infinite loops

### Demo 12 Patterns

1. **Fan-Out/Fan-In**: Parallel execution with result aggregation
2. **Polymorphic Handlers**: Single aggregator handles multiple finding types
3. **Data via Messages**: No state operations in fan-out executors
4. **State in Aggregator**: Centralized state management after collection
5. **AI Decision Making**: Confidence scoring and reasoning

## Troubleshooting

### Configuration Errors

**Error**: "AZURE_OPENAI_ENDPOINT environment variable not set"

**Solution**: Set the environment variable and restart your terminal/IDE:
```bash
setx AZURE_OPENAI_ENDPOINT "https://your-resource.openai.azure.com/"
setx AZURE_OPENAI_API_KEY "your-api-key-here"
```

### Authentication Errors

**Error**: "AZURE_OPENAI_API_KEY environment variable not set"

**Solution**: Set your API key:
```bash
setx AZURE_OPENAI_API_KEY "your-api-key-here"
```

### Build Errors

**Error**: "The type or namespace name 'X' could not be found"

**Solution**: Restore NuGet packages:
```bash
dotnet restore
dotnet build
```

## License

SPDX-License-Identifier: LicenseRef-MAFPlayground-NPU-1.0-CH  
Copyright (c) 2025 Jose Luis

## References

- [Microsoft Agents AI Framework](https://github.com/microsoft/agents)
- [Azure OpenAI Documentation](https://learn.microsoft.com/azure/ai-services/openai/)
- [Model Context Protocol](https://modelcontextprotocol.io/)
