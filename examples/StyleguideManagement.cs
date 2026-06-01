using System;
using System.Threading.Tasks;
using Lara.Sdk;

namespace Lara.SDK.Examples
{
    /**
     * Complete styleguide management examples for the Lara .NET SDK
     *
     * This example demonstrates:
     * - Create, list, get, update, delete styleguides
     */
    public static class StyleguideManagement
    {
        public static async Task RunExamples()
        {
            // All examples use environment variables for credentials, so set them first:
            // export LARA_ACCESS_KEY_ID="your-access-key-id"
            // export LARA_ACCESS_KEY_SECRET="your-access-key-secret"

            // Get credentials from environment variables
            var accessKeyId = Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_ID");
            var accessKeySecret = Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_SECRET");

            if (string.IsNullOrEmpty(accessKeyId) || string.IsNullOrEmpty(accessKeySecret))
            {
                Console.WriteLine("Please set LARA_ACCESS_KEY_ID and LARA_ACCESS_KEY_SECRET environment variables.");
                return;
            }

            var credentials = new Credentials(accessKeyId, accessKeySecret);
            var lara = new Translator(credentials);

            await RunExamples(lara);
        }

        private static async Task RunExamples(Translator lara)
        {
            Console.WriteLine("Styleguides require a specific subscription plan.");
            Console.WriteLine("If you encounter errors, please check your subscription level.");

            string? styleguideId = null;

            try
            {
                // Example 1: Basic styleguide management
                Console.WriteLine("=== Basic Styleguide Management ===");
                var initialContent = "Use a formal tone. Prefer British English spelling. Avoid contractions.";
                var styleguide = await lara.Styleguides.Create("MyDemoStyleguide", initialContent);
                Console.WriteLine($"Created styleguide: {styleguide.Name} (ID: {styleguide.Id})");
                styleguideId = styleguide.Id;

                // List all styleguides
                var styleguides = await lara.Styleguides.List();
                Console.WriteLine($"Total styleguides: {styleguides.Count}");
                Console.WriteLine();

                // Example 2: Styleguide operations
                Console.WriteLine("=== Styleguide Operations ===");
                // Get styleguide details
                var retrievedStyleguide = await lara.Styleguides.Get(styleguideId);
                if (retrievedStyleguide != null)
                {
                    Console.WriteLine($"Styleguide: {retrievedStyleguide.Name} (Owner: {retrievedStyleguide.OwnerId})");
                    Console.WriteLine($"   Personal: {retrievedStyleguide.IsPersonal}");
                    Console.WriteLine($"   Created at: {retrievedStyleguide.CreatedAt}");
                    if (!string.IsNullOrEmpty(retrievedStyleguide.Content))
                    {
                        var preview = retrievedStyleguide.Content.Length > 80
                            ? retrievedStyleguide.Content.Substring(0, 80)
                            : retrievedStyleguide.Content;
                        Console.WriteLine($"   Content preview: {preview}...");
                    }
                }
                Console.WriteLine();

                // Example 3: Update styleguide
                Console.WriteLine("=== Update Styleguide ===");
                // Update only the name
                var renamedStyleguide = await lara.Styleguides.Update(styleguideId, "UpdatedDemoStyleguide");
                Console.WriteLine($"Updated name: '{styleguide.Name}' -> '{renamedStyleguide.Name}'");

                // Update only the content
                var updatedContent = "Use a casual tone. Prefer American English spelling. Contractions are welcome.";
                var updatedStyleguide = await lara.Styleguides.Update(styleguideId, content: updatedContent);
                Console.WriteLine($"Updated content for styleguide: {updatedStyleguide.Name}");
                if (!string.IsNullOrEmpty(updatedStyleguide.Content))
                {
                    var preview = updatedStyleguide.Content.Length > 80
                        ? updatedStyleguide.Content.Substring(0, 80)
                        : updatedStyleguide.Content;
                    Console.WriteLine($"   New content preview: {preview}...");
                }

                // Update both name and content at the same time
                var fullyUpdatedStyleguide = await lara.Styleguides.Update(
                    styleguideId,
                    "FinalDemoStyleguide",
                    "Use clear and concise language. Avoid jargon."
                );
                Console.WriteLine($"Updated name and content: {fullyUpdatedStyleguide.Name}");
                Console.WriteLine();

                // Example 4: Get a non-existent styleguide
                Console.WriteLine("=== Get Non-Existent Styleguide ===");
                var missing = await lara.Styleguides.Get("non-existent-id");
                if (missing == null)
                {
                    Console.WriteLine("Styleguide not found (returned null as expected)");
                }
                Console.WriteLine();
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error during styleguide management: {e.Message}\n");
                return;
            }
            finally
            {
                // Cleanup
                Console.WriteLine("=== Cleanup ===");
                if (styleguideId != null)
                {
                    try
                    {
                        var deletedStyleguide = await lara.Styleguides.Delete(styleguideId);
                        Console.WriteLine($"Deleted styleguide: {deletedStyleguide.Name}");
                    }
                    catch (LaraException e)
                    {
                        Console.WriteLine($"Error deleting styleguide: {e.Message}");
                    }
                }
            }

            Console.WriteLine("\nStyleguide management examples completed!");
        }
    }
}
