using Lara.Sdk;
using Lara.Sdk.Models.Authentication;

namespace Lara.SDK.Examples
{
    /**
     * Audio transcript translation examples for the Lara .NET SDK.
     *
     * This example demonstrates:
     * - One-shot audio transcript translation
     * - Step-by-step upload, poll status, retrieve transcript flow
     */
    public static class AudioTranscriptTranslation
    {
        public static async Task RunExamples()
        {
            var accessKeyId = Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_ID");
            var accessKeySecret = Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_SECRET");

            if (string.IsNullOrEmpty(accessKeyId) || string.IsNullOrEmpty(accessKeySecret))
            {
                Console.WriteLine("Please set LARA_ACCESS_KEY_ID and LARA_ACCESS_KEY_SECRET environment variables.");
                return;
            }

            var accessKey = new AccessKey(accessKeyId, accessKeySecret);
            var lara = new Translator(accessKey);

            var sampleFilePath = "sample_audio.mp3";

            if (!File.Exists(sampleFilePath))
            {
                Console.WriteLine($"Please create a sample audio file at: {sampleFilePath}");
                Console.WriteLine("Supported formats: .wav, .mp3, .opus, .ogg, .webm\n");
                return;
            }

            await RunExamples(lara, sampleFilePath);
        }

        private static async Task RunExamples(Translator lara, string sampleFilePath)
        {
            var sourceLang = "en";
            var targetLang = "it";

            // Example 1: One-shot transcript translation
            Console.WriteLine("=== One-shot Audio Transcript Translation ===");
            try
            {
                var options = new AudioTranscriptOptions
                {
                    Style = TranslationStyle.Fluid,
                    NoTrace = false
                };

                var transcript = await lara.Audio.TranslateTranscript(sampleFilePath, sourceLang, targetLang, options);

                Console.WriteLine("✅ Transcript translation completed");
                Console.WriteLine($"Translation: {transcript.Translation}");
                Console.WriteLine($"Segments: {transcript.Segments.Count}\n");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error translating transcript: {e.Message}\n");
                return;
            }

            // Example 2: Step-by-step transcript translation
            Console.WriteLine("=== Step-by-Step Audio Transcript Translation ===");
            try
            {
                var options = new AudioTranscriptOptions
                {
                    NoTrace = false
                };

                Console.WriteLine("Step 1: Uploading audio for transcript translation...");
                var audio = await lara.Audio.UploadForTranscription(sampleFilePath, sourceLang, targetLang, options);
                Console.WriteLine($"Audio uploaded with ID: {audio.Id}");
                Console.WriteLine($"Initial status: {audio.Status}");

                Console.WriteLine("\nStep 2: Waiting for transcript translation to complete...");
                var status = audio;
                const int pollingIntervalMs = 2000;
                const int maxWaitTimeMs = 15 * 60 * 1000; // 15 minutes
                var start = DateTime.UtcNow;

                while (status.Status != AudioStatus.Translated && status.Status != AudioStatus.Error)
                {
                    if ((DateTime.UtcNow - start).TotalMilliseconds >= maxWaitTimeMs)
                    {
                        throw new LaraTimeoutException("Timeout waiting for transcript translation to complete");
                    }

                    await Task.Delay(pollingIntervalMs);
                    status = await lara.Audio.Status(audio.Id);
                    Console.WriteLine($"Current status: {status.Status}");
                }

                if (status.Status == AudioStatus.Error)
                {
                    var errorMessage = status.ErrorReason ?? "Transcript translation failed";
                    throw new LaraApiException(500, "AudioError", errorMessage);
                }

                Console.WriteLine("\nStep 3: Retrieving translated transcript...");
                var transcript = await lara.Audio.GetTranslatedTranscript(audio.Id);
                Console.WriteLine("✅ Step-by-step transcript translation completed");
                Console.WriteLine($"Translation: {transcript.Translation}");
                Console.WriteLine($"Segments: {transcript.Segments.Count}");
            }
            catch (LaraException e)
            {
                Console.WriteLine($"Error in step-by-step transcript process: {e.Message}\n");
            }
        }
    }
}
