using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Lara;

namespace Lara.SDK.Examples
{
    /**
     * Complete memory management examples for the Lara .NET SDK
     *
     * This example demonstrates:
     * - Create, list, update, delete memories
     * - Add individual translations
     * - Multiple memory operations
     * - TMX file import with progress monitoring
     * - Translation deletion
     * - Translation with TUID and context
     */
    public static class MemoriesManagement
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
            string? memoryId = null;
            string? memory2ToDelete = null;

            // Example 1: Basic memory management
            Console.WriteLine("=== Basic Memory Management ===");
            try
            {
                var memory = await lara.Memories.Create("MyDemoMemory");
                Console.WriteLine($"Created memory: {memory.Name} (ID: {memory.Id})");

                // Get memory details
                var retrievedMemory = await lara.Memories.Get(memory.Id);
                if (retrievedMemory != null)
                {
                    Console.WriteLine($"Memory: {retrievedMemory.Name} (Owner: {retrievedMemory.OwnerId})");
                }

                // Update memory
                var updatedMemory = await lara.Memories.Update(memory.Id, "UpdatedDemoMemory");
                Console.WriteLine($"Updated name: '{memory.Name}' -> '{updatedMemory.Name}'");
                Console.WriteLine();

                // List all memories
                var memories = await lara.Memories.List();
                Console.WriteLine($"Total memories: {memories.Count}\n");

                // Store the memory ID for later examples
                memoryId = memory.Id;
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error creating memory: {e.Message}\n");
                return;
            }

            // Example 2: Adding translations
            // Important: To update/overwrite a translation unit you must provide a tuid. Calls without a tuid always create a new unit and will not update existing entries.
            Console.WriteLine("=== Adding Translations ===");
            try
            {
                // Basic translation addition (with TUID)
                var memImport1 = await lara.Memories.AddTranslation(memoryId!, "en-US", "fr-FR", "Hello", "Bonjour", "greeting_001");
                Console.WriteLine($"Added: 'Hello' -> 'Bonjour' with TUID 'greeting_001' (Import ID: {memImport1.Id})");

                // Translation with context
                var memImport2 = await lara.Memories.AddTranslation(
                    memoryId!, "en-US", "fr-FR", "How are you?", "Comment allez-vous?", "greeting_002",
                    "Good morning", "Have a nice day"
                );
                Console.WriteLine($"Added with context (Import ID: {memImport2.Id})\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error adding translations: {e.Message}\n");
            }

            // Example 3: Multiple memory operations
            Console.WriteLine("=== Multiple Memory Operations ===");
            try
            {
                // Create second memory for multi-memory operations
                var memory2 = await lara.Memories.Create("SecondDemoMemory");
                var memory2Id = memory2.Id;
                Console.WriteLine($"Created second memory: {memory2.Name}");

                // Add translation to multiple memories (with TUID)
                var memoryIds = new List<string> { memoryId!, memory2Id };
                var multiImportJob = await lara.Memories.AddTranslation(memoryIds, "en-US", "it-IT", "Hello World!", "Ciao Mondo!", "greeting_003");
                Console.WriteLine($"Added translation to multiple memories (Import ID: {multiImportJob.Id})\n");

                // Store for cleanup
                memory2ToDelete = memory2Id;
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error with multiple memory operations: {e.Message}\n");
                memory2ToDelete = null;
            }

            // Example 4: TMX import functionality
            Console.WriteLine("=== TMX Import Functionality ===");

            // Replace with your actual TMX file path
            var tmxFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample_memory.tmx");

            if (File.Exists(tmxFilePath))
            {
                try
                {
                    Console.WriteLine($"Importing TMX file: {Path.GetFileName(tmxFilePath)}");
                    var import = await lara.Memories.ImportTmx(memoryId!, tmxFilePath);
                    Console.WriteLine($"Import started with ID: {import.Id}");
                    Console.WriteLine($"Initial progress: {import.Progress * 100}%");

                    // Wait for import to complete
                    try
                    {
                        var progressCallback = new Action<MemoryImport>(mi =>
                        {
                            Console.WriteLine($"Progress update: {mi.Progress * 100}%");
                        });
                        var completedImport = await lara.Memories.WaitForImport(import, progressCallback, TimeSpan.FromSeconds(10));
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
                    Console.WriteLine($"Error with TMX import: {e.Message}\n");
                }
            }
            else
            {
                Console.WriteLine($"TMX file not found: {tmxFilePath}");
            }

            // Example 5: Translation deletion
            Console.WriteLine("=== Translation Deletion ===");
            try
            {
                // Delete a specific translation unit (with TUID)
                // Important: if you omit tuid, all entries that match the provided fields will be removed
                var deleteJob = await lara.Memories.DeleteTranslation(
                    memoryId!,
                    "en-US",
                    "fr-FR",
                    "Hello",
                    "Bonjour",
                    "greeting_001"  // Specify the TUID to delete a specific translation unit
                );
                Console.WriteLine($"Deleted translation unit (Job ID: {deleteJob.Id})\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error deleting translation: {e.Message}\n");
            }

            // Cleanup
            Console.WriteLine("=== Cleanup ===");
            try
            {
                if (memoryId != null)
                {
                    var deletedMemory = await lara.Memories.Delete(memoryId);
                    Console.WriteLine($"Deleted memory: {deletedMemory.Name}");
                }

                if (memory2ToDelete != null)
                {
                    var deletedMemory2 = await lara.Memories.Delete(memory2ToDelete);
                    Console.WriteLine($"Deleted second memory: {deletedMemory2.Name}");
                }
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error deleting memory: {e.Message}");
            }

            Console.WriteLine("\nMemory management examples completed!");
        }
    }
}