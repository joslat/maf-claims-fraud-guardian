// SPDX-License-Identifier: LicenseRef-MAFPlayground-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis

using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.AI.OpenAI;
using ClaimsCore.Common.Models;
using ClaimsCoreMcp.Tools;
using MAFPlayground.Utils;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClaimsCore.Workflows.ClaimsIntegrated;

/// <summary>
/// Demo 12 Integrated: Claims Fraud Detection Workflow with ClaimsCore.Common Models
/// 
/// This is an integrated version of Demo12 that:
/// - Uses ClaimsCore.Common data models (FraudAnalysisState, OSINTFinding, etc.)
/// - Calls ClaimsCoreMcp.Tools.ClaimsTools directly for data access
/// - Maintains the same workflow structure as the original Demo12
/// 
/// Implements a comprehensive fraud detection pipeline that analyzes validated
/// claims through multiple AI agents working in parallel to detect potential fraud indicators.
/// 
/// Key Architecture Features:
/// - Fan-out/fan-in pattern for parallel fraud analysis
/// - Polymorphic aggregator handling multiple finding types
/// - Data passed through messages (NOT context state in fan-out executors)
/// - Context state operations only in aggregator and non-fan-out executors
/// 
/// Takes a validated claim (ValidationResult from Demo11) and processes it through:
/// 1. DataReviewExecutor - Initial data quality and completeness check
/// 2. ClassificationRouter - Routes to appropriate claim type handler
/// 3. PropertyTheftFanOut - Sends claim to 3 parallel fraud detection agents
/// 4. FraudAggregatorExecutor - Collects findings (fan-in), stores in state
/// 5. FraudDecisionAgent - Analyzes aggregated data and makes final determination
/// 6. OutcomePresenterAgent - Generates professional email to case handler
/// 
/// Key Differences from Original Demo12:
/// - Uses real ClaimsCore.Common models instead of SharedClaimsData
/// - Calls ClaimsTools methods directly (CheckOnlineMarketplaces, GetClaimHistory, etc.)
/// - Works with actual data from MockClaimsDataService
/// - Compatible with ClaimsCoreMcp server architecture
/// </summary>
internal static class Demo12_ClaimsFraudDetection_Integrated
{
    // --------------------- Shared state ---------------------
    // Using FraudAnalysisState from ClaimsCore.Common.Models

    private static class FraudStateShared
    {
        public const string Scope = "FraudStateScope";
        public const string Key = "singleton";
    }

    private static async Task<FraudAnalysisState> ReadFraudStateAsync(IWorkflowContext context)
    {
        var state = await context.ReadStateAsync<FraudAnalysisState>(FraudStateShared.Key, scopeName: FraudStateShared.Scope);
        return state ?? new FraudAnalysisState();
    }

    private static ValueTask SaveFraudStateAsync(IWorkflowContext context, FraudAnalysisState state)
        => context.QueueStateUpdateAsync(FraudStateShared.Key, state, scopeName: FraudStateShared.Scope);

    // --------------------- Entry point ---------------------
    public static async Task Execute(int scenario = 1)
    {
        // Set console encoding to UTF-8 to support emojis and special characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Demo 12 Integrated: Claims Fraud Detection Workflow ===\n");
        Console.WriteLine("This demo uses ClaimsCore.Common models and ClaimsCoreMcp tools.\n");
        Console.WriteLine("Features: Polymorphic aggregator, data passing via messages, state in aggregator\n");

        // Azure OpenAI setup
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);
        var deploymentName = "gpt-4o";
        IChatClient chatClient = azureClient.GetChatClient(deploymentName).AsIChatClient();

        // Build workflow
        var workflow = BuildFraudDetectionWorkflow(chatClient);

        WorkflowVisualizerTool.PrintAll(workflow, "Demo 12 Integrated: Claims Fraud Detection Workflow");

        // Get mock claim based on scenario
        var mockClaim = GetMockClaim(scenario);

        Console.WriteLine("\n" + new string('=', 80));
        Console.WriteLine("FRAUD DETECTION ANALYSIS (Integrated Version)");
        Console.WriteLine(new string('=', 80) + "\n");
        Console.WriteLine($"Scenario: {GetScenarioName(scenario)}");
        Console.WriteLine("Analyzing claim:");
        Console.WriteLine($"  Customer: {mockClaim.CustomerId}");
        Console.WriteLine($"  Type: {mockClaim.NormalizedClaimType} - {mockClaim.NormalizedClaimSubType}");
        Console.WriteLine($"  Amount: ${mockClaim.PurchasePrice:N2}");
        Console.WriteLine($"  Date of Loss: {mockClaim.DateOfLoss}");
        Console.WriteLine();

