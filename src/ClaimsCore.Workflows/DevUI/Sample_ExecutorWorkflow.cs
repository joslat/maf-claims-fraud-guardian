// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

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
/// Sample: Function Executor Workflow with DevUI
/// 
/// This sample demonstrates how to build workflows using custom Executors for processing logic.
/// Each executor handles both List&lt;ChatMessage&gt; (accumulates) and TurnToken (triggers processing).
/// 
/// This example creates a message processing pipeline with three stages:
/// 1. InputProcessor: Validates and preprocesses incoming messages
/// 2. MessageTransformer: Transforms message content with custom business logic
/// 3. OutputFormatter: Formats final output for display
/// 
/// DevUI Features Demonstrated:
/// - Custom executor classes with message accumulation
/// - TurnToken pattern for triggering processing
/// - Message transformation pipeline
/// - Chat protocol compatibility
/// - IResettableExecutor for cross-run state management
/// 
/// DevUI Endpoints:
/// - DevUI Interface: http://localhost:5000/devui
/// - OpenAI Responses API: http://localhost:5000/v1/responses
/// - OpenAI Conversations API: http://localhost:5000/v1/conversations
/// </summary>
/// <remarks>
/// This sample shows how to:
/// - Create custom executor classes that accumulate messages
/// - Use TurnToken to trigger processing after all messages are received
/// - Build multi-stage message transformation pipelines
/// - Maintain Chat Protocol compatibility for DevUI
/// - Implement IResettableExecutor for proper state cleanup
/// </remarks>
internal static class Sample_ExecutorWorkflow
{
    public static async Task Execute()
    {
        // Set console encoding to UTF-8 to support emojis and special characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Sample: Function Executor Workflow with DevUI ===\n");
        Console.WriteLine("Setting up web server with function executor workflow for DevUI...\n");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development // Force Development mode for DevUI
        });

        // ====================================
        // Register Message Processing Workflow
        // ====================================
        Console.WriteLine("Registering workflows:");
        
        builder.AddWorkflow("message-processor-workflow", (sp, key) =>
        {
            // Create the pipeline executors
            var inputProcessor = new InputProcessorExecutor();
            var messageTransformer = new MessageTransformerExecutor();
            var outputFormatter = new OutputFormatterExecutor();

            // Build the Workflow Pipeline
            // List<ChatMessage> ? InputProcessor ? MessageTransformer ? OutputFormatter ? List<ChatMessage>
            var workflow = new WorkflowBuilder(inputProcessor)
                .AddEdge(inputProcessor, messageTransformer)
                .AddEdge(messageTransformer, outputFormatter)
                .WithOutputFrom(outputFormatter)
                .WithName(key)
                .Build();

            return workflow;
        }).AddAsAIAgent();  // ?? CRITICAL: Required for DevUI compatibility!
        
        Console.WriteLine("  ? message-processor-workflow - Message processing pipeline with custom executors");

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
        Console.WriteLine("  • message-processor-workflow - Message processing pipeline");
        Console.WriteLine("    Stage 1: InputProcessor - Validates and adds timestamp");
        Console.WriteLine("    Stage 2: MessageTransformer - Applies content-based transformations");
        Console.WriteLine("    Stage 3: OutputFormatter - Formats final output with decorations");
        
        Console.WriteLine("\n?? How to Use:");
        Console.WriteLine($"  1. Open your browser to: {url}/devui");
        Console.WriteLine("  2. Select 'message-processor-workflow' from the agent dropdown");
        Console.WriteLine("  3. Type any message to see it processed through the pipeline");
        Console.WriteLine("  4. Try different message types:");
        Console.WriteLine("     • Greeting: 'Hello!' or 'Hi there'");
        Console.WriteLine("     • Help: 'I need help with...'");
        Console.WriteLine("     • Urgent: 'Urgent: Please assist'");
        Console.WriteLine("     • Standard: Any other message");
        Console.WriteLine("  5. Watch the console to see each executor processing the message");
        
        Console.WriteLine("\n?? Example Inputs:");
        Console.WriteLine("  • 'Hello, how are you?'");
        Console.WriteLine("  • 'I need help with my claim'");
        Console.WriteLine("  • 'Urgent: Car accident just happened'");
        Console.WriteLine("  • 'What's the weather today?'");
        
        Console.WriteLine("\n?? Workflow Features:");
        Console.WriteLine("  ? Custom executor classes with message accumulation");
        Console.WriteLine("  ? TurnToken pattern for processing triggers");
        Console.WriteLine("  ? IResettableExecutor for cross-run state management");
        Console.WriteLine("  ? Sequential pipeline processing");
        Console.WriteLine("  ? Content-based transformation logic");
        Console.WriteLine("  ? Formatted output with decorative elements");
        Console.WriteLine("  ? Chat Protocol compatible (List<ChatMessage> input/output)");
        
        Console.WriteLine("\n??  Press Ctrl+C to stop the server");
        Console.WriteLine(new string('=', 80) + "\n");

        await app.RunAsync();
    }
}

