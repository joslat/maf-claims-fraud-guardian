// SPDX-License-Identifier: LicenseRef-MAFClaimsFraudGuardian-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using ClaimsCore.Workflows.Workflows;
using ClaimsCore.Workflows.DevUI;

namespace ClaimsCore.Workflows;

/// <summary>
/// Claims Core Workflows Application
/// 
/// This application provides AI-powered workflow orchestration for:
/// - Claims Intake Workflow: Interactive claims processing with ClaimsCore.Common models
/// - Claims Fraud Detection Workflow: Multi-agent parallel fraud analysis
/// 
/// Uses Microsoft.Agents.AI.Workflows for multi-agent orchestration
/// and ClaimsCore.Common for shared data models.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("📋 Claims Core Workflows Application");
        Console.WriteLine("=====================================\n");
        
        // Check for required environment variables
        if (!ValidateEnvironment())
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("Available Workflows:");
            Console.WriteLine("  1 - Claims Intake Workflow");
            Console.WriteLine("  2 - Claims Fraud Detection Workflow");
            Console.WriteLine("  3 - Claims Fraud Detection with DevUI");
            Console.WriteLine("  4 - Sample: Agent Workflow with DevUI");
            //Console.WriteLine("  5 - Sample: Agent Executors Workflow with DevUI");
            //Console.WriteLine("  6 - Sample: Function Executor Workflow with DevUI");
            Console.WriteLine("  q - Quit");
            Console.WriteLine(new string('=', 60));
            Console.Write("\nSelect a workflow (1-4, or q): ");

            var choice = Console.ReadLine()?.Trim().ToLower();

            switch (choice)
            {
                case "1":
                    await RunClaimsIntakeWorkflow();
                    break;
                case "2":
                    await RunClaimsFraudDetectionWorkflow();
                    break;
                case "3":
                    await RunClaimsFraudDetectionWithDevUI();
                    break;
                case "4":
                    await RunSampleAgentWorkflowBasic();
                    break;
                //case "5":
                //    await RunSampleAgentExecutorsWorkflow();
                //    break;
                //case "6":
                //    await RunSampleExecutorWorkflow();
                //    break;
                case "q":
                case "quit":
                    exit = true;
                    Console.WriteLine("\n👋 Goodbye!");
                    break;
                default:
                    Console.WriteLine("\n⚠️  Invalid choice. Please select 1-4, or q.");
                    break;
            }
        }
    }

    private static bool ValidateEnvironment()
    {
        try
        {
            // Check if environment variables are set by trying to access AIConfig
            var endpoint = AIConfig.Endpoint;
            var credential = AIConfig.KeyCredential;
            
            Console.WriteLine("✅ Azure OpenAI configuration validated");
            Console.WriteLine($"   Endpoint: {endpoint}");
            return true;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Environment Configuration Error:");
            Console.WriteLine($"   {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("\nPlease set the following environment variables:");
            Console.WriteLine("  - AZURE_OPENAI_ENDPOINT: Your Azure OpenAI endpoint URL");
            Console.WriteLine("  - AZURE_OPENAI_API_KEY: Your Azure OpenAI API key");
            Console.WriteLine("\nExample:");
            Console.WriteLine("  setx AZURE_OPENAI_ENDPOINT \"https://your-resource.openai.azure.com/\"");
            Console.WriteLine("  setx AZURE_OPENAI_API_KEY \"your-api-key-here\"");
            return false;
        }
    }

    private static async Task RunClaimsIntakeWorkflow()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🚀 Claims Intake Workflow");
        Console.WriteLine(new string('=', 60) + "\n");
        Console.WriteLine("ℹ️  Interactive claims processing workflow:");
        Console.WriteLine("   • Conversational AI-powered claim gathering");
        Console.WriteLine("   • Customer identification and validation");
        Console.WriteLine("   • Contract resolution and verification");
        Console.WriteLine("   • Real-time data quality checks\n");
        
        try
        {
            await ClaimsIntakeWorkflow.Execute();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error running Claims Intake Workflow: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
        }
        
        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
    }

    private static async Task RunClaimsFraudDetectionWorkflow()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🚀 Claims Fraud Detection Workflow");
        Console.WriteLine(new string('=', 60) + "\n");
        Console.WriteLine("ℹ️  Multi-agent parallel fraud analysis:");
        Console.WriteLine("   • OSINT validation (online marketplace checks)");
        Console.WriteLine("   • Customer history and risk analysis");
        Console.WriteLine("   • Transaction-level fraud scoring");
        Console.WriteLine("   • AI-powered fraud determination\n");
        
        Console.WriteLine(new string('-', 60));
        Console.WriteLine("Select Risk Scenario:");
        Console.WriteLine("  1 - HIGH RISK (default)");
        Console.WriteLine("      Customer: CUST-10001 (John Smith)");
        Console.WriteLine("      Fraud Score: 65 | Value: CHF 15,000 | Days ago: 2");
        Console.WriteLine("      Item: Hello Kitty Collector's Edition Mountain Bike");
        Console.WriteLine("      Expected: INVESTIGATE\n");
        Console.WriteLine("  2 - LOW RISK");
        Console.WriteLine("      Customer: CUST-67890 (Maria Garcia)");
        Console.WriteLine("      Fraud Score: 10 | Value: CHF 850 | Days ago: 45");
        Console.WriteLine("      Item: Samsung Galaxy S23 mobile phone");
        Console.WriteLine("      Expected: APPROVE\n");
        Console.WriteLine("  3 - MODERATE RISK");
        Console.WriteLine("      Customer: CUST-10003 (Alice Johnson)");
        Console.WriteLine("      Fraud Score: 10 | Value: CHF 38,000 | Days ago: 1");
        Console.WriteLine("      Item: 2022 Volkswagen Golf GTI");
        Console.WriteLine("      Expected: INVESTIGATE");
        Console.WriteLine(new string('-', 60));
        Console.Write("Enter scenario (1, 2, or 3) [default: 1]: ");
        
        var scenarioInput = Console.ReadLine()?.Trim();
        var scenario = 1; // Default to high risk
        
        if (!string.IsNullOrEmpty(scenarioInput) && int.TryParse(scenarioInput, out int parsedScenario))
        {
            if (parsedScenario >= 1 && parsedScenario <= 3)
            {
                scenario = parsedScenario;
            }
            else
            {
                Console.WriteLine("⚠️  Invalid scenario. Using default (1 - HIGH RISK).");
            }
        }
        
        try
        {
            await ClaimsFraudDetectionWorkflow.Execute(scenario);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error running Claims Fraud Detection Workflow: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
        }
        
        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
    }

    private static async Task RunClaimsFraudDetectionWithDevUI()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🌐 Claims Fraud Detection Workflow - DevUI");
        Console.WriteLine(new string('=', 60) + "\n");
        Console.WriteLine("ℹ️  Browser-based workflow visualization:");
        Console.WriteLine("   • Interactive step-by-step execution");
        Console.WriteLine("   • Real-time agent outputs and fraud scoring");
        Console.WriteLine("   • Workflow state inspection");
        Console.WriteLine("   • Visual fan-out/fan-in pattern display\n");
        
        Console.WriteLine(new string('-', 60));
        Console.WriteLine("Select Default Risk Scenario:");
        Console.WriteLine("  1 - HIGH RISK (default)");
        Console.WriteLine("      Hello Kitty Bike - CHF 15,000");
        Console.WriteLine("  2 - LOW RISK");
        Console.WriteLine("      Samsung Phone - CHF 850");
        Console.WriteLine("  3 - MODERATE RISK");
        Console.WriteLine("      VW Golf GTI - CHF 38,000");
        Console.WriteLine(new string('-', 60));
        Console.Write("Enter scenario (1, 2, or 3) [default: 1]: ");
        
        var scenarioInput = Console.ReadLine()?.Trim();
        var scenario = 1;
        
        if (!string.IsNullOrEmpty(scenarioInput) && int.TryParse(scenarioInput, out int parsedScenario))
        {
            if (parsedScenario >= 1 && parsedScenario <= 3)
            {
                scenario = parsedScenario;
            }
        }

        Console.Write("\nEnter port number [default: 5173]: ");
        var portInput = Console.ReadLine()?.Trim();
        var port = 5173;
        
        if (!string.IsNullOrEmpty(portInput) && int.TryParse(portInput, out int parsedPort))
        {
            port = parsedPort;
        }
        
        try
        {
            await ClaimsFraudDetectionWorkflow_DevUI.ExecuteWithDevUI(scenario, port);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error running Claims Fraud Detection with DevUI: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
            
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }

    private static async Task RunSampleAgentWorkflowBasic()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🌐 Sample: Agent Workflow Basic with DevUI");
        Console.WriteLine(new string('=', 60) + "\n");
        Console.WriteLine("ℹ️  This sample demonstrates:");
        Console.WriteLine("   • Setting up simple AI agents with DevUI");
        Console.WriteLine("   • Creating a sequential workflow");
        Console.WriteLine("   • Interactive browser-based testing");
        Console.WriteLine("   • Real-time agent interaction\n");
        
        try
        {
            await Sample_AgentWorkflowBasic.Execute();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error running Sample Agent Workflow Basic: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
            
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }

    private static async Task RunSampleAgentExecutorsWorkflow()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🌐 Sample: Agent Executors Workflow with DevUI");
        Console.WriteLine(new string('=', 60) + "\n");
        Console.WriteLine("ℹ️  This sample demonstrates:");
        Console.WriteLine("   • Custom AI agent executors with conversation memory");
        Console.WriteLine("   • Iterative refinement workflow (writer + feedback loop)");
        Console.WriteLine("   • Quality-based decision making (rating threshold)");
        Console.WriteLine("   • Safety limits (max iteration attempts)");
        Console.WriteLine("   • Real-time progress tracking and custom events\n");
        
        try
        {
            await Sample_AgentExecutorsWorkflow.Execute();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error running Sample Agent Executors Workflow: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
            
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }

    private static async Task RunSampleExecutorWorkflow()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🌐 Sample: Function Executor Workflow with DevUI");
        Console.WriteLine(new string('=', 60) + "\n");
        Console.WriteLine("ℹ️  This sample demonstrates:");
        Console.WriteLine("   • Using FunctionExecutor for lightweight custom logic");
        Console.WriteLine("   • Building workflows without creating executor classes");
        Console.WriteLine("   • Message transformation pipeline (3 stages)");
        Console.WriteLine("   • Mix of sync and async function handlers");
        Console.WriteLine("   • Content-based processing (greetings, help, urgent, standard)\n");
        
        try
        {
            await Sample_ExecutorWorkflow.Execute();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error running Sample Executor Workflow: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
            
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }
}




















