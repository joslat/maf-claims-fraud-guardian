# MAF Claims Fraud Guardian

A comprehensive demonstration of the **Microsoft Agent Framework (MAF)** for intelligent insurance claims processing and fraud detection using AI-powered multi-agent workflows.

## 🎯 Project Overview

This project showcases a complete end-to-end claims management system with:

1. **Claims Intake Workflow** - Interactive conversational claims processing
2. **Claims Fraud Detection Workflow** - Parallel multi-agent fraud analysis
3. **Shared Data Models** - Unified domain models across the system
4. **MCP Backend** - Model Context Protocol server for data access

### Key Scenarios

- 🚲 **Scenario 1 (High Risk)**: Rare Hello Kitty mountain bike theft (CHF 15,000)
- 📱 **Scenario 2 (Low Risk)**: Mobile phone theft from gym locker (CHF 850)
- 🚗 **Scenario 3 (Moderate Risk)**: Car theft from residential street (CHF 38,000)

---

## 📚 Architecture

### Three Core Projects

```
maf-claims-fraud-guardian/
├── src/
│   ├── ClaimsCore.Workflows/      # Main workflow application
│   ├── ClaimsCoreMcp/              # Backend MCP Server
│   └── ClaimsCore.Common/          # Shared data models
```

---

## 🔧 System Components

### 1. Claims Core Workflows (`ClaimsCore.Workflows`)

The main application with two production workflows:

#### **Claims Intake Workflow**
Interactive conversational workflow that guides customers through claims filing.

**Features:**
- 🤖 Conversational AI intake with natural language understanding
- 👤 Customer identification (by ID or name lookup)
- 📋 Contract resolution and validation
- 🔄 Iterative refinement with feedback loops
- ✅ Claim composition and validation

**Agents:**
1. **ClaimsUserFacingAgent** - Friendly intake specialist
   - Gathers claim details conversationally
   - Uses tools to resolve customer and contract
   - Handles natural date formats

2. **ClaimsReadyForProcessingAgent** - Validation specialist
   - Validates all required fields
   - Normalizes claim types
   - Provides structured feedback for missing info

3. **ClaimsProcessingAgent** - Processing specialist
   - Generates claim IDs
   - Confirms details
   - Provides next steps

**How to Run:**
```bash
cd src/ClaimsCore.Workflows
dotnet run
# Select option: 1 - Claims Intake Workflow
```

---

#### **Claims Fraud Detection Workflow**
Parallel multi-agent workflow that analyzes claims for fraud patterns.

**Features:**
- 🔍 Data quality review before analysis
- 📊 Classification routing by claim type
- ⚡ Parallel fraud detection (fan-out/fan-in pattern)
- 🤝 Polymorphic findings aggregation
- 📧 Professional fraud determination and reporting

**Architecture:**
```
DataReviewExecutor
    ↓
ClassificationRouter
    ↓
PropertyTheftFanOut (fan-out)
    ├─→ OSINTExecutor
    ├─→ UserHistoryExecutor
    └─→ TransactionFraudExecutor
    ↓
FraudAggregatorExecutor (fan-in)
    ↓
FraudDecisionExecutor
    ↓
OutcomePresenterExecutor
```

**Parallel Agents:**

1. **OSINT Validator Agent** - Online marketplace intelligence
   - Searches Swiss & international marketplaces (ricardo.ch, anibis.ch, eBay)
   - Detects if stolen items are being resold
   - Fraud score: 0-100

2. **User History Agent** - Customer analysis
   - Analyzes claim history and patterns
   - Detects suspicious claim frequency
   - Previous fraud flags
   - Customer fraud score: 0-100

3. **Transaction Fraud Agent** - Transaction risk analysis
   - Analyzes claim amounts vs typical patterns
   - Detects timing anomalies
   - Description quality checks
   - Transaction fraud score: 0-100

4. **Fraud Decision Agent** - Final determination
   - Synthesizes all three findings
   - Makes fraud/no-fraud decision
   - Provides confidence score and recommendation

5. **Outcome Presenter Agent** - Professional reporting
   - Generates formal email to case handler
   - Executive summary of findings
   - Clear next steps

**How to Run:**
```bash
cd src/ClaimsCore.Workflows
dotnet run
# Select option: 2 - Claims Fraud Detection Workflow
# Choose scenario: 1 (High Risk), 2 (Low Risk), or 3 (Moderate Risk)
```

**DevUI Mode:**
```bash
cd src/ClaimsCore.Workflows
dotnet run
# Select option: 3 - Claims Fraud Detection with DevUI
# Open browser to http://localhost:5173/devui
```

---

### 2. Claims Core MCP Server (`ClaimsCoreMcp`)

Backend service that provides all claims data via the Model Context Protocol.

**Exposed Tools:**

| Tool | Purpose | Input |
|------|---------|-------|
| `get_customer_profile` | Resolve customer + get profile | customer_id OR first_name + last_name |
| `get_contract` | Get active contracts & coverage | customer_id |
| `get_claim_history` | Get past claims + fraud flags | customer_id |
| `get_suspicious_claims` | Get flagged claims | customer_id |
| `get_current_date` | Get current date/time | None |
| `check_online_marketplaces` | OSINT: Check for resold items | item_description, item_value |
| `get_transaction_risk_profile` | Analyze transaction risk | claim_amount, date_of_loss |

