# Lara .NET SDK

[![.NET Version](https://img.shields.io/badge/.net-9.0+-blue.svg)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

This SDK empowers you to build your own branded translation AI leveraging our translation fine-tuned language model. 

All major translation features are accessible, making it easy to integrate and customize for your needs. 

## 🌍 **Features:**
- **Text Translation**: Single strings, multiple strings, and complex text blocks
- **Document Translation**: Word, PDF, and other document formats with status monitoring
- **Translation Memory**: Store and reuse translations for consistency
- **Glossaries**: Enforce terminology standards across translations
- **Language Detection**: Automatic source language identification
- **Advanced Options**: Translation instructions and more

## 📚 Documentation

Lara's SDK full documentation is available at [https://developers.laratranslate.com/](https://developers.laratranslate.com/)

## 🚀 Quick Start

### Installation

```bash
dotnet add package Lara.Sdk
```

### Basic Usage

```csharp
using System;
using System.Threading.Tasks;
using Lara.Sdk;

class Program
{
    static async Task Main(string[] args)
    {
        // Set your credentials using environment variables (recommended)
        var credentials = new Credentials(
            Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_ID"),
            Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_SECRET")
        );

        // Create translator instance
        var lara = new Translator(credentials);

        // Simple text translation
        try
        {
            var result = await lara.Translate("Hello, world!", "en-US", "fr-FR");
            Console.WriteLine($"Translation: {result.Translation}");
            // Output: Translation: Bonjour, le monde !
        }
        catch (LaraException e)
        {
            Console.WriteLine($"Translation error: {e.Message}");
        }
    }
}
```

## 📖 Examples

The `examples/` directory contains comprehensive examples for all SDK features.

**All examples use environment variables for credentials, so set them first:**
```bash
export LARA_ACCESS_KEY_ID="your-access-key-id"
export LARA_ACCESS_KEY_SECRET="your-access-key-secret"
```

### Text Translation
- **[TextTranslation.cs](examples/TextTranslation.cs)** - Complete text translation examples
  - Single string translation
  - Multiple strings translation  
  - Translation with instructions
  - TextBlocks translation (mixed translatable/non-translatable content)
  - Auto-detect source language
  - Advanced translation options
  - Get available languages

```bash
cd examples
dotnet run -- text-translation
```

### Document Translation
- **[DocumentTranslation.cs](examples/DocumentTranslation.cs)** - Document translation examples
  - Basic document translation
  - Advanced options with memories and glossaries
  - Step-by-step translation with status monitoring

```bash
cd examples
dotnet run -- document-translation
```

### Translation Memory Management
- **[MemoriesManagement.cs](examples/MemoriesManagement.cs)** - Memory management examples
  - Create, list, update, delete memories
  - Add individual translations
  - Multiple memory operations
  - TMX file import with progress monitoring
  - Translation deletion
  - Translation with TUID and context

```bash
cd examples
dotnet run -- memories-management
```

### Glossary Management
- **[GlossariesManagement.cs](examples/GlossariesManagement.cs)** - Glossary management examples
  - Create, list, update, delete glossaries
  - CSV import with status monitoring
  - Glossary export
  - Glossary terms count
  - Import status checking

```bash
cd examples
dotnet run -- glossaries-management
```

## 🔧 API Reference

### Core Components

### 🔐 Authentication

The SDK supports authentication via access key and secret:

```csharp
var credentials = new Credentials("your-access-key-id", "your-access-key-secret");
var lara = new Translator(credentials);
```

**Environment Variables (Recommended):**
```bash
export LARA_ACCESS_KEY_ID="your-access-key-id"
export LARA_ACCESS_KEY_SECRET="your-access-key-secret"
```

```csharp
var credentials = new Credentials(
    Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_ID"),
    Environment.GetEnvironmentVariable("LARA_ACCESS_KEY_SECRET")
);
```

### 🌍 Translator

```csharp
// Create translator with credentials
var lara = new Translator(credentials);
```

#### Text Translation

```csharp
// Basic translation
var result = await lara.Translate("Hello", "en-US", "fr-FR");

// Multiple strings
var result = await lara.Translate(new[] {"Hello", "World"}, "en-US", "fr-FR");

// TextBlocks (mixed translatable/non-translatable content)
var textBlocks = new List<TextBlock>
{
    new TextBlock("Translatable text", true),
    new TextBlock("<br>", false),  // Non-translatable HTML
    new TextBlock("More translatable text", true),
};
var result = await lara.Translate(textBlocks, "en-US", "fr-FR");

// With advanced options  
var options = new TranslateOptions
{
    Instructions = new[] {"Formal tone"},
    AdaptTo = new[] {"mem_1A2b3C4d5E6f7G8h9I0jKl"},  // Replace with actual memory IDs
    Glossaries = new[] {"gls_1A2b3C4d5E6f7G8h9I0jKl"},  // Replace with actual glossary IDs
    Style = TranslationStyle.Fluid,
    TimeoutInMillis = 10000
};

var result = await lara.Translate("Hello", "en-US", "fr-FR", options);
```

### 📖 Document Translation
#### Simple document translation

```csharp
var filePath = "/path/to/your/document.txt";  // Replace with actual file path
var fileStream = await lara.Documents.Translate(filePath, "en-US", "fr-FR");

// With options
var options = new DocumentTranslateOptions
{
    AdaptTo = new[] {"mem_1A2b3C4d5E6f7G8h9I0jKl"},  // Replace with actual memory IDs
    Glossaries = new[] {"gls_1A2b3C4d5E6f7G8h9I0jKl"}  // Replace with actual glossary IDs
};

var fileStream = await lara.Documents.Translate(filePath, "en-US", "fr-FR", options);
```
### Document translation with status monitoring
#### Document upload
```csharp
//Optional: upload options
var uploadOptions = new DocumentUploadOptions
{
    AdaptTo = new[] {"mem_1A2b3C4d5E6f7G8h9I0jKl"},  // Replace with actual memory IDs
    Glossaries = new[] {"gls_1A2b3C4d5E6f7G8h9I0jKl"}  // Replace with actual glossary IDs
};

var document = await lara.Documents.Upload(filePath, "en-US", "fr-FR", uploadOptions);
```
#### Document translation status monitoring
```csharp
var status = await lara.Documents.Status(document.Id);
```
#### Download translated document
```csharp
var downloadOptions = new DocumentDownloadOptions();

var fileStream = await lara.Documents.Download(document.Id, downloadOptions);
```

### 🧠 Memory Management

```csharp
// Create memory
var memory = await lara.Memories.Create("MyMemory");

// Create memory with external ID (MyMemory integration)
var memory = await lara.Memories.Create("Memory from MyMemory", "aabb1122");  // Replace with actual external ID

// Important: To update/overwrite a translation unit you must provide a tuid. Calls without a tuid always create a new unit and will not update existing entries.
// Add translation to single memory
var memoryImport = await lara.Memories.AddTranslation("mem_1A2b3C4d5E6f7G8h9I0jKl", "en-US", "fr-FR", "Hello", "Bonjour", "greeting_001");

// Add translation to multiple memories
var memoryIds = new List<string> {"mem_1A2b3C4d5E6f7G8h9I0jKl", "mem_2XyZ9AbC8dEf7GhI6jKlMn"};  // Replace with actual memory IDs
var memoryImport = await lara.Memories.AddTranslation(memoryIds, "en-US", "fr-FR", "Hello", "Bonjour", "greeting_002");

// Add with context
var memoryImport = await lara.Memories.AddTranslation(
    "mem_1A2b3C4d5E6f7G8h9I0jKl", "en-US", "fr-FR", "Hello", "Bonjour", "tuid", 
    "sentenceBefore", "sentenceAfter"
);

// TMX import from file
var tmxFilePath = "/path/to/your/memory.tmx";  // Replace with actual TMX file path
var memoryImport = await lara.Memories.ImportTmx("mem_1A2b3C4d5E6f7G8h9I0jKl", tmxFilePath);

// Delete translation
// Important: if you omit tuid, all entries that match the provided fields will be removed
var deleteJob = await lara.Memories.DeleteTranslation(
    "mem_1A2b3C4d5E6f7G8h9I0jKl", "en-US", "fr-FR", "Hello", "Bonjour", "greeting_001"
);

// Wait for import completion
var completedImport = await lara.Memories.WaitForImport(memoryImport, progressCallback, TimeSpan.FromMinutes(5));
```

### 📚 Glossary Management

```csharp
// Create glossary
var glossary = await lara.Glossaries.Create("MyGlossary");

// Import CSV from file
var csvFilePath = "/path/to/your/glossary.csv";  // Replace with actual CSV file path
var glossaryImport = await lara.Glossaries.ImportCsv("gls_1A2b3C4d5E6f7G8h9I0jKl", csvFilePath);

// Check import status
var importStatus = await lara.Glossaries.GetImportStatus(glossaryImport.Id);

// Wait for import completion
var completedImport = await lara.Glossaries.WaitForImport(glossaryImport, progressCallback, TimeSpan.FromMinutes(5));

// Export glossary
var csvData = await lara.Glossaries.Export("gls_1A2b3C4d5E6f7G8h9I0jKl", "csv/table-uni", "en-US");

// Get glossary terms count
var counts = await lara.Glossaries.Counts("gls_1A2b3C4d5E6f7G8h9I0jKl");
```

### Translation Options

```csharp
public class TranslateOptions
{
    public string[] AdaptTo { get; set; }             // Memory IDs to adapt to
    public string[] Glossaries { get; set; }          // Glossary IDs to use
    public string[] Instructions { get; set; }        // Translation instructions
    public TranslationStyle Style { get; set; }       // Translation style (Fluid, Faithful, Creative)
    public string ContentType { get; set; }           // Content type (text/plain, text/html, etc.)
    public bool? Multiline { get; set; }              // Enable multiline translation
    public int? TimeoutInMillis { get; set; }         // Request timeout in milliseconds
    public string SourceHint { get; set; }            // Hint for source language detection
    public bool? NoTrace { get; set; }                // Disable request tracing
    public bool? Verbose { get; set; }                // Enable verbose response
    public TranslatePriority Priority { get; set; }   // Translation priority
}
```

### Language Codes

The SDK supports full language codes (e.g., `en-US`, `fr-FR`, `es-ES`) as well as simple codes (e.g., `en`, `fr`, `es`):

```csharp
// Full language codes (recommended)
var result = await lara.Translate("Hello", "en-US", "fr-FR");

// Simple language codes
var result = await lara.Translate("Hello", "en", "fr");
```

### 🌐 Supported Languages

The SDK supports all languages available in the Lara API. Use the `Languages()` method to get the current list:

```csharp
var languages = await lara.Languages();
Console.WriteLine($"Supported languages: [{string.Join(", ", languages)}]");
```

## ⚙️ Configuration

### Error Handling

The SDK provides detailed error information:

```csharp
try
{
    var result = await lara.Translate("Hello", "en-US", "fr-FR");
    Console.WriteLine($"Translation: {result.Translation}");
}
catch (LaraException e)
{
    Console.WriteLine($"API Error: {e.Message}");
}
catch (LaraTimeoutException e)
{
    Console.WriteLine($"Timeout Error: {e.Message}");
}
```

## 📋 Requirements

- .NET 9.0 or higher
- Valid Lara API credentials

## 🧪 Testing

Run the examples to test your setup:

```bash
# All examples use environment variables for credentials, so set them first:
export LARA_ACCESS_KEY_ID="your-access-key-id"
export LARA_ACCESS_KEY_SECRET="your-access-key-secret"
```

```bash
# Run example files
cd examples
dotnet run -- text-translation
dotnet run -- document-translation
dotnet run -- memories-management
dotnet run -- glossaries-management
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Happy translating! 🌍✨