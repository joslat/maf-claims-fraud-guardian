# Claims Core MCP Server

Backend MCP (Model Context Protocol) server that exposes core claims and policy data for the Microsoft Agent Framework. This server provides customer profile, contracts, claim history, and suspicious-claim signals via HTTP/SSE transport.

## Features

- **HTTP Transport**: Hosted as an ASP.NET Core web application, accessible over HTTP
- **MCP Inspector Compatible**: Test via MCP Inspector at `http://localhost:5050`
- **Production Ready**: Can be deployed to Azure App Service, Container Apps, or any cloud provider

## Tools Exposed

### 1. `get_customer_profile`

Resolve a customer and get their basic profile + the `customer_id` used in all other calls.

**Inputs** (one of):
- `customer_id`: The customer ID (e.g., "CUST-12345")
- `first_name` + `last_name`: Customer name combination

**Response**: Customer profile with contact info and address

### 2. `get_contract`

Return the active (and optionally past) contracts for a given customer, including coverage and deductible info.

**Input**: `customer_id`

**Response**: List of contracts with coverage details, conditions, and deductibles

### 3. `get_claim_history`

Return all past claims for the customer with basic metadata and statuses. Used for risk and behavior analysis.

**Input**: `customer_id`

**Response**: List of all claims with status, amounts, and fraud flags

### 4. `get_suspicious_claims`

Return only the claims that are considered suspicious or flagged for fraud, plus a simple summary for the Fraud agents.

**Input**: `customer_id`

**Response**: Suspicious claims with suspicion scores, reason codes, and summary

## Typical Flow

1. Look up the customer via `get_customer_profile` (by name or ID)
2. Use the returned `customer_id` with the other three functions
3. Check contracts, claim history, and suspicious claims as needed

## Getting Started

### Prerequisites

- .NET 10 SDK (or latest preview)
- Visual Studio 2022 or VS Code

### Run Locally

```bash
cd src/ClaimsCoreMcp/ClaimsCoreMcp
dotent run
```

The server will start on:
- HTTP: `http://localhost:5050`
- MCP Endpoint: `http://localhost:5050` (root path)
- Health Check: `http://localhost:5050/health`

### Test with MCP Inspector

1. Start the server with `dotnet run`
2. Open MCP Inspector
3. Connect to `http://localhost:5050`
4. Browse and test the available tools

### Test with curl

```bash
# Health check
curl http://localhost:5050/health

# MCP endpoint (for MCP clients)
curl http://localhost:5050/
```

## Sample Data

The server includes mock data for testing:

| Customer ID | Name | Segment |
|-------------|------|---------|
| CUST-12345 | Jose Latorre | Retail |
| CUST-67890 | Maria Garcia | Premium |
| CUST-11111 | Hans Mueller | Retail |

## Deployment

### Azure App Service

```bash
dotnet publish -c Release
# Deploy to Azure App Service
az webapp deploy --resource-group <rg> --name <app-name> --src-path ./publish
```

### Docker

Build and run the MCP server in a container:

```bash
# Build the Docker image
cd C:\MAF\maf-claims-fraud-guardian\src\ClaimsCoreMcp
docker build -t claims-core-mcp:latest .

# Run the container
docker run -d -p 5050:8080 --name claims-core-mcp claims-core-mcp:latest

# Test the MCP server
curl http://localhost:5050/health

# View logs
docker logs claims-core-mcp

# Stop and remove
docker stop claims-core-mcp
docker rm claims-core-mcp
```

The Dockerfile uses a multi-stage build with .NET 10 SDK for building and .NET 10 runtime for deployment.

### Local with ngrok (for public URL)

```bash
dotnet run
ngrok http 5050
```

## Configuration

### MCP Client Configuration (e.g., Claude Desktop)

Add to your MCP client configuration:

```json
{
  "mcpServers": {
    "claims-core-mcp": {
      "url": "http://localhost:5050"
    }
  }
}
```

## Tech Stack

- ASP.NET Core Minimal API
- ModelContextProtocol.AspNetCore (Official MCP C# SDK)
- HTTP Transport for internet accessibility

## License

MIT License - Part of the MAF Claims Fraud Guardian demo project.
