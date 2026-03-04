using System;
using System.IO;
using System.Threading.Tasks;
using Lara.Sdk;
using Lara.Sdk.Models.Authentication;

namespace Lara.SDK.Examples
{
    /**
     * Complete audio translation examples for the Lara .NET SDK
     *
     *
     * This example demonstrates:
     * - Basic audio translation
     * - Advanced options with memories and glossaries
     * - Step-by-step audio translation with status monitoring
     */
    public static class AudioTranslation
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

            var accessKey = new AccessKey(accessKeyId, accessKeySecret);
            var lara = new Translator(accessKey);

            // Replace with your actual audio file path
            var sampleFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample_audio.mp3");

            if (!File.Exists(sampleFilePath))
            {
                Console.WriteLine($"Please create a sample audio file at: {sampleFilePath}");
                return;
            }

            await RunExamples(lara, sampleFilePath);
        }

        private static async Task RunExamples(Translator lara, string sampleFilePath)
        {
            // Example 1: Basic audio translation
            Console.WriteLine("=== Basic Audio Translation ===");
            var sourceLang = "en-US";
            var targetLang = "de-DE";

            Console.WriteLine($"Translating audio: {Path.GetFileName(sampleFilePath)} from {sourceLang} to {targetLang}");

            try
            {
                var translatedStream = await lara.Audio.Translate(sampleFilePath, sourceLang, targetLang);

                // Save translated audio - replace with your desired output path
                var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample_audio_translated.mp3");
                using var outputFile = File.Create(outputPath);
                await translatedStream.CopyToAsync(outputFile);

                Console.WriteLine("Audio translation completed");
                Console.WriteLine($"Translated file saved to: {Path.GetFileName(outputPath)}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error translating audio: {e.Message}\n");
            }

            // Example 2: Audio translation with advanced options
            Console.WriteLine("=== Audio Translation with Advanced Options ===");
            try
            {
                var translationOptions = new AudioUploadOptions
                {
                    AdaptTo = new string[] { "mem_1A2b3C4d5E6f7G8h9I0jKl" },  // Replace with actual memory IDs
                    Glossaries = new string[] { "gls_1A2b3C4d5E6f7G8h9I0jKl" }  // Replace with actual glossary IDs
                };

                var translatedStream = await lara.Audio.Translate(sampleFilePath, sourceLang, targetLang, translationOptions);

                // Save translated audio - replace with your desired output path
                var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "advanced_audio_translated.mp3");
                using var outputFile = File.Create(outputPath);
                await translatedStream.CopyToAsync(outputFile);

                Console.WriteLine("Advanced audio translation completed");
                Console.WriteLine($"Translated file saved to: {Path.GetFileName(outputPath)}");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error in advanced translation: {e.Message}\n");
            }

            // Example 3: Step-by-step audio translation with status monitoring
            Console.WriteLine("=== Step-by-Step Audio Translation ===");

            try
            {
                // Upload audio
                Console.WriteLine("Step 1: Uploading audio...");
                var uploadOptions = new AudioUploadOptions
                {
                    AdaptTo = new string[] { "mem_1A2b3C4d5E6f7G8h9I0jKl" },  // Replace with actual memory IDs
                    Glossaries = new string[] { "gls_1A2b3C4d5E6f7G8h9I0jKl" }  // Replace with actual glossary IDs
                };

                var audio = await lara.Audio.Upload(sampleFilePath, sourceLang, targetLang, uploadOptions);
                Console.WriteLine($"Audio uploaded with ID: {audio.Id}");
                Console.WriteLine($"Initial status: {audio.Status}");

                // Check status
                Console.WriteLine("\nStep 2: Checking status...");
                var updatedAudio = await lara.Audio.Status(audio.Id);
                Console.WriteLine($"Current status: {updatedAudio.Status}");

                // Download translated audio
                Console.WriteLine("\nStep 3: Downloading would happen after translation completes...");
                Console.WriteLine("Step-by-step translation completed");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error in step-by-step process: {e.Message}\n");
            }
        }
    }
}
