using ClaimsCore.Workflows.ClaimsIntegrated;

namespace ClaimsCore.Workflows;

/// <summary>
/// Claims Core Workflows Application
/// 
/// This application provides AI-powered workflow orchestration for:
/// - Demo11 Integrated: Claims Intake with ClaimsCore.Common models and ClaimsCoreMcp tools
/// - Demo12 Integrated: Fraud Detection with ClaimsCore.Common models and ClaimsCoreMcp tools
/// 
/// Uses Microsoft.Agents.AI.Workflows for multi-agent orchestration
/// and ClaimsCore.Common for shared data models.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("?? Claims Core Workflows Application");
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
            Console.WriteLine("Available Demos:");
            Console.WriteLine("  1 - Demo 11: Claims Intake Workflow (Integrated)");
            Console.WriteLine("  2 - Demo 12: Fraud Detection Workflow (Integrated)");
            Console.WriteLine("  q - Quit");
            Console.WriteLine(new string('=', 60));
            Console.Write("\nSelect a demo (1, 2, or q): ");

            var choice = Console.ReadLine()?.Trim().ToLower();

            switch (choice)
            {
                case "1":
                    await RunDemo11Integrated();
                    break;
                case "2":
                    await RunDemo12Integrated();
                    break;
                case "q":
                case "quit":
                    exit = true;
                    Console.WriteLine("\n👋 Goodbye!");
                    break;
                default:
                    Console.WriteLine("\n⚠️  Invalid choice. Please select 1, 2, or q.");
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

    private static async Task RunDemo11Integrated()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🚀 Starting Demo 11: Claims Intake Workflow (Integrated)");
        Console.WriteLine(new string('=', 60) + "\n");
        Console.WriteLine("ℹ️  This version uses:");
        Console.WriteLine("   • ClaimsCore.Common data models");
        Console.WriteLine("   • ClaimsCoreMcp.Tools for data access");
        Console.WriteLine("   • MockClaimsDataService for customer data\n");
        
        try
        {
            await Demo11_ClaimsWorkflow_Integrated.Execute();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error running Demo 11 Integrated: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
        }
        
        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
    }

    private static async Task RunDemo12Integrated()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🚀 Demo 12: Fraud Detection Workflow (Integrated)");
        Console.WriteLine(new string('=', 60) + "\n");
        Console.WriteLine("ℹ️  This version uses:");
        Console.WriteLine("   • ClaimsCore.Common data models");
        Console.WriteLine("   • ClaimsCoreMcp.Tools for fraud detection");
        Console.WriteLine("   • MockClaimsDataService for customer data\n");
        
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
            await Demo12_ClaimsFraudDetection_Integrated.Execute(scenario);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error running Demo 12 Integrated: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
        }
        
        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
    }
}