**Sample Data:**

| Customer ID | Name | Type | Notes |
|-------------|------|------|-------|
| CUST-10001 | John Smith | Retail | High fraud score (65), 5 claims |
| CUST-67890 | Maria Garcia | Premium | Low fraud score (10), clean record |
| CUST-10003 | Alice Johnson | Retail | Low fraud score (10), 1 pending |

**How to Run:**
```bash
cd src/ClaimsCoreMcp
dotnet run
# Server runs on http://localhost:5050
# Test with: curl http://localhost:5050/health
```

**Docker Deployment:**
```bash
cd src/ClaimsCoreMcp
docker build -t claims-core-mcp:latest .
docker run -d -p 5050:8080 --name claims-core-mcp claims-core-mcp:latest
curl http://localhost:5050/health
```

---

### 3. Claims Core Common Models (`ClaimsCore.Common`)

Shared domain models used across workflows and MCP server.

**Data Models:**

**Customer Domain:**
- `CustomerInfo` - Basic customer information
- `CustomerProfile` - Customer with contracts and history

**Claims Domain:**
- `ClaimDraft` - Initial claim information
- `Claim` - Complete claim record
- `ClaimWorkflowState` - State during intake workflow

**Fraud Detection Domain:**
- `FraudAnalysisState` - State during fraud analysis
- `DataReviewResult` - Quality review findings
- `OSINTFinding` - Marketplace search results
- `UserHistoryFinding` - Customer history analysis
- `TransactionFraudFinding` - Transaction risk analysis
- `FraudDecision` - Final fraud determination

**Workflow Domain:**
- `IntakeDecision` - Intake agent decision
- `ValidationResult` - Validation agent result
- `ClaimReadinessStatus` - Enum for claim status

---

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- Azure OpenAI credentials (for AI agents)
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository:**
```bash
git clone https://github.com/joslat/maf-claims-fraud-guardian.git
cd maf-claims-fraud-guardian
```

2. **Set environment variables:**
```bash
# Windows (PowerShell)
$env:AZURE_OPENAI_ENDPOINT = "https://your-resource.openai.azure.com/"
$env:AZURE_OPENAI_API_KEY = "your-api-key-here"

# Or set permanently
setx AZURE_OPENAI_ENDPOINT "https://your-resource.openai.azure.com/"
setx AZURE_OPENAI_API_KEY "your-api-key-here"
```

3. **Run the main application:**
```bash
cd src/ClaimsCore.Workflows
dotnet run
```

4. **Select a demo:**
   - `1` - Claims Intake Workflow
   - `2` - Claims Fraud Detection Workflow
   - `3` - Claims Fraud Detection with DevUI
   - `4` - Sample: Agent Workflow with DevUI

---

## 💡 Example Workflows

### Claims Intake Flow

```
User Input
    ↓
ClaimsUserFacingAgent
    - Asks for customer ID or name
    - Asks for claim details
    - Calls get_current_date, get_customer_profile, get_contract
    ↓
ClaimsReadyForProcessingAgent
    - Validates all required fields
    - Resolves customer if needed
    - Fetches contract details
    ↓
ClaimsProcessingAgent
    - Generates claim ID
    - Confirms claim details
    ↓
Output: Confirmation Email
```

### Fraud Detection Flow

```
Input: Validated Claim
    ↓
DataReviewExecutor
    - Checks completeness (0-100 score)
    ↓
ClassificationRouter
    - Routes by claim type (e.g., PropertyTheft)
    ↓
PARALLEL:
├─ OSINTExecutor → Marketplace Search → Fraud Score
├─ UserHistoryExecutor → Customer History → Fraud Score
└─ TransactionFraudExecutor → Amount/Timing Analysis → Fraud Score
    ↓
FraudAggregatorExecutor
    - Collects all 3 findings
    - Stores in shared state
    ↓
FraudDecisionExecutor
    - Synthesizes findings
    - Makes final determination
    ↓
OutcomePresenterExecutor
    - Generates professional email
    ↓
Output: Fraud Determination Email
```

---

## 🏗️ Key Architecture Patterns

### 1. Multi-Agent Orchestration
- **ReflectingExecutor**: Automatic message routing based on handler signatures
- **Agents**: ChatClientAgent with specialized instructions and tools
- **Conversation Memory**: Thread-based conversation state per agent

### 2. Fan-Out/Fan-In Pattern
```csharp
.AddFanOutEdge(propertyTheftFanOut, targets: [osint, userHistory, transaction])
.AddFanInEdge(fraudAggregator, sources: [osint, userHistory, transaction])
```

### 3. Polymorphic Message Handling
```csharp
// Aggregator handles 3 different finding types
public ValueTask<string> HandleAsync(OSINTFinding finding, ...)
public ValueTask<string> HandleAsync(UserHistoryFinding finding, ...)
public ValueTask<string> HandleAsync(TransactionFraudFinding finding, ...)
```

