using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lara;

namespace Lara.SDK.Examples
{
    /**
     * Complete text translation examples for the Lara .NET SDK
     *
     * This example demonstrates:
     * - Single string translation
     * - Multiple strings translation
     * - Translation with instructions
     * - TextBlocks translation (mixed translatable/non-translatable content)
     * - Auto-detect source language
     * - Advanced translation options
     * - Get available languages
     */
    public static class TextTranslation
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
            // Example 1: Basic single string translation
            Console.WriteLine("=== Basic Single String Translation ===");
            try
            {
                var result = await lara.Translate("Hello, world!", "en-US", "fr-FR");
                Console.WriteLine("Original: Hello, world!");
                Console.WriteLine($"French: {result.Translation}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error translating text: {e.Message}\n");
                return;
            }

            // Example 2: Multiple strings translation
            Console.WriteLine("=== Multiple Strings Translation ===");
            try
            {
                var texts = new string[] { "Hello", "How are you?", "Goodbye" };
                var result = await lara.Translate(texts, "en-US", "es-ES");
                Console.WriteLine($"Original: [{string.Join(", ", texts)}]");
                Console.WriteLine($"Spanish: [{string.Join(", ", result.Translation)}]\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error translating multiple texts: {e.Message}\n");
                return;
            }

            // Example 3: TextBlocks translation (mixed translatable/non-translatable content)
            Console.WriteLine("=== TextBlocks Translation ===");
            try
            {
                var textBlocks = new List<TextBlock>
                {
                    new TextBlock("Adventure novels, mysteries, cookbooks—wait, who packed those?", true),
                    new TextBlock("<br>", false),  // Non-translatable HTML
                    new TextBlock("Suddenly, it doesn't feel so deserted after all.", true),
                    new TextBlock("<div class=\"separator\"></div>", false),  // Non-translatable HTML
                    new TextBlock("Every page you turn is a new journey, and the best part?", true),
                };

                var result = await lara.Translate(textBlocks, "en-US", "it-IT");
                var translations = result.TextBlocks;

                Console.WriteLine($"Original TextBlocks: {textBlocks.Count} blocks");
                Console.WriteLine($"Translated blocks: {translations?.Count ?? 0}");
                if (translations != null)
                {
                    for (int i = 0; i < translations.Count; i++)
                    {
                        Console.WriteLine($"Block {i + 1}: {translations[i]}");
                    }
                }
                Console.WriteLine();
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error with TextBlocks translation: {e.Message}\n");
                return;
            }

            // Example 4: Translation with instructions
            Console.WriteLine("=== Translation with Instructions ===");
            try
            {
                var options = new TranslateOptions
                {
                    Instructions = new string[] { "Be formal", "Use technical terminology" }
                };

                var result = await lara.Translate("Could you send me the report by tomorrow morning?", "en-US", "de-DE", options);
                Console.WriteLine("Original: Could you send me the report by tomorrow morning?");
                Console.WriteLine($"German (formal): {result.Translation}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error with instructed translation: {e.Message}\n");
                return;
            }

            // Example 5: Auto-detecting source language
            Console.WriteLine("=== Auto-detect Source Language ===");
            try
            {
                var result = await lara.Translate("Bonjour le monde!", null, "en-US");
                Console.WriteLine("Original: Bonjour le monde!");
                Console.WriteLine($"Detected source: {result.SourceLanguage}");
                Console.WriteLine($"English: {result.Translation}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error with auto-detection: {e.Message}\n");
                return;
            }

            // Example 6: Advanced options with comprehensive settings
            Console.WriteLine("=== Translation with Advanced Options ===");
            try
            {
                var options = new TranslateOptions
                {
                    AdaptTo = new string[] { "mem_1A2b3C4d5E6f7G8h9I0jKl", "mem_2XyZ9AbC8dEf7GhI6jKlMn" },  // Replace with actual memory IDs
                    Glossaries = new string[] { "gls_1A2b3C4d5E6f7G8h9I0jKl", "gls_2XyZ9AbC8dEf7GhI6jKlMn" },  // Replace with actual glossary IDs
                    Instructions = new string[] { "Be professional" },
                    Style = TranslationStyle.Fluid,
                    ContentType = "text/plain",
                    TimeoutInMillis = 10000,
                    Priority = TranslatePriority.Normal
                };

                var result = await lara.Translate("This is a comprehensive translation example", "en-US", "it-IT", options);
                Console.WriteLine("Original: This is a comprehensive translation example");
                Console.WriteLine($"Italian (with all options): {result.Translation}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error with advanced translation: {e.Message}\n");
                return;
            }

            // Example 7: Get available languages
            Console.WriteLine("=== Available Languages ===");
            try
            {
                var languages = await lara.Languages();
                if (languages != null && languages.Count > 0)
                {
                    Console.WriteLine($"Supported languages: [{string.Join(", ", languages)}]");
                }
                else
                {
                    Console.WriteLine("No languages available");
                }
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error getting languages: {e.Message}");
                return;
            }
        }
    }
}