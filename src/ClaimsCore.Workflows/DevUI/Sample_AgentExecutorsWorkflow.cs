// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.AI.OpenAI;
using MAFPlayground.Utils;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClaimsCore.Workflows.DevUI;

/// <summary>
/// Sample: Agent Executors Workflow with DevUI
/// 
/// This sample demonstrates how to create custom executors for AI agents and expose them through DevUI.
/// This is useful when you want more control over the agent's behaviors in a workflow.
///
/// In this example, we create two custom executors:
/// 1. SloganWriterExecutor: An AI agent that generates slogans based on a given task.
/// 2. FeedbackExecutor: An AI agent that provides feedback on the generated slogans.
/// (These two executors manage the agent instances and their conversation threads.)
///
/// The workflow alternates between these two executors until the slogan meets a certain
/// quality threshold or a maximum number of attempts is reached.
/// 
/// DevUI Features Demonstrated:
/// - Custom executor registration
/// - Iterative refinement workflows
/// - Real-time progress tracking
/// - Custom event monitoring
/// - Quality-based decision loops
/// 
/// DevUI Endpoints:
/// - DevUI Interface: http://localhost:5000/devui
/// - OpenAI Responses API: http://localhost:5000/v1/responses
/// - OpenAI Conversations API: http://localhost:5000/v1/conversations
/// </summary>
/// <remarks>
/// This sample shows advanced workflow patterns:
/// - Custom executors with state management
/// - Multiple message handlers per executor
/// - Conditional routing based on quality metrics
/// - Iteration limits for safety
/// - Custom workflow events for monitoring
/// </remarks>
internal static class Sample_AgentExecutorsWorkflow
{
    public static async Task Execute()
    {
        // Set console encoding to UTF-8 to support emojis and special characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Sample: Agent Executors Workflow with DevUI ===\n");
        Console.WriteLine("Setting up web server with custom executor workflow for DevUI...\n");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development // Force Development mode for DevUI
        });

        // Set up the Azure OpenAI client using AIConfig
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);

        var chatClient = azureClient
            .GetChatClient("gpt-4o")
            .AsIChatClient();

        builder.Services.AddChatClient(chatClient);

        // ====================================
        // Register Slogan Writer Workflow
        // ====================================
        Console.WriteLine("Registering workflows:");
        
        builder.AddWorkflow("slogan-writer-workflow", (sp, key) =>
        {
            var client = sp.GetRequiredService<IChatClient>();
            
            // Create the custom executors
            var sloganWriter = new SloganWriterExecutor("SloganWriter", client);
            var feedbackProvider = new FeedbackExecutor("FeedbackProvider", client)
            {
                MinimumRating = 8,
                MaxAttempts = 3
            };

            // Build the workflow by adding executors and connecting them
            var workflow = new WorkflowBuilder(sloganWriter)
                .AddEdge(sloganWriter, feedbackProvider)
                .AddEdge(feedbackProvider, sloganWriter)
                .WithOutputFrom(feedbackProvider)
                .WithName(key)
                .Build();

            return workflow;
        }).AddAsAIAgent();
        
        Console.WriteLine("  ? slogan-writer-workflow - Iterative slogan generation with feedback");

        // ====================================
        // Configure DevUI Services
        // ====================================
        builder.Services.AddOpenAIResponses();
        builder.Services.AddOpenAIConversations();

        var app = builder.Build();

        // ====================================
        // Map DevUI Endpoints
        // ====================================
        app.MapOpenAIResponses();
        app.MapOpenAIConversations();

        // Map DevUI (always enabled since we forced Development mode)
        app.MapDevUI();

        // ====================================
        // Display Usage Information
        // ====================================
        var url = "http://localhost:5000"; // Default URL
        
        Console.WriteLine("\n" + new string('=', 80));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("? DevUI Server Started Successfully!");
        Console.ResetColor();
        Console.WriteLine(new string('=', 80));
        
        Console.WriteLine("\n?? Available Endpoints:");
        Console.WriteLine($"  • DevUI Interface:           {url}/devui");
        Console.WriteLine($"  • OpenAI Responses API:      {url}/v1/responses");
        Console.WriteLine($"  • OpenAI Conversations API:  {url}/v1/conversations");
        
        Console.WriteLine("\n?? Registered Workflow:");
        Console.WriteLine("  • slogan-writer-workflow - Iterative slogan generation with AI feedback");
        Console.WriteLine("    - SloganWriter: Generates creative slogans");
        Console.WriteLine("    - FeedbackProvider: Reviews and rates slogans (minimum rating: 8/10)");
        Console.WriteLine("    - Maximum attempts: 3 iterations");
        
        Console.WriteLine("\n?? How to Use:");
        Console.WriteLine($"  1. Open your browser to: {url}/devui");
        Console.WriteLine("  2. Select 'slogan-writer-workflow' from the agent dropdown");
        Console.WriteLine("  3. Type your slogan request, for example:");
        Console.WriteLine("     'Create a slogan for a new electric SUV that is affordable and fun to drive.'");
        Console.WriteLine("  4. Watch the workflow iterate between writer and feedback until:");
        Console.WriteLine("     • The slogan receives a rating of 8/10 or higher, OR");
        Console.WriteLine("     • Maximum 3 attempts are reached");
        Console.WriteLine("  5. View traces showing each iteration and quality improvement");
        
        Console.WriteLine("\n?? Example Inputs:");
        Console.WriteLine("  • 'Create a slogan for a new electric SUV'");
        Console.WriteLine("  • 'Generate a slogan for an eco-friendly water bottle'");
        Console.WriteLine("  • 'Write a slogan for a smart home security system'");
        
        Console.WriteLine("\n?? Workflow Features:");
        Console.WriteLine("  ? Custom executors with conversation memory");
        Console.WriteLine("  ? Quality-based iteration (rating threshold)");
        Console.WriteLine("  ? Safety limit (max 3 attempts)");
        Console.WriteLine("  ? Real-time progress events");
        Console.WriteLine("  ? Structured output (JSON schemas)");
        
        Console.WriteLine("\n??  Press Ctrl+C to stop the server");
        Console.WriteLine(new string('=', 80) + "\n");

        await app.RunAsync();
    }

    // ====================================
    // Data Models
    // ====================================

    /// <summary>
    /// A class representing the output of the slogan writer agent.
    /// </summary>
    private sealed class SloganResult
    {
        [JsonPropertyName("task")]
        public required string Task { get; set; }

        [JsonPropertyName("slogan")]
        public required string Slogan { get; set; }
    }

    /// <summary>
    /// A class representing the output of the feedback agent.
    /// </summary>
    private sealed class FeedbackResult
    {
        [JsonPropertyName("comments")]
        public string Comments { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("actions")]
        public string Actions { get; set; } = string.Empty;
    }

    // ====================================
    // Custom Workflow Events
    // ====================================

    /// <summary>
    /// A custom event to indicate that a slogan has been generated.
    /// </summary>
    private sealed class SloganGeneratedEvent(SloganResult sloganResult) : WorkflowEvent(sloganResult)
    {
        public override string ToString() => $"Slogan: {sloganResult.Slogan}";
    }

    /// <summary>
    /// A custom event to indicate that feedback has been provided.
    /// </summary>
    private sealed class FeedbackEvent(FeedbackResult feedbackResult) : WorkflowEvent(feedbackResult)
    {
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        public override string ToString() => $"Feedback:\n{JsonSerializer.Serialize(feedbackResult, this._options)}";
    }

    // ====================================
    // Custom Executors
    // ====================================

    /// <summary>
    /// A custom executor that uses an AI agent to generate slogans based on a given task.
    /// Note that this executor has two message handlers:
    /// 1. HandleAsync(ChatMessage message): Handles the initial task to create a slogan from DevUI.
    /// 2. HandleAsync(FeedbackResult message): Handles feedback to improve the slogan.
    /// </summary>
    private sealed class SloganWriterExecutor : Executor
    {
        private readonly AIAgent _agent;
        private readonly AgentThread _thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="SloganWriterExecutor"/> class.
        /// </summary>
        /// <param name="id">A unique identifier for the executor.</param>
        /// <param name="chatClient">The chat client to use for the AI agent.</param>
        public SloganWriterExecutor(string id, IChatClient chatClient) : base(id)
        {
            ChatClientAgentOptions agentOptions = new()
            {
                ChatOptions = new()
                {
                    Instructions = "You are a professional slogan writer. You will be given a task to create a slogan.",
                    ResponseFormat = ChatResponseFormat.ForJsonSchema<SloganResult>()
                }
            };

            this._agent = new ChatClientAgent(chatClient, agentOptions);
            this._thread = this._agent.GetNewThread();
        }

        protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder) =>
            routeBuilder.AddHandler<ChatMessage, SloganResult>(this.HandleAsync)
                        .AddHandler<FeedbackResult, SloganResult>(this.HandleAsync);

        // Handle initial request from DevUI (ChatMessage input)
        public async ValueTask<SloganResult> HandleAsync(ChatMessage message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"\n?? SloganWriter: Processing initial request");
            Console.WriteLine($"   Task: {message.Text}");
            
            var result = await this._agent.RunAsync(message.Text ?? "", this._thread, cancellationToken: cancellationToken);

            var sloganResult = JsonSerializer.Deserialize<SloganResult>(result.Text) ?? throw new InvalidOperationException("Failed to deserialize slogan result.");

            Console.WriteLine($"   Generated slogan: {sloganResult.Slogan}");
            
            await context.AddEventAsync(new SloganGeneratedEvent(sloganResult), cancellationToken);
            return sloganResult;
        }

        // Handle feedback iteration
        public async ValueTask<SloganResult> HandleAsync(FeedbackResult message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"\n?? SloganWriter: Processing feedback");
            Console.WriteLine($"   Rating received: {message.Rating}/10");
            Console.WriteLine($"   Comments: {message.Comments}");
            
            var feedbackMessage = $"""
                Here is the feedback on your previous slogan:
                Comments: {message.Comments}
                Rating: {message.Rating}
                Suggested Actions: {message.Actions}

                Please use this feedback to improve your slogan.
                """;

            var result = await this._agent.RunAsync(feedbackMessage, this._thread, cancellationToken: cancellationToken);
            var sloganResult = JsonSerializer.Deserialize<SloganResult>(result.Text) ?? throw new InvalidOperationException("Failed to deserialize slogan result.");

            Console.WriteLine($"   Revised slogan: {sloganResult.Slogan}");
            
            await context.AddEventAsync(new SloganGeneratedEvent(sloganResult), cancellationToken);
            return sloganResult;
        }
    }

    /// <summary>
    /// A custom executor that uses an AI agent to provide feedback on a slogan.
    /// </summary>
    private sealed class FeedbackExecutor : Executor<SloganResult>
    {
        private readonly AIAgent _agent;
        private readonly AgentThread _thread;

        public int MinimumRating { get; init; } = 8;

        public int MaxAttempts { get; init; } = 3;

        private int _attempts;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackExecutor"/> class.
        /// </summary>
        /// <param name="id">A unique identifier for the executor.</param>
        /// <param name="chatClient">The chat client to use for the AI agent.</param>
        public FeedbackExecutor(string id, IChatClient chatClient) : base(id)
        {
            ChatClientAgentOptions agentOptions = new()
            {
                ChatOptions = new()
                {
                    Instructions = "You are a professional editor. You will be given a slogan and the task it is meant to accomplish.",
                    ResponseFormat = ChatResponseFormat.ForJsonSchema<FeedbackResult>()
                }
            };

            this._agent = new ChatClientAgent(chatClient, agentOptions);
            this._thread = this._agent.GetNewThread();
        }

        public override async ValueTask HandleAsync(SloganResult message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"\n?? FeedbackProvider: Reviewing slogan (Attempt {_attempts + 1}/{MaxAttempts})");
            Console.WriteLine($"   Task: {message.Task}");
            Console.WriteLine($"   Slogan: {message.Slogan}");
            
            var sloganMessage = $"""
                Here is a slogan for the task '{message.Task}':
                Slogan: {message.Slogan}
                Please provide feedback on this slogan, including comments, a rating from 1 to 10, and suggested actions for improvement.
                """;

            var response = await this._agent.RunAsync(sloganMessage, this._thread, cancellationToken: cancellationToken);
            var feedback = JsonSerializer.Deserialize<FeedbackResult>(response.Text) ?? throw new InvalidOperationException("Failed to deserialize feedback.");

            Console.WriteLine($"   Rating: {feedback.Rating}/10");
            Console.WriteLine($"   Comments: {feedback.Comments}");
            
            await context.AddEventAsync(new FeedbackEvent(feedback), cancellationToken);

            if (feedback.Rating >= this.MinimumRating)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n? SLOGAN APPROVED! (Rating: {feedback.Rating}/10)");
                Console.ResetColor();
                
                var outputMessage = $"The following slogan was accepted:\n\n{message.Slogan}";
                await context.YieldOutputAsync(new ChatMessage(ChatRole.Assistant, outputMessage), cancellationToken);
                return;
            }

            if (this._attempts >= this.MaxAttempts)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n?? MAX ATTEMPTS REACHED ({this.MaxAttempts})");
                Console.ResetColor();
                
                var outputMessage = $"The slogan was rejected after {this.MaxAttempts} attempts. Final slogan:\n\n{message.Slogan}";
                await context.YieldOutputAsync(new ChatMessage(ChatRole.Assistant, outputMessage), cancellationToken);
                return;
            }

            Console.WriteLine($"   ? Sending feedback to SloganWriter for revision...");
            
            await context.SendMessageAsync(feedback, cancellationToken: cancellationToken);
            this._attempts++;
        }
    }
}
