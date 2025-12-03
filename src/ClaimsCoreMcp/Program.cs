using ClaimsCoreMcp.Tools;
using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure MCP Server with HTTP/SSE transport
builder.Services.AddMcpServer(options =>
{
    options.ServerInfo = new()
    {
        Name = "claims-core-mcp",
        Version = "1.0.0"
    };
})
.WithHttpTransport()
.WithToolsFromAssembly();

// Add CORS for development/testing
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

// Map MCP endpoints (exposes /sse for HTTP transport)
app.MapMcp();

// Health check endpoint
app.MapGet("/", () => Results.Ok(new
{
    Name = "claims-core-mcp",
    Description = "Backend MCP that exposes core claims and policy data for the Agent Framework: customer profile, contracts, claim history, and suspicious-claim signals.",
    Version = "1.0.0",
    Status = "Running",
    Endpoints = new
    {
        Mcp = "/sse",
        Health = "/"
    }
}));

app.Run();
