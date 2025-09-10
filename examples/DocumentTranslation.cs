using System;
using System.IO;
using System.Threading.Tasks;
using Lara;

namespace Lara.SDK.Examples
{
    /**
     * Complete document translation examples for the Lara .NET SDK
     *
     * This example demonstrates:
     * - Basic document translation
     * - Advanced options with memories and glossaries
     * - Step-by-step document translation with status monitoring
     */
    public static class DocumentTranslation
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

            // Replace with your actual document file path
            var sampleFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample_document.txt");

            if (!File.Exists(sampleFilePath))
            {
                Console.WriteLine($"Please create a sample document file at: {sampleFilePath}");
                Console.WriteLine("Add some sample text content to translate.\n");
                return;
            }

            await RunExamples(lara, sampleFilePath);
        }

        private static async Task RunExamples(Translator lara, string sampleFilePath)
        {

            // Example 1: Basic document translation
            Console.WriteLine("=== Basic Document Translation ===");
            var sourceLang = "en-US";
            var targetLang = "de-DE";
            
            Console.WriteLine($"Translating document: {Path.GetFileName(sampleFilePath)} from {sourceLang} to {targetLang}");
            
            try
            {
                var translatedStream = await lara.Documents.Translate(sampleFilePath, sourceLang, targetLang);
                
                // Save translated document - replace with your desired output path
                var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample_document_translated.txt");
                using var outputFile = File.Create(outputPath);
                await translatedStream.CopyToAsync(outputFile);
                
                Console.WriteLine("Document translation completed");
                Console.WriteLine($"Translated file saved to: {Path.GetFileName(outputPath)}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error translating document: {e.Message}\n");
            }

            // Example 2: Document translation with advanced options
            Console.WriteLine("=== Document Translation with Advanced Options ===");
            try
            {
                var translationOptions = new DocumentTranslateOptions
                {
                    AdaptTo = new string[] { "mem_1A2b3C4d5E6f7G8h9I0jKl" },  // Replace with actual memory IDs
                    Glossaries = new string[] { "gls_1A2b3C4d5E6f7G8h9I0jKl" }  // Replace with actual glossary IDs
                };
                
                var translatedStream = await lara.Documents.Translate(sampleFilePath, sourceLang, targetLang, translationOptions);
                
                // Save translated document - replace with your desired output path
                var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "advanced_document_translated.txt");
                using var outputFile = File.Create(outputPath);
                await translatedStream.CopyToAsync(outputFile);
                
                Console.WriteLine("Advanced document translation completed");
                Console.WriteLine($"Translated file saved to: {Path.GetFileName(outputPath)}");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error in advanced translation: {e.Message}\n");
            }

            // Example 3: Step-by-step document translation with status monitoring
            Console.WriteLine("=== Step-by-Step Document Translation ===");
            
            try
            {
                // Upload document
                Console.WriteLine("Step 1: Uploading document...");
                var uploadOptions = new DocumentUploadOptions
                {
                    AdaptTo = new string[] { "mem_1A2b3C4d5E6f7G8h9I0jKl" },  // Replace with actual memory ID
                    Glossaries = new string[] { "gls_1A2b3C4d5E6f7G8h9I0jKl" }  // Replace with actual glossary ID
                };
                
                var document = await lara.Documents.Upload(sampleFilePath, sourceLang, targetLang, uploadOptions);
                Console.WriteLine($"Document uploaded with ID: {document.Id}");
                Console.WriteLine($"Initial status: {document.Status}");
                
                // Check status
                Console.WriteLine("\nStep 2: Checking status...");
                var updatedDocument = await lara.Documents.Status(document.Id);
                Console.WriteLine($"Current status: {updatedDocument.Status}");
                
                // Download translated document
                Console.WriteLine("\nStep 3: Downloading would happen after translation completes...");
                var downloadOptions = new DocumentDownloadOptions();

                Console.WriteLine("Step-by-step translation completed");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error in step-by-step process: {e.Message}\n");
            }
        }
    }
}