// ============================================================================
// EXECUTOR IMPLEMENTATIONS
// ============================================================================

/// <summary>
/// Stage 1: Input Processor Executor
/// Validates and preprocesses incoming messages, adds timestamp.
/// Accumulates messages until TurnToken triggers processing.
/// </summary>
internal sealed class InputProcessorExecutor : Executor, IResettableExecutor
{
    private List<ChatMessage> _pendingMessages = [];

    public InputProcessorExecutor() : base("InputProcessor", declareCrossRunShareable: true) { }

    protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
    {
        return routeBuilder
            .AddHandler<List<ChatMessage>>(HandleMessagesAsync)
            .AddHandler<TurnToken>(HandleTurnTokenAsync);
    }

    private ValueTask HandleMessagesAsync(List<ChatMessage> messages, IWorkflowContext context, CancellationToken ct)
    {
        // Accumulate messages - don't process yet, wait for TurnToken
        _pendingMessages.AddRange(messages);
        return default;
    }

    private async ValueTask HandleTurnTokenAsync(TurnToken token, IWorkflowContext context, CancellationToken ct)
    {
        Console.WriteLine($"\n?? InputProcessor: Received {_pendingMessages.Count} message(s)");

        var processedMessages = new List<ChatMessage>();
        
        foreach (var message in _pendingMessages)
        {
            Console.WriteLine($"   Content: {message.Text}");

            if (string.IsNullOrWhiteSpace(message.Text))
            {
                Console.WriteLine("   ??  Empty message - providing default");
                processedMessages.Add(new ChatMessage(ChatRole.User, "[Empty message - no content provided]"));
                continue;
            }

            // Preprocess: Add timestamp
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var processed = $"[{timestamp}] {message.Text}";
            
            Console.WriteLine($"   ? Preprocessed with timestamp");
            processedMessages.Add(new ChatMessage(message.Role, processed));
        }

        // Clear pending messages for next turn
        _pendingMessages = [];

        // Send processed messages to next executor
        await context.SendMessageAsync(processedMessages, cancellationToken: ct);

        // Forward TurnToken to trigger next executor
        await context.SendMessageAsync(token, cancellationToken: ct);
    }

    public ValueTask ResetAsync()
    {
        _pendingMessages = [];
        return default;
    }
}

/// <summary>
/// Stage 2: Message Transformer Executor
/// Applies content-based transformations to messages.
/// Accumulates messages until TurnToken triggers processing.
/// </summary>
internal sealed class MessageTransformerExecutor : Executor, IResettableExecutor
{
    private List<ChatMessage> _pendingMessages = [];

    public MessageTransformerExecutor() : base("MessageTransformer", declareCrossRunShareable: true) { }

    protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
    {
        return routeBuilder
            .AddHandler<List<ChatMessage>>(HandleMessagesAsync)
            .AddHandler<TurnToken>(HandleTurnTokenAsync);
    }

    private ValueTask HandleMessagesAsync(List<ChatMessage> messages, IWorkflowContext context, CancellationToken ct)
    {
        // Accumulate messages - wait for TurnToken
        _pendingMessages.AddRange(messages);
        return default;
    }

