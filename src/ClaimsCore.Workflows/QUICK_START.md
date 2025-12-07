# Quick Start Guide - Claims Core Workflows

## Prerequisites

1. **.NET 10 SDK** - Install from https://dotnet.microsoft.com/download
2. **Azure OpenAI Account** - With a deployed model (e.g., gpt-4o)
3. **Azure Authentication** - Azure CLI installed and logged in

## Setup (5 minutes)

### Step 1: Configure Azure OpenAI

```bash
# Windows PowerShell
setx AZURE_OPENAI_ENDPOINT "https://your-resource.openai.azure.com/"
setx AZURE_OPENAI_API_KEY "your-api-key-here"

# Linux/Mac
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_API_KEY="your-api-key-here"
```

**Note**: Replace `your-resource` with your actual Azure OpenAI resource name and `your-api-key-here` with your API key.

### Step 2: Run the Application

```bash
cd src/ClaimsCore.Workflows
dotnet run
```

## Using the Application

### Main Menu

```
?? Claims Core Workflows Application
=====================================

? Azure OpenAI configuration validated
   Endpoint: https://your-resource.openai.azure.com/
   Deployment: gpt-4o

============================================================
Available Demos:
  1 - Demo 11: Claims Intake Workflow
  2 - Demo 12: Fraud Detection Workflow
  q - Quit
============================================================

Select a demo (1, 2, or q):
```

### Demo 11: Claims Intake Workflow

Interactive conversational workflow that:
- Gathers claim information through natural conversation
- Looks up customer by name
- Validates contract coverage
- Produces a validated claim ready for processing

**Example Session**:
```
You: My bike was stolen
Agent: I'm sorry to hear that. What's your name?

You: John Smith
Agent: Found your account. When did this happen?

You: This morning
Agent: Can you describe the bike?

You: Trek X-Caliber 8, red mountain bike
Agent: ? Claim validated and ready for processing
```

### Demo 12: Fraud Detection Workflow

Automated fraud analysis that:
- Checks if stolen items are listed online (OSINT)
- Analyzes customer claim history
- Scores transaction for fraud indicators
- Makes fraud determination with confidence score

**Example Output**:
```
FRAUD DETERMINATION: INVESTIGATE
Confidence: 87%

Key Factors:
- High customer fraud score (65)
- Item found on online marketplaces
- Pattern of frequent claims
```

## Troubleshooting

### "AZURE_OPENAI_ENDPOINT environment variable not set"

**Solution**: Set both environment variables and restart your terminal:
```bash
setx AZURE_OPENAI_ENDPOINT "https://your-resource.openai.azure.com/"
setx AZURE_OPENAI_API_KEY "your-api-key-here"
# Close and reopen terminal/IDE
```

### "AZURE_OPENAI_API_KEY environment variable not set"

**Solution**: Set your API key:
```bash
setx AZURE_OPENAI_API_KEY "your-api-key-here"
# Close and reopen terminal/IDE
```

### Build Errors

**Solution**: Restore packages and rebuild:
```bash
dotnet restore
dotnet build
```

## Optional Configuration

### Custom Deployment Name

By default, the application uses the "gpt-4o" deployment. To use a different deployment:

```bash
setx AZURE_OPENAI_DEPLOYMENT "your-deployment-name"
```

### Mock Customer Data

The demos include 3 test customers:

| Name | Customer ID | Fraud Risk | Test Scenario |
|------|-------------|------------|---------------|
| John Smith | CUST-10001 | HIGH (65) | Frequent filer, 1 flagged claim |
| Jane Doe | CUST-10002 | LOW (20) | Clean record |
| Alice Johnson | CUST-10003 | VERY LOW (10) | First-time filer |

Use these names in Demo 11 to test different scenarios.

## Architecture

```
???????????????????????????????????????????
?     Claims Core Workflows App           ?
?                                         ?
?  ?????????????      ?????????????     ?
?  ?  Demo 11  ?      ?  Demo 12  ?     ?
?  ?  Claims   ?      ?  Fraud    ?     ?
?  ?  Intake   ?      ?  Detection?     ?
?  ?????????????      ?????????????     ?
?         ?                  ?           ?
?         ????????????????????           ?
?                    ?                   ?
?         ????????????????????          ?
?         ?  ClaimsMockTools ?          ?
?         ?  (Test Data)     ?          ?
?         ????????????????????          ?
???????????????????????????????????????????
                    ?
        ????????????????????????
        ?   Azure OpenAI       ?
        ?   (gpt-4o)           ?
        ????????????????????????
```

## Key Features

? **Self-Contained** - No external project dependencies  
? **Secure** - Uses Azure authentication, no hardcoded secrets  
? **Interactive** - Conversational claim intake  
? **AI-Powered** - Advanced fraud detection with reasoning  
? **Production-Ready** - Can be integrated with ClaimsCoreMcp server  

## Next Steps

1. **Try Demo 11**: File a test claim using one of the mock customers
2. **Try Demo 12**: See how fraud detection analyzes claims
3. **Review Documentation**: See `ClaimsDemo/README.md` for detailed info
4. **Production Integration**: Connect to ClaimsCoreMcp server for real data

## Support

- **Documentation**: See `ClaimsDemo/README.md`
- **Configuration**: See `NORMALIZATION_SUMMARY.md`
- **MCP Server**: See `../ClaimsCoreMcp/README.md`

## Summary

```bash
# Quick start in 2 commands:
setx AZURE_OPENAI_ENDPOINT "https://your-resource.openai.azure.com/"
setx AZURE_OPENAI_API_KEY "your-api-key-here"
dotnet run
```

That's it! Select a demo from the menu and start exploring AI-powered claims workflows.
