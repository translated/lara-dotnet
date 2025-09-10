using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lara.Sdk;

namespace Lara.SDK.Examples
{
    /**
     * Complete glossary management examples for the Lara .NET SDK
     *
     * This example demonstrates:
     * - Create, list, update, delete glossaries
     * - CSV import with status monitoring
     * - Glossary export
     * - Glossary terms count
     * - Import status checking
     */
    public static class GlossariesManagement
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
            Console.WriteLine("Glossaries require a specific subscription plan.");
            Console.WriteLine("If you encounter errors, please check your subscription level.");

            string? glossaryId = null;

            // Example 1: Basic glossary management
            Console.WriteLine("=== Basic Glossary Management ===");
            try
            {
                var glossary = await lara.Glossaries.Create("MyDemoGlossary");
                Console.WriteLine($"Created glossary: {glossary.Name} (ID: {glossary.Id})");

                // List all glossaries
                var glossaries = await lara.Glossaries.List();
                Console.WriteLine($"Total glossaries: {glossaries.Count}");

                // Store the glossary ID for later examples
                glossaryId = glossary.Id;
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error creating glossary: {e.Message}");
                return;
            }

            // Example 2: Glossary operations
            Console.WriteLine("=== Glossary Operations ===");
            try
            {
                // Get glossary details
                var retrievedGlossary = await lara.Glossaries.Get(glossaryId!);
                if (retrievedGlossary != null)
                {
                    Console.WriteLine($"Glossary: {retrievedGlossary.Name} (Owner: {retrievedGlossary.OwnerId})");
                }

                // Get glossary statistics
                var counts = await lara.Glossaries.Counts(glossaryId!);
                if (counts.Unidirectional != null)
                {
                    foreach (var entry in counts.Unidirectional)
                    {
                        Console.WriteLine($"   {entry.Key}: {entry.Value} entries");
                    }
                }

                // Update glossary
                var updatedGlossary = await lara.Glossaries.Update(glossaryId!, "UpdatedDemoGlossary");
                Console.WriteLine($"Updated name: 'MyDemoGlossary' -> '{updatedGlossary.Name}'");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error with glossary operations: {e.Message}");
            }

            // Example 3: CSV import functionality
            Console.WriteLine("=== CSV Import Functionality ===");

            // Replace with your actual CSV file path
            var csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample_glossary.csv");

            if (File.Exists(csvFilePath))
            {
                try
                {
                    Console.WriteLine($"Importing CSV file: {Path.GetFileName(csvFilePath)}");
                    var import = await lara.Glossaries.ImportCsv(glossaryId!, csvFilePath);
                    Console.WriteLine($"Import started with ID: {import.Id}");
                    Console.WriteLine($"Initial progress: {import.Progress * 100}%");

                    // Check import status manually
                    Console.WriteLine("Checking import status...");
                    var importStatus = await lara.Glossaries.GetImportStatus(import.Id);
                    Console.WriteLine($"Current progress: {importStatus.Progress * 100}%");

                    // Wait for import to complete
                    try
                    {
                        var completedImport = await lara.Glossaries.WaitForImport(import, null, TimeSpan.FromSeconds(10));
                        Console.WriteLine("Import completed!");
                        Console.WriteLine($"Final progress: {completedImport.Progress * 100}%");
                    }
                    catch (LaraTimeoutException)
                    {
                        Console.WriteLine("Import timeout: The import process took too long to complete.");
                    }
                    Console.WriteLine();
                }
                catch (LaraException e)
                {
                    Console.WriteLine($"Error with CSV import: {e.Message}");
                }
            }
            else
            {
                Console.WriteLine($"CSV file not found: {csvFilePath}");
            }

            // Example 4: Export functionality with different formats
            Console.WriteLine("=== Export Functionality ===");
            try
            {
                // Export as standard CSV
                Console.WriteLine("Exporting as standard CSV...");
                using var csvStream = await lara.Glossaries.Export(glossaryId!, "csv/table-uni", "en-US");
                using var csvReader = new StreamReader(csvStream);
                var csvText = await csvReader.ReadToEndAsync();
                Console.WriteLine($"CSV unidirectional export successful ({csvText.Length} bytes)");
                
                // Save sample exports to files - replace with your desired output paths
                var exportFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exported_glossary.csv");
                await File.WriteAllTextAsync(exportFilePath, csvText);
                Console.WriteLine($"Sample export saved to: {Path.GetFileName(exportFilePath)}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error with export: {e.Message}\n");
            }

            // Example 5: Glossary Terms Count
            Console.WriteLine("=== Glossary Terms Count ===");
            try
            {
                // Get detailed counts
                var counts = await lara.Glossaries.Counts(glossaryId!);

                Console.WriteLine("Detailed glossary terms count:");

                if (counts.Unidirectional != null)
                {
                    Console.WriteLine("   Unidirectional entries by language pair:");
                    foreach (var entry in counts.Unidirectional)
                    {
                        Console.WriteLine($"     {entry.Key}: {entry.Value} terms");
                    }
                }
                else
                {
                    Console.WriteLine("   No unidirectional entries found");
                }

                var totalEntries = counts.Multidirectional;
                if (counts.Unidirectional != null)
                {
                    totalEntries += counts.Unidirectional.Values.Sum();
                }
                Console.WriteLine($"   Total entries: {totalEntries}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error getting glossary terms count: {e.Message}\n");
            }

            // Cleanup
            Console.WriteLine("=== Cleanup ===");
            try
            {
                if (glossaryId != null)
                {
                    var deletedGlossary = await lara.Glossaries.Delete(glossaryId);
                    Console.WriteLine($"Deleted glossary: {deletedGlossary.Name}");

                    // Clean up export files - replace with actual cleanup if needed
                    var exportFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exported_glossary.csv");
                    if (File.Exists(exportFilePath))
                    {
                        File.Delete(exportFilePath);
                        Console.WriteLine("Cleaned up export file");
                    }
                }
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error deleting glossary: {e.Message}");
            }

            Console.WriteLine("\nGlossary management examples completed!");
        }
    }
}