### 4. Shared State Management
```csharp
private static async Task<T> ReadStateAsync(IWorkflowContext context)
    => await context.ReadStateAsync<T>(Key, scopeName: Scope);

private static ValueTask SaveStateAsync(IWorkflowContext context, T state)
    => context.QueueStateUpdateAsync(Key, state, scopeName: Scope);
```

### 5. Tool Integration
```csharp
AIFunctionFactory.Create(
    (string itemDescription, decimal itemValue) => 
        ClaimsTools.CheckOnlineMarketplaces(itemDescription, itemValue),
    name: "check_online_marketplaces",
    description: "Check if stolen property is listed for sale"
)
```

---

## 📖 Documentation

- **[Claims Workflows README](./src/ClaimsCore.Workflows/Workflows/README.md)** - Detailed workflow documentation
- **[MCP Server README](./src/ClaimsCoreMcp/README.md)** - Backend service guide
- **[TODO.md](./src/todo.md)** - Project roadmap and pending items

---

## 🧪 Testing Scenarios

### Scenario 1: High Risk (Hello Kitty Bike)
- **Customer**: John Smith (CUST-10001)
- **Item**: Rare Hello Kitty mountain bike
- **Value**: CHF 15,000
- **Fraud Score**: 65 (likely fraud)
- **Expected**: INVESTIGATE

### Scenario 2: Low Risk (Samsung Phone)
- **Customer**: Maria Garcia (CUST-67890)
- **Item**: Samsung Galaxy S23
- **Value**: CHF 850
- **Fraud Score**: 10 (clean)
- **Expected**: APPROVE

### Scenario 3: Moderate Risk (VW Golf)
- **Customer**: Alice Johnson (CUST-10003)
- **Item**: 2022 Volkswagen Golf GTI
- **Value**: CHF 38,000
- **Fraud Score**: 30 (borderline)
- **Expected**: INVESTIGATE

---

## 🔄 Workflow Execution

### Console Mode
Interactive text-based workflow execution with real-time agent output streaming.

### DevUI Mode
Browser-based workflow visualization with:
- Real-time agent output display
- Workflow state inspection
- Step-by-step execution control
- OpenAI API endpoint for external clients

---

## 🛠️ Technologies

- **Framework**: Microsoft Agent Framework (MAF)
- **.NET**: .NET 10
- **AI**: Azure OpenAI (GPT-4o)
- **Protocol**: Model Context Protocol (MCP)
- **Transport**: HTTP/SSE for MCP
- **Patterns**: Fan-out/fan-in, polymorphic handlers, shared state

---

## 📋 Data Flow

### 1. Claims Intake
```
Customer → UserInput → ClaimsUserFacingAgent → ClaimDraft
         → ClaimsReadyForProcessingAgent → ValidationResult
         → ClaimsProcessingAgent → Confirmation Email
```

### 2. Fraud Detection
```
Validated Claim → DataReview → Classification → [Parallel Analysis]
                                                 ├─ OSINT
                                                 ├─ User History
                                                 └─ Transaction Risk
                            → Aggregation → Decision → Email
```

### 3. Tool Access
```
Agents → AIFunctionFactory → ClaimsTools (Direct Calls)
      → MCP Server (via HTTP) [Future Enhancement]
      → MockClaimsDataService (In-Memory Data)
```

---

## 🎓 Learning Path

1. **Start Simple**: Run Claims Intake Workflow (interactive)
2. **Try Parallel**: Run Claims Fraud Detection with High Risk scenario
3. **Visualize**: Run with DevUI to see real-time execution
4. **Examine Code**: Review executor implementations for patterns
5. **Extend**: Add new agents, workflows, or data sources

---

## 🚀 Future Enhancements

See [TODO.md](./src/todo.md) for planned improvements:

### High Priority
- ✅ Rename workflows to production names
- 🟡 Update XML documentation
- 🟡 Improve documentation (ADRs, patterns)
- ⚠️ MCP server toggle (blocked: Docker)

### Medium Priority
- 🟡 Add comprehensive test layer

### Low Priority
- 🟢 State access in fan-out executors
- 🟢 Enhanced DevUI visualizations
- 🟢 Additional documentation

---

## 📞 Support & Resources

- **Repository**: [maf-claims-fraud-guardian](https://github.com/joslat/maf-claims-fraud-guardian)
- **Issues**: Use GitHub Issues for bugs and feature requests
- **Discussions**: Use GitHub Discussions for questions

---

## 📄 License

SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH

Copyright (c) 2025 Jose Luis Latorre

---

## 🎯 Quick Reference

| Task | Command |
|------|---------|
| Run workflows | `cd src/ClaimsCore.Workflows && dotnet run` |
| Run MCP server | `cd src/ClaimsCoreMcp && dotnet run` |
| Run tests | `dotnet test` |
| Build all | `dotnet build` |
| View logs | `dotnet run -- verbose` |
| Deploy MCP | `docker build -t claims-core-mcp . && docker run -p 5050:8080` |

---

**Last Updated**: January 29, 2025  
**Status**: Active Development  
**.NET Target**: .NET 10
