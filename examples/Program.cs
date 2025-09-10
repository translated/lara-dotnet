using System;
using System.Threading.Tasks;

namespace Lara.SDK.Examples
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Lara.SDK Examples");
                Console.WriteLine("=====================");
                Console.WriteLine();
                Console.WriteLine("Usage: dotnet run -- [example]");
                Console.WriteLine();
                Console.WriteLine("Available examples:");
                Console.WriteLine("  text-translation     - Text translation examples");
                Console.WriteLine("  document-translation - Document translation examples");
                Console.WriteLine("  memories-management  - Translation memory management");
                Console.WriteLine("  glossaries-management - Glossary management");
                Console.WriteLine();
                Console.WriteLine("Example: dotnet run -- text-translation");
                return;
            }

            var example = args[0].ToLowerInvariant();

            switch (example)
            {
                case "text-translation":
                    await TextTranslation.RunExamples();
                    break;
                case "document-translation":
                    await DocumentTranslation.RunExamples();
                    break;
                case "memories-management":
                    await MemoriesManagement.RunExamples();
                    break;
                case "glossaries-management":
                    await GlossariesManagement.RunExamples();
                    break;
                default:
                    Console.WriteLine($"Unknown example: {example}");
                    Console.WriteLine("Run without arguments to see available examples.");
                    break;
            }
        }
    }
}