        await using StreamingRun run = await InProcessExecution.StreamAsync(workflow, mockClaim);

        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            switch (evt)
            {
                case AgentRunUpdateEvent agentUpdate:
                    if (!string.IsNullOrEmpty(agentUpdate.Update.Text))
                    {
                        Console.Write(agentUpdate.Update.Text);
                    }
                    break;

                case WorkflowOutputEvent output:
                    Console.WriteLine("\n\n" + new string('=', 80));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ FRAUD ANALYSIS COMPLETE");
                    Console.ResetColor();
                    Console.WriteLine(new string('=', 80));
                    Console.WriteLine();
                    Console.WriteLine(output.Data);
                    Console.WriteLine();
                    Console.WriteLine(new string('=', 80));
                    break;
            }
        }

        Console.WriteLine("\n✅ Demo 12 Integrated Complete!\n");
        Console.WriteLine("Key Concepts Demonstrated:");
        Console.WriteLine("  ✓ Integration with ClaimsCore.Common models");
        Console.WriteLine("  ✓ Direct calls to ClaimsCoreMcp tools");
        Console.WriteLine("  ✓ Data quality review before fraud analysis");
        Console.WriteLine("  ✓ Classification routing by claim type");
        Console.WriteLine("  ✓ Parallel fraud detection (fan-out/fan-in pattern)");
        Console.WriteLine("  ✓ OSINT validation with real marketplace data");
        Console.WriteLine("  ✓ Customer history and fraud score analysis");
        Console.WriteLine("  ✓ Transaction-level fraud scoring");
        Console.WriteLine("  ✓ Polymorphic aggregator (handles 3 different finding types)");
        Console.WriteLine("  ✓ Data passing via messages (NO state in fan-out executors)");
        Console.WriteLine("  ✓ State operations in aggregator (stores collected findings)");
        Console.WriteLine("  ✓ AI-powered fraud decision with confidence scores");
        Console.WriteLine("  ✓ Real data from MockClaimsDataService\n");
    }

    /// <summary>
    /// Execute the fraud detection workflow with DevUI web interface.
    /// Provides a browser-based UI for visualizing and interacting with the workflow.
    /// Includes a failsafe to provide default input if none is supplied.
    /// </summary>
    public static async Task ExecuteWithDevUI(int scenario = 1, int port = 5000)
    {
        // Set console encoding to UTF-8 to support emojis and special characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Demo 12 Integrated: Fraud Detection with DevUI ===\n");
        Console.WriteLine($"📋 Default scenario: {GetScenarioName(scenario)}\n");
        Console.WriteLine("Setting up web server with DevUI for workflow visualization...\n");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development // Force Development mode for DevUI
        });

        // Azure OpenAI setup
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);
        var deploymentName = "gpt-4o";
        var chatClient = azureClient.GetChatClient(deploymentName).AsIChatClient();

        builder.Services.AddChatClient(chatClient);

        // Register the fraud detection workflow
        Console.WriteLine("Registering fraud detection workflow...");
        
        builder.AddWorkflow("fraud-detection-integrated", (sp, key) =>
        {
            // Build the workflow with the default scenario claim as entry point
            var workflow = BuildFraudDetectionWorkflowWithFailsafe(chatClient, scenario);
            
            return workflow;
        }).AddAsAIAgent();
        
        Console.WriteLine("  ✓ fraud-detection-integrated - Claims fraud detection pipeline");

        // Configure DevUI services
        builder.Services.AddOpenAIResponses();
        builder.Services.AddOpenAIConversations();

        var app = builder.Build();

        // Override default port
        app.Urls.Clear();
        app.Urls.Add($"http://localhost:{port}");

        // Map DevUI endpoints
        app.MapOpenAIResponses();
        app.MapOpenAIConversations();
        app.MapDevUI();

        // Display usage information
        var url = $"http://localhost:{port}";
        
        Console.WriteLine("\n" + new string('=', 80));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ DevUI Server Started Successfully!");
        Console.ResetColor();
        Console.WriteLine(new string('=', 80));
        
        Console.WriteLine("\n📊 Available Endpoints:");
        Console.WriteLine($"  • DevUI Interface:           {url}/devui");
        Console.WriteLine($"  • OpenAI Responses API:      {url}/v1/responses");
        Console.WriteLine($"  • OpenAI Conversations API:  {url}/v1/conversations");
        
        Console.WriteLine("\n🔍 Registered Workflow:");
        Console.WriteLine("  • fraud-detection-integrated - Claims fraud detection pipeline");
        Console.WriteLine($"    Default scenario: {GetScenarioName(scenario)}");
        
        Console.WriteLine("\n💡 How to Use:");
        Console.WriteLine($"  1. Open your browser to: {url}/devui");
        Console.WriteLine("  2. Select 'fraud-detection-integrated' from the agent dropdown");
        Console.WriteLine("  3. Click 'Start' to run with default scenario, or provide JSON input:");
        Console.WriteLine("     • Empty input = uses default scenario");
        Console.WriteLine("     • Number (1-3) = selects specific scenario");
        Console.WriteLine("     • JSON = ValidationResult object");
        Console.WriteLine("  4. View the fraud analysis pipeline in real-time");
        Console.WriteLine("  5. Inspect traces, agent outputs, and fraud scores");
        
        Console.WriteLine("\n📝 Example Inputs:");
        Console.WriteLine("  • Leave empty (uses scenario 1 - Hello Kitty bike)");
        Console.WriteLine("  • Type: 2 (uses scenario 2 - Mobile phone)");
        Console.WriteLine("  • Type: 3 (uses scenario 3 - Car theft)");
        
        Console.WriteLine("\n⚠️  Press Ctrl+C to stop the server");
        Console.WriteLine(new string('=', 80) + "\n");

        await app.RunAsync();
    }

    // --------------------- Scenario data ---------------------
    private static string GetScenarioName(int scenario) => scenario switch
    {
        1 => "HIGH RISK - Rare Hello Kitty collector's bike theft (CHF 15K)",
        2 => "LOW RISK - Mobile phone theft from gym locker (CHF 850)",
        3 => "MODERATE RISK - Car theft from residential street (CHF 38K)",
        _ => "HIGH RISK (default)"
    };

    private static ValidationResult GetMockClaim(int scenario) => scenario switch
    {
        1 => GetHighRiskClaim(),
        2 => GetLowRiskClaim(),
        3 => GetModerateRiskClaim(),
        _ => GetHighRiskClaim() // Default to high risk
    };

    private static ValidationResult GetHighRiskClaim()
    {
        return new ValidationResult
        {
            Ready = true,
            CustomerId = "CUST-10001",
            ContractId = "CONTRACT-P-5001",
            NormalizedClaimType = "Property",
            NormalizedClaimSubType = "BikeTheft",
            DateOfLoss = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"), // Day before yesterday
            DateReported = DateTime.Now.ToString("yyyy-MM-dd"),
            ShortDescription = "Rare Hello Kitty collector's mountain bike stolen from restaurant",
            ItemDescription = "Hello Kitty Mountain Bike Collector's Edition, pink and white with Hello Kitty motifs, chromated parts, double suspension",
            DetailedDescription = "The day before yesterday, I went to a restaurant in downtown Zürich. When I came out after dinner, my bike was gone. It was locked with a heavy-duty U-lock to a bike rack right in front of the restaurant. The bike is a very rare Hello Kitty Mountain Bike Collector's Edition - very pink and white with Hello Kitty motifs all over the frame, chromated parts on the handlebars and wheels, and full double suspension. I purchased it from a specialized collector in Japan for CHF 15,000. It's extremely distinctive and easily recognizable.",
            PurchasePrice = 15000.00m
        };
    }

    private static ValidationResult GetLowRiskClaim()
    {
        return new ValidationResult
        {
            Ready = true,
            CustomerId = "CUST-67890",
            ContractId = "POL-54321",
            NormalizedClaimType = "Property",
            NormalizedClaimSubType = "MobileTheft",
            DateOfLoss = DateTime.Now.AddDays(-45).ToString("yyyy-MM-dd"), // Not recent (45 days ago)
            DateReported = DateTime.Now.ToString("yyyy-MM-dd"),
            ShortDescription = "Mobile phone stolen from gym locker",
            ItemDescription = "Samsung Galaxy S23, black, 256GB",
            DetailedDescription = "About six weeks ago, I was at my regular gym. I left my mobile phone in my locker with a combination lock. When I returned after my workout, the locker was still locked but my phone was missing. I reported it to the gym staff immediately and filed a police report the same day. The phone was a Samsung Galaxy S23, black color, 256GB storage, which I purchased about 8 months ago for CHF 850.",
            PurchasePrice = 850.00m
        };
    }

    private static ValidationResult GetModerateRiskClaim()
    {
        return new ValidationResult
        {
            Ready = true,
            CustomerId = "CUST-10003",
            ContractId = "CONTRACT-P-5003",
            NormalizedClaimType = "Property",
            NormalizedClaimSubType = "CarTheft",
            DateOfLoss = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), // Very recent (yesterday)
            DateReported = DateTime.Now.ToString("yyyy-MM-dd"),
            ShortDescription = "Car stolen from residential street",
            ItemDescription = "2022 Volkswagen Golf GTI, dark grey metallic, license plate ZH-123456",
            DetailedDescription = "Yesterday evening, I parked my car on the street near my apartment in Zürich. This morning when I went to drive to work, the car was gone. I had locked it and engaged the alarm system. The car is a 2022 Volkswagen Golf GTI in dark grey metallic color, license plate ZH-123456. I purchased it new about 18 months ago for CHF 38,000. I immediately filed a police report and checked with the city about any towing - the car was not towed. It appears to have been stolen.",
            PurchasePrice = 38000.00m
        };
    }

    private static Workflow BuildFraudDetectionWorkflow(IChatClient chatClient)
    {
        // Register tools that call ClaimsTools directly
        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(
                (string itemDescription, decimal itemValue) => 
                    ClaimsTools.CheckOnlineMarketplaces(itemDescription, itemValue),
                name: "check_online_marketplaces",
                description: "Check if stolen property is listed for sale on online marketplaces. Used for OSINT fraud detection."
            ),
            AIFunctionFactory.Create(
                (string customerId) => ClaimsTools.GetClaimHistory(customerId),
                name: "get_customer_claim_history",
                description: "Return all past claims for the customer with basic metadata and statuses. Used for risk and behavior analysis."
            ),
            AIFunctionFactory.Create(
                (decimal claimAmount, string dateOfLoss) => 
                    ClaimsTools.GetTransactionRiskProfile(claimAmount, dateOfLoss),
                name: "get_transaction_risk_profile",
                description: "Analyze transaction risk profile for fraud indicators based on claim amount and timing patterns."
            )
        };

        // Agents
        var dataReviewAgent = GetDataReviewAgent(chatClient);
        var osintAgent = GetOSINTAgent(chatClient, tools);
        var userHistoryAgent = GetUserHistoryAgent(chatClient, tools);
        var transactionAgent = GetTransactionFraudAgent(chatClient, tools);
        var fraudDecisionAgent = GetFraudDecisionAgent(chatClient);
        var outcomePresenterAgent = GetOutcomePresenterAgent(chatClient);

        // Executors
        var dataReviewExec = new DataReviewExecutor(dataReviewAgent);
        var classificationExec = new ClassificationRouterExecutor();
        var propertyTheftFanOutExec = new PropertyTheftFanOutExecutor();
        var osintExec = new OSINTExecutor(osintAgent);
        var userHistoryExec = new UserHistoryExecutor(userHistoryAgent);
        var transactionExec = new TransactionFraudExecutor(transactionAgent);
        var fraudAggregatorExec = new FraudAggregatorExecutor();
        var fraudDecisionExec = new FraudDecisionExecutor(fraudDecisionAgent);
        var outcomeExec = new OutcomePresenterExecutor(outcomePresenterAgent);

        // Build workflow
        return new WorkflowBuilder(dataReviewExec)
            .AddEdge<DataReviewResult>(dataReviewExec, classificationExec, 
                condition: dr => dr is not null && dr.Proceed)
            .AddEdge(classificationExec, propertyTheftFanOutExec)
            .AddFanOutEdge(propertyTheftFanOutExec, targets: [osintExec, userHistoryExec, transactionExec])
            .AddFanInEdge(fraudAggregatorExec, sources: [osintExec, userHistoryExec, transactionExec])
            .AddEdge(fraudAggregatorExec, fraudDecisionExec)
            .AddEdge(fraudDecisionExec, outcomeExec)
            .WithOutputFrom(outcomeExec)
            .Build();
    }

    /// <summary>
    /// Builds the fraud detection workflow with a failsafe input executor for DevUI.
    /// If no input is provided via DevUI, the failsafe provides a default ValidationResult.
    /// </summary>
    private static Workflow BuildFraudDetectionWorkflowWithFailsafe(IChatClient chatClient, int scenario)
    {
        // Register tools that call ClaimsTools directly
        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(
                (string itemDescription, decimal itemValue) => 
                    ClaimsTools.CheckOnlineMarketplaces(itemDescription, itemValue),
                name: "check_online_marketplaces",
                description: "Check if stolen property is listed for sale on online marketplaces. Used for OSINT fraud detection."
            ),
            AIFunctionFactory.Create(
                (string customerId) => ClaimsTools.GetClaimHistory(customerId),
                name: "get_customer_claim_history",
                description: "Return all past claims for the customer with basic metadata and statuses. Used for risk and behavior analysis."
            ),
            AIFunctionFactory.Create(
                (decimal claimAmount, string dateOfLoss) => 
                    ClaimsTools.GetTransactionRiskProfile(claimAmount, dateOfLoss),
                name: "get_transaction_risk_profile",
                description: "Analyze transaction risk profile for fraud indicators based on claim amount and timing patterns."
            )
        };

        // Agents
        var dataReviewAgent = GetDataReviewAgent(chatClient);
        var osintAgent = GetOSINTAgent(chatClient, tools);
        var userHistoryAgent = GetUserHistoryAgent(chatClient, tools);
        var transactionAgent = GetTransactionFraudAgent(chatClient, tools);
        var fraudDecisionAgent = GetFraudDecisionAgent(chatClient);
        var outcomePresenterAgent = GetOutcomePresenterAgent(chatClient);

        // Executors
        var failsafeInputExec = new FailsafeInputExecutor(scenario); // Provides default input if none supplied
        var dataReviewExec = new DataReviewExecutor(dataReviewAgent);
        var classificationExec = new ClassificationRouterExecutor();
        var propertyTheftFanOutExec = new PropertyTheftFanOutExecutor();
        var osintExec = new OSINTExecutor(osintAgent);
        var userHistoryExec = new UserHistoryExecutor(userHistoryAgent);
        var transactionExec = new TransactionFraudExecutor(transactionAgent);
        var fraudAggregatorExec = new FraudAggregatorExecutor();
        var fraudDecisionExec = new FraudDecisionExecutor(fraudDecisionAgent);
        var outcomeExec = new OutcomePresenterExecutor(outcomePresenterAgent);

        // Build workflow with failsafe input executor
        return new WorkflowBuilder(failsafeInputExec)  // Start with failsafe
            .AddEdge(failsafeInputExec, dataReviewExec)
            .AddEdge<DataReviewResult>(dataReviewExec, classificationExec, 
                condition: dr => dr is not null && dr.Proceed)
            .AddEdge(classificationExec, propertyTheftFanOutExec)
            .AddFanOutEdge(propertyTheftFanOutExec, targets: [osintExec, userHistoryExec, transactionExec])
            .AddFanInEdge(fraudAggregatorExec, sources: [osintExec, userHistoryExec, transactionExec])
            .AddEdge(fraudAggregatorExec, fraudDecisionExec)
            .AddEdge(fraudDecisionExec, outcomeExec)
            .WithOutputFrom(outcomeExec)
            .WithName("fraud-detection-integrated")
            .Build();
    }

    // --------------------- Agent factories ---------------------
    
    private static AIAgent GetDataReviewAgent(IChatClient chat) =>
        new ChatClientAgent(chat, new ChatClientAgentOptions
        {
            Name = "DataReviewAgent",
            Instructions = """
                You are a data quality specialist for insurance claims.
                
                Review the claim data for:
                1. Completeness - all required fields present
                2. Consistency - values make sense together
                3. Data quality - descriptions are detailed enough
                4. Red flags - obviously suspicious patterns
                
                Provide a quality score (0-100) and list any concerns.
                Decide whether to proceed with fraud analysis.
                """,
            ChatOptions = new()
            {
                ResponseFormat = ChatResponseFormat.ForJsonSchema<DataReviewResult>()
            }
        });

    private static AIAgent GetOSINTAgent(IChatClient chat, List<AITool> tools) =>
        new ChatClientAgent(chat, new ChatClientAgentOptions
        {
            Name = "OSINTValidatorAgent",
            Instructions = """
                You are an OSINT (Open Source Intelligence) specialist.
                
                Check if the reported stolen property is listed for sale online.
                Use the check_online_marketplaces tool to search Swiss marketplaces
                (ricardo.ch, anibis.ch, etc.) and international sites (eBay).
                
                Provide:
                - Whether item was found
                - Which marketplaces were checked
                - Details of matching listings
                - Fraud indicator score (0-100)
                - Summary of findings
                """,
            ChatOptions = new()
            {
                Tools = tools,
                ResponseFormat = ChatResponseFormat.ForJsonSchema<OSINTFinding>()
            }
        });

    private static AIAgent GetUserHistoryAgent(IChatClient chat, List<AITool> tools) =>
        new ChatClientAgent(chat, new ChatClientAgentOptions
        {
            Name = "UserValidationAgent",
            Instructions = """
                You are a customer fraud analyst.
                
                Analyze the customer's claim history for suspicious patterns.
                Use get_customer_claim_history tool to retrieve past claims.
                
                Look for:
                - Frequency of claims
                - Pattern of claim types
                - Previous fraud indicators
                - Historical fraud score
                
                Provide customer fraud score (0-100) and summary.
                """,
            ChatOptions = new()
            {
                Tools = tools,
                ResponseFormat = ChatResponseFormat.ForJsonSchema<UserHistoryFinding>()
            }
        });

    private static AIAgent GetTransactionFraudAgent(IChatClient chat, List<AITool> tools) =>
        new ChatClientAgent(chat, new ChatClientAgentOptions
        {
            Name = "TransactionFraudScoringAgent",
            Instructions = """
                You are a transaction fraud specialist.
                
                Analyze this specific claim transaction for fraud indicators:
                - Claim amount vs typical claims
                - Timing patterns (e.g., filed right before policy expires)
                - Description quality and plausibility
                - Geographic/behavioral anomalies
                
                Use get_transaction_risk_profile tool for context.
                
                Provide:
                - Anomaly score (0-100)
                - List of red flags detected
                - Overall transaction fraud score
                - Summary
                """,
            ChatOptions = new()
            {
                Tools = tools,
                ResponseFormat = ChatResponseFormat.ForJsonSchema<TransactionFraudFinding>()
            }
        });

    private static AIAgent GetFraudDecisionAgent(IChatClient chat) =>
        new ChatClientAgent(chat, new ChatClientAgentOptions
        {
            Name = "FraudDecisionAgent",
            Instructions = """
                You are a fraud decision specialist.
                
                Analyze ALL the findings from OSINT, User History, and Transaction analysis.
                
                Make a final determination:
                - Is this likely fraud? (true/false)
                - Confidence score (0-100)
                - Combined fraud score (0-100)
                - Recommendation: APPROVE / INVESTIGATE / REJECT
                - Clear reasoning
                - Key factors that influenced your decision
                
                Be thorough but fair. Consider all evidence.
                """,
            ChatOptions = new()
            {
                ResponseFormat = ChatResponseFormat.ForJsonSchema<FraudDecision>()
            }
        });

    private static AIAgent GetOutcomePresenterAgent(IChatClient chat) =>
        new ChatClientAgent(chat, """
            You are a professional fraud case reporter.
            
            Generate a clear, professional email to the insurance case handler
            summarizing the fraud analysis and recommendation.
            
            Include:
            - Executive summary
            - Fraud determination and confidence
            - Key findings from each analysis
            - Recommended action
            - Next steps
            
            Use professional, formal language.
            Be factual and avoid speculation beyond the analysis.
            """);

    // --------------------- Executors ---------------------

    /// <summary>
    /// FailsafeInputExecutor - Provides default input if none is supplied (for DevUI).
    /// Handles ChatMessage input from DevUI and converts it to ValidationResult.
    /// Supports:
    /// - Empty message content -> uses default scenario
    /// - Number (1-3) -> selects specific scenario
    /// - JSON -> parses as ValidationResult object
    /// </summary>
    private sealed class FailsafeInputExecutor :
        ReflectingExecutor<FailsafeInputExecutor>,
        IMessageHandler<ChatMessage, ValidationResult>
    {
        private readonly int _defaultScenario;

        public FailsafeInputExecutor(int defaultScenario = 1) : base("FailsafeInput")
        {
            _defaultScenario = defaultScenario;
        }

        // Handle workflow entry from DevUI - expects ChatMessage input, returns ValidationResult
        public async ValueTask<ValidationResult> HandleAsync(
            ChatMessage message,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"📌 FailsafeInput: Processing workflow entry from DevUI");
            
            // Extract text content from ChatMessage
            var input = message.Text ?? string.Empty;
            Console.WriteLine($"   Message content: '{(string.IsNullOrWhiteSpace(input) ? "(empty)" : input)}'");
            
            ValidationResult claimToProcess;
            
            // If input is null, empty, or whitespace -> use default scenario
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"⚠️  No input provided - using default scenario {_defaultScenario}");
                claimToProcess = GetMockClaim(_defaultScenario);
            }
            // Try to parse as scenario number (1-3)
            else if (int.TryParse(input.Trim(), out int scenarioNumber) && scenarioNumber >= 1 && scenarioNumber <= 3)
            {
                Console.WriteLine($"✅ FailsafeInput: Scenario {scenarioNumber} selected");
                claimToProcess = GetMockClaim(scenarioNumber);
            }
            // Try to parse as JSON ValidationResult
            else if (input.TrimStart().StartsWith("{"))
            {
                try
                {
                    Console.WriteLine($"✅ FailsafeInput: Parsing JSON ValidationResult");
                    var parsedClaim = JsonSerializer.Deserialize<ValidationResult>(input, JsonSerializerOptions.Web);
                    
                    if (parsedClaim != null && parsedClaim.Ready)
                    {
                        claimToProcess = parsedClaim;
                        Console.WriteLine($"   Successfully parsed ValidationResult for customer {parsedClaim.CustomerId}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️  Parsed ValidationResult is not ready - using default scenario {_defaultScenario}");
                        claimToProcess = GetMockClaim(_defaultScenario);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"⚠️  Failed to parse JSON: {ex.Message}");
                    Console.WriteLine($"   Using default scenario {_defaultScenario}");
                    claimToProcess = GetMockClaim(_defaultScenario);
                }
            }
            // Unrecognized input -> use default scenario
            else
            {
                Console.WriteLine($"⚠️  Unrecognized input format - using default scenario {_defaultScenario}");
                claimToProcess = GetMockClaim(_defaultScenario);
            }
            
            // Return the ValidationResult to be passed to the next executor
            return claimToProcess;
        }
    }

    private sealed class DataReviewExecutor :
        ReflectingExecutor<DataReviewExecutor>,
        IMessageHandler<ValidationResult, DataReviewResult>
    {
        private readonly AIAgent _agent;
        public DataReviewExecutor(AIAgent agent) : base("DataReview") => _agent = agent;

        public async ValueTask<DataReviewResult> HandleAsync(
            ValidationResult claim,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            var state = await ReadFraudStateAsync(context);
            state.OriginalClaim = claim;
            
            Console.WriteLine("\n=== Data Quality Review ===\n");

            var prompt = $"""
                Review this claim data for quality and completeness:
                
                Customer ID: {claim.CustomerId}
                Contract ID: {claim.ContractId}
                Type: {claim.NormalizedClaimType} - {claim.NormalizedClaimSubType}
                Date of Loss: {claim.DateOfLoss}
                Date Reported: {claim.DateReported}
                Item: {claim.ItemDescription}
                Value: ${claim.PurchasePrice}
                
                Short Description: {claim.ShortDescription}
                Detailed Description: {claim.DetailedDescription}
                """;

            var response = await _agent.RunAsync(prompt, cancellationToken: cancellationToken);
            var result = response.Deserialize<DataReviewResult>(JsonSerializerOptions.Web);

            state.DataReview = result;
            await SaveFraudStateAsync(context, state);

            Console.WriteLine($"Quality Score: {result.QualityScore}/100");
            Console.WriteLine($"Proceed: {result.Proceed}\n");

            return result;
        }
    }

    private sealed class ClassificationRouterExecutor :
        ReflectingExecutor<ClassificationRouterExecutor>,
        IMessageHandler<DataReviewResult, string>
    {
        public ClassificationRouterExecutor() : base("ClassificationRouter") { }

        public async ValueTask<string> HandleAsync(
            DataReviewResult review,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            var state = await ReadFraudStateAsync(context);
            var claim = state.OriginalClaim!;
            
            Console.WriteLine($"\n=== Classification Router ===");
            Console.WriteLine($"Claim Type: {claim.NormalizedClaimType}");
            Console.WriteLine($"Sub-Type: {claim.NormalizedClaimSubType}\n");

            // For now, route everything to Property Theft
            // Future: Add other claim type handlers
            state.ClaimType = claim.NormalizedClaimType ?? "";
            state.ClaimSubType = claim.NormalizedClaimSubType ?? "";
            await SaveFraudStateAsync(context, state);

            return "PropertyTheft";
        }
    }

    private sealed class PropertyTheftFanOutExecutor :
        ReflectingExecutor<PropertyTheftFanOutExecutor>,
        IMessageHandler<string>
    {
        public PropertyTheftFanOutExecutor() : base("PropertyTheftFanOut") { }

        public async ValueTask HandleAsync(
            string routeType,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine("\n=== Parallel Fraud Detection (Fan-Out) ===");
            Console.WriteLine("Dispatching to 3 fraud detection agents...\n");
            
            // Read state HERE (fan-out executor can use state)
            var state = await ReadFraudStateAsync(context);
            var claim = state.OriginalClaim!;
            
            // Send the CLAIM to all executors
            await context.SendMessageAsync(claim, cancellationToken: cancellationToken);
        }
    }

    private sealed class OSINTExecutor :
        ReflectingExecutor<OSINTExecutor>,
        IMessageHandler<ValidationResult, OSINTFinding>
    {
        private readonly AIAgent _agent;
        public OSINTExecutor(AIAgent agent) : base("OSINT") => _agent = agent;

        public async ValueTask<OSINTFinding> HandleAsync(
            ValidationResult claim,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine("=== OSINT Validation (Online Marketplaces) ===\n");

            var prompt = $"""
                Check if this stolen item is listed for sale online:
                
                Item Description: {claim.ItemDescription}
                Detailed Context: {claim.DetailedDescription}
                Value: ${claim.PurchasePrice}
                Date of Loss: {claim.DateOfLoss}
                
                Use check_online_marketplaces tool to search.
                """;

            var response = await _agent.RunAsync(prompt, cancellationToken: cancellationToken);
            var finding = response.Deserialize<OSINTFinding>(JsonSerializerOptions.Web);

            Console.ForegroundColor = finding.FraudIndicatorScore > 70 ? ConsoleColor.Red : 
                                      finding.FraudIndicatorScore > 40 ? ConsoleColor.Yellow : ConsoleColor.Green;
            Console.WriteLine($"✓ OSINT Check Complete - Fraud Score: {finding.FraudIndicatorScore}/100");
            Console.ResetColor();
            Console.WriteLine($"  Item Found Online: {(finding.ItemFoundOnline ? "YES" : "NO")}");
            Console.WriteLine($"  Marketplaces Checked: {finding.MarketplacesChecked.Count}");
            if (finding.MatchingListings.Count > 0)
            {
                Console.WriteLine($"  Matching Listings: {finding.MatchingListings.Count}");
                foreach (var listing in finding.MatchingListings.Take(2))
                {
                    Console.WriteLine($"    • {listing}");
                }
            }
            Console.WriteLine($"  Summary: {finding.Summary}");
            Console.WriteLine();
            
            return finding;
        }
    }

    private sealed class UserHistoryExecutor :
        ReflectingExecutor<UserHistoryExecutor>,
        IMessageHandler<ValidationResult, UserHistoryFinding>
    {
        private readonly AIAgent _agent;
        public UserHistoryExecutor(AIAgent agent) : base("UserHistory") => _agent = agent;

        public async ValueTask<UserHistoryFinding> HandleAsync(
            ValidationResult claim,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine("=== User History Analysis ===\n");

            var prompt = $"""
                Analyze this customer's claim history for fraud patterns:
                
                Customer ID: {claim.CustomerId}
                Current Claim Type: {claim.NormalizedClaimType} - {claim.NormalizedClaimSubType}
                
                Use get_customer_claim_history tool to retrieve past claims.
                """;

            var response = await _agent.RunAsync(prompt, cancellationToken: cancellationToken);
            var finding = response.Deserialize<UserHistoryFinding>(JsonSerializerOptions.Web);

            Console.ForegroundColor = finding.CustomerFraudScore > 60 ? ConsoleColor.Red : 
                                      finding.CustomerFraudScore > 30 ? ConsoleColor.Yellow : ConsoleColor.Green;
            Console.WriteLine($"✓ User History Check Complete - Customer Fraud Score: {finding.CustomerFraudScore}/100");
            Console.ResetColor();
            Console.WriteLine($"  Previous Claims: {finding.PreviousClaimsCount}");
            Console.WriteLine($"  Suspicious Activity: {(finding.SuspiciousActivityDetected ? "DETECTED" : "None")}");
            if (finding.ClaimHistory.Count > 0)
            {
                Console.WriteLine($"  Recent Claims History:");
                foreach (var claimItem in finding.ClaimHistory.Take(3))
                {
                    Console.WriteLine($"    • {claimItem}");
                }
            }
            Console.WriteLine($"  Summary: {finding.Summary}");
            Console.WriteLine();
            
            return finding;
        }
    }

    private sealed class TransactionFraudExecutor :
        ReflectingExecutor<TransactionFraudExecutor>,
        IMessageHandler<ValidationResult, TransactionFraudFinding>
    {
        private readonly AIAgent _agent;
        public TransactionFraudExecutor(AIAgent agent) : base("TransactionFraud") => _agent = agent;

        public async ValueTask<TransactionFraudFinding> HandleAsync(
            ValidationResult claim,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine("=== Transaction Fraud Scoring ===\n");

            var prompt = $"""
                Analyze this transaction for fraud indicators:
                
                Claim Amount: ${claim.PurchasePrice}
                Date of Loss: {claim.DateOfLoss}
                Description: {claim.DetailedDescription}
                
                Use get_transaction_risk_profile tool for context.
                """;

            var response = await _agent.RunAsync(prompt, cancellationToken: cancellationToken);
            var finding = response.Deserialize<TransactionFraudFinding>(JsonSerializerOptions.Web);

            Console.ForegroundColor = finding.TransactionFraudScore > 60 ? ConsoleColor.Red : 
                                      finding.TransactionFraudScore > 30 ? ConsoleColor.Yellow : ConsoleColor.Green;
            Console.WriteLine($"✓ Transaction Analysis Complete - Fraud Score: {finding.TransactionFraudScore}/100");
            Console.ResetColor();
            Console.WriteLine($"  Anomaly Score: {finding.AnomalyScore}/100");
            if (finding.RedFlags.Count > 0)
            {
                Console.WriteLine($"  Red Flags Detected: {finding.RedFlags.Count}");
                foreach (var flag in finding.RedFlags)
                {
                    Console.WriteLine($"    ⚠️  {flag}");
                }
            }
            else
            {
                Console.WriteLine($"  Red Flags: None detected");
            }
            Console.WriteLine($"  Summary: {finding.Summary}");
            Console.WriteLine();
            
            return finding;
        }
    }

    private sealed class FraudAggregatorExecutor :
        ReflectingExecutor<FraudAggregatorExecutor>,
        IMessageHandler<OSINTFinding, string>,
        IMessageHandler<UserHistoryFinding, string>,
        IMessageHandler<TransactionFraudFinding, string>
    {
        private OSINTFinding? _osintFinding;
        private UserHistoryFinding? _userHistoryFinding;
        private TransactionFraudFinding? _transactionFinding;
        private int _receivedCount = 0;
        private const int ExpectedCount = 3;

        public FraudAggregatorExecutor() : base("FraudAggregator") { }

        public ValueTask<string> HandleAsync(
            OSINTFinding finding, 
            IWorkflowContext context, 
            CancellationToken cancellationToken = default)
        {
            return HandleFindingAsync("OSINT", finding, context, cancellationToken, 
                () => { _osintFinding = finding; });
        }

        public ValueTask<string> HandleAsync(
            UserHistoryFinding finding, 
            IWorkflowContext context, 
            CancellationToken cancellationToken = default)
        {
            return HandleFindingAsync("UserHistory", finding, context, cancellationToken, 
                () => { _userHistoryFinding = finding; });
        }

        public ValueTask<string> HandleAsync(
            TransactionFraudFinding finding, 
            IWorkflowContext context, 
            CancellationToken cancellationToken = default)
        {
            return HandleFindingAsync("Transaction", finding, context, cancellationToken, 
                () => { _transactionFinding = finding; });
        }

        private async ValueTask<string> HandleFindingAsync<T>(
            string sourceName,
            T finding,
            IWorkflowContext context,
            CancellationToken cancellationToken,
            Action storeFinding)
        {
            Console.WriteLine($"[Aggregator] ✓ Received finding from {sourceName} ({_receivedCount + 1}/{ExpectedCount})");
            
            storeFinding();
            _receivedCount++;

            if (_receivedCount >= ExpectedCount)
            {
                Console.WriteLine("\n=== All Fraud Findings Collected (Fan-In) ===\n");
                
                var state = await ReadFraudStateAsync(context);
                state.OSINTFinding = _osintFinding;
                state.UserHistoryFinding = _userHistoryFinding;
                state.TransactionFraudFinding = _transactionFinding;
                await SaveFraudStateAsync(context, state);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ All findings stored in shared state!");
                Console.ResetColor();
                Console.WriteLine();
                
                // Reset for potential re-runs
                _osintFinding = null;
                _userHistoryFinding = null;
                _transactionFinding = null;
                _receivedCount = 0;
                
                return "[Fraud Analysis Complete] All findings collected and stored";
            }

            return null!;
        }
    }

    private sealed class FraudDecisionExecutor :
        ReflectingExecutor<FraudDecisionExecutor>,
        IMessageHandler<string, FraudDecision>
    {
        private readonly AIAgent _agent;
        public FraudDecisionExecutor(AIAgent agent) : base("FraudDecision") => _agent = agent;

        public async ValueTask<FraudDecision> HandleAsync(
            string _,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            var state = await ReadFraudStateAsync(context);
            
            Console.WriteLine("=== Final Fraud Decision ===\n");

            var prompt = $"""
                Make final fraud determination based on ALL findings:
                
                OSINT Finding:
                {JsonSerializer.Serialize(state.OSINTFinding, new JsonSerializerOptions { WriteIndented = true })}
                
                User History Finding:
                {JsonSerializer.Serialize(state.UserHistoryFinding, new JsonSerializerOptions { WriteIndented = true })}
                
                Transaction Fraud Finding:
                {JsonSerializer.Serialize(state.TransactionFraudFinding, new JsonSerializerOptions { WriteIndented = true })}
                
                Provide your final decision with reasoning.
                """;

            var response = await _agent.RunAsync(prompt, cancellationToken: cancellationToken);
            var decision = response.Deserialize<FraudDecision>(JsonSerializerOptions.Web);

            state.FraudDecision = decision;
            await SaveFraudStateAsync(context, state);

            // Print readable decision
            Console.WriteLine(new string('─', 80));
            Console.ForegroundColor = decision.IsFraud ? ConsoleColor.Red : ConsoleColor.Green;
            var icon = decision.IsFraud ? "🚨" : "✅";
            Console.WriteLine($"{icon} FRAUD DETERMINATION: {(decision.IsFraud ? "LIKELY FRAUD" : "NO FRAUD DETECTED")}");
            Console.ResetColor();
            Console.WriteLine($"📊 Confidence: {decision.ConfidenceScore}%");
            Console.WriteLine($"📝 Recommendation: {decision.Recommendation}");
            Console.WriteLine(new string('─', 80));
            Console.WriteLine();

            return decision;
        }
    }

    private sealed class OutcomePresenterExecutor :
        ReflectingExecutor<OutcomePresenterExecutor>,
        IMessageHandler<FraudDecision, string>
    {
        private readonly AIAgent _agent;
        public OutcomePresenterExecutor(AIAgent agent) : base("OutcomePresenter") => _agent = agent;

        public async ValueTask<string> HandleAsync(
            FraudDecision decision,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
        {
            var state = await ReadFraudStateAsync(context);
            
            Console.WriteLine("=== Generating Case Handler Email ===\n");

            var prompt = $"""
                Generate a professional email to the insurance case handler summarizing this fraud analysis:
                
                Claim Details:
                - Customer: {state.OriginalClaim?.CustomerId}
                - Type: {state.ClaimType} - {state.ClaimSubType}
                - Amount: ${state.OriginalClaim?.PurchasePrice}
                
                Fraud Analysis:
                {JsonSerializer.Serialize(state.FraudDecision, new JsonSerializerOptions { WriteIndented = true })}
                
                Include all key findings and provide clear next steps.
                """;

            var sb = new StringBuilder();
            await foreach (var up in _agent.RunStreamingAsync(new ChatMessage(ChatRole.User, prompt), cancellationToken: cancellationToken))
            {
                if (!string.IsNullOrEmpty(up.Text))
                {
                    sb.Append(up.Text);
                }
            }

            return sb.ToString();
        }
    }
}
