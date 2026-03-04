using System;
using System.IO;
using System.Threading.Tasks;
using Lara.Sdk;
using Lara.Sdk.Models.Authentication;

namespace Lara.SDK.Examples
{
    /**
     * Complete image translation examples for the Lara .NET SDK
     *
     * This example demonstrates:
     * - Basic image translation
     * - Advanced options with memories and glossaries
     * - Extracting and translating text from an image
     */
    public static class ImageTranslation
    {
        public static async Task RunExamples()
        {
            // All examples use environment variables for credentials, so set them first:
            // export LARA_ACCESS_KEY_ID="your-access-key-id"
            // export LARA_ACCESS_KEY_SECRET="your-access-key-secret"

            // Get credentials from environment variables
            var accessKeyId = Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_ID");
            var accessKeySecret = Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_SECRET");

            var credentials = new AccessKey(accessKeyId, accessKeySecret);
            var lara = new Translator(credentials);

            await RunExamples(lara);
        }

        private static async Task RunExamples(Translator lara)
        {
            // Replace with your actual image file path
            var sampleFilePath = Path.Combine(Directory.GetCurrentDirectory(), "textimage.png");

            if (!File.Exists(sampleFilePath))
            {
                Console.WriteLine($"Please create a sample image file at: {sampleFilePath}");
                return;
            }

            var sourceLang = "en";
            var targetLang = "de";

            // Example 1: Basic image translation (image output)
            Console.WriteLine("=== Basic Image Translation ===");
            Console.WriteLine($"Translating image: {Path.GetFileName(sampleFilePath)} from {sourceLang} to {targetLang}");

            try
            {
                var options = new ImageTranslateOptions
                {
                    TextRemoval = ImageTextRemoval.Overlay
                };

                var translatedStream = await lara.Images.Translate(sampleFilePath, sourceLang, targetLang, options);

                var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "sample_image_translated.png");
                await using (var fileStream = File.Create(outputPath))
                {
                    await translatedStream.CopyToAsync(fileStream);
                }

                Console.WriteLine("✅ Image translation completed");
                Console.WriteLine($"📄 Translated image saved to: {Path.GetFileName(outputPath)}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error translating image: {e.Message}\n");
                return;
            }

            // Example 2: Image translation with advanced options
            Console.WriteLine("=== Image Translation with Advanced Options ===");
            try
            {
                var options = new ImageTranslateOptions
                {
                    AdaptTo = new[] { "mem_1A2b3C4d5E6f7G8h9I0jKl" },  // Replace with actual memory IDs
                    Glossaries = new[] { "gls_1A2b3C4d5E6f7G8h9I0jKl" },  // Replace with actual glossary IDs
                    Style = TranslationStyle.Faithful,
                    TextRemoval = ImageTextRemoval.Inpainting
                };

                var translatedStream = await lara.Images.Translate(sampleFilePath, sourceLang, targetLang, options);

                var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "advanced_image_translated.png");
                await using (var fileStream = File.Create(outputPath))
                {
                    await translatedStream.CopyToAsync(fileStream);
                }

                Console.WriteLine("✅ Advanced image translation completed");
                Console.WriteLine($"📄 Translated image saved to: {Path.GetFileName(outputPath)}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error in advanced translation: {e.Message}");
            }
            Console.WriteLine();

            // Example 3: Extract and translate text from an image
            Console.WriteLine("=== Extract and Translate Text ===");
            try
            {
                var options = new ImageTextTranslateOptions
                {
                    AdaptTo = new[] { "mem_1A2b3C4d5E6f7G8h9I0jKl" },  // Replace with actual memory IDs
                    Glossaries = new[] { "gls_1A2b3C4d5E6f7G8h9I0jKl" },  // Replace with actual glossary IDs
                    Style = TranslationStyle.Faithful
                };

                var results = await lara.Images.TranslateText(sampleFilePath, sourceLang, targetLang, options);

                Console.WriteLine("✅ Extract and translate completed");
                Console.WriteLine($"Found {results.Paragraphs.Length} text blocks");

                for (int i = 0; i < results.Paragraphs.Length; i++)
                {
                    var paragraph = results.Paragraphs[i];
                    Console.WriteLine($"\nText Block {i + 1}:");
                    Console.WriteLine($"Original: {paragraph.Text}");
                    Console.WriteLine($"Translated: {paragraph.Translation}");
                }
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error extracting and translating text: {e.Message}");
            }
        }
    }
}