    private async ValueTask HandleTurnTokenAsync(TurnToken token, IWorkflowContext context, CancellationToken ct)
    {
        Console.WriteLine($"\n?? MessageTransformer: Processing {_pendingMessages.Count} message(s)");

        var transformedMessages = new List<ChatMessage>();

        foreach (var message in _pendingMessages)
        {
            var text = message.Text ?? "";
            string transformed;

            if (text.ToLower().Contains("hello") || text.ToLower().Contains("hi"))
            {
                transformed = $"GREETING DETECTED: {text}\n? Friendly tone enabled";
                Console.WriteLine("   ?? Detected greeting");
            }
            else if (text.ToLower().Contains("help"))
            {
                transformed = $"HELP REQUEST: {text}\n? Support mode activated";
                Console.WriteLine("   ?? Detected help request");
            }
            else if (text.ToLower().Contains("urgent") || text.ToLower().Contains("emergency"))
            {
                transformed = $"?? PRIORITY: {text}\n? High priority processing";
                Console.WriteLine("   ?? Detected urgent request");
            }
            else
            {
                transformed = $"STANDARD MESSAGE: {text}\n? Normal processing";
                Console.WriteLine("   ?? Standard message");
            }

            transformedMessages.Add(new ChatMessage(ChatRole.Assistant, transformed));
        }

        // Clear pending messages
        _pendingMessages = [];

        // Send transformed messages to next executor
        await context.SendMessageAsync(transformedMessages, cancellationToken: ct);

        // Forward TurnToken to trigger next executor
        await context.SendMessageAsync(token, cancellationToken: ct);
    }

    public ValueTask ResetAsync()
    {
        _pendingMessages = [];
        return default;
    }
}

/// <summary>
/// Stage 3: Output Formatter Executor
/// Formats final output with decorative elements and yields to workflow output.
/// Accumulates messages until TurnToken triggers processing.
/// </summary>
internal sealed class OutputFormatterExecutor : Executor, IResettableExecutor
{
    private List<ChatMessage> _pendingMessages = [];

    public OutputFormatterExecutor() : base("OutputFormatter", declareCrossRunShareable: true) { }

    protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
    {
        return routeBuilder
            .AddHandler<List<ChatMessage>>(HandleMessagesAsync)
            .AddHandler<TurnToken>(HandleTurnTokenAsync);
    }

    private ValueTask HandleMessagesAsync(List<ChatMessage> messages, IWorkflowContext context, CancellationToken ct)
    {
        // Accumulate messages - wait for TurnToken
        _pendingMessages.AddRange(messages);
        return default;
    }

    private async ValueTask HandleTurnTokenAsync(TurnToken token, IWorkflowContext context, CancellationToken ct)
    {
        Console.WriteLine($"\n?? OutputFormatter: Formatting {_pendingMessages.Count} message(s)");

        var formattedMessages = new List<ChatMessage>();

        foreach (var message in _pendingMessages)
        {
            var lines = (message.Text ?? "").Split('\n');
            var formatted = string.Join("\n", lines.Select(line => $"  {line}"));

            var finalOutput = $"""
                ????????????????????????????????????????????????????????????????
                ?  MESSAGE PROCESSING COMPLETE
                ????????????????????????????????????????????????????????????????
                {formatted}
                ????????????????????????????????????????????????????????????????
                ?  Processed by Function Executor Workflow
                ?  Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
                ????????????????????????????????????????????????????????????????
                """;

            formattedMessages.Add(new ChatMessage(ChatRole.Assistant, finalOutput));
        }

        Console.WriteLine("   ? Output formatted and ready");

        // Clear pending messages
        _pendingMessages = [];

        // YIELD the final output - this is what DevUI receives!
        // This is the final executor, so we yield instead of send
        await context.YieldOutputAsync(formattedMessages, ct);

        // Note: We don't forward TurnToken here since this is the last executor
        // and we've already yielded the output
    }

    public ValueTask ResetAsync()
    {
        _pendingMessages = [];
        return default;
    }
}
