using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Serialization;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    public static class JsonFileParserDemo
    {
        public static void Run()
        {
            Console.WriteLine("📄 JSON FILE PARSER DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("🔍 JSON File Parser Features:");
            Console.WriteLine("  • Load spell configurations from JSON files");
            Console.WriteLine("  • Parse magic circles and spell effects");
            Console.WriteLine("  • Validate JSON structure and content");
            Console.WriteLine("  • Display parsed results with visualization");
            Console.WriteLine();
            
            ShowJsonParsingExamples();
            ShowInteractiveJsonParser();
        }

        private static void ShowJsonParsingExamples()
        {
            Console.WriteLine("📚 JSON PARSING EXAMPLES");
            Console.WriteLine(new string('─', 40));
            Console.WriteLine();
            
            // Example 1: Simple Circle JSON
            string simpleCircleJson = @"{
  ""id"": ""simple-circle"",
  ""name"": ""Basic Elemental Circle"",
  ""description"": ""A simple three-element circle"",
  ""radius"": 3,
  ""talismans"": [
    {
      ""id"": ""fire-core"",
      ""element"": ""Fire"",
      ""power"": 1.5,
      ""position"": 0
    },
    {
      ""id"": ""water-shield"",
      ""element"": ""Water"",
      ""power"": 1.0,
      ""position"": 120
    },
    {
      ""id"": ""earth-ground"",
      ""element"": ""Earth"",
      ""power"": 0.8,
      ""position"": 240
    }
  ]
}";

            Console.WriteLine("📝 Example 1: Simple Circle JSON");
            Console.WriteLine("```json");
            Console.WriteLine(simpleCircleJson);
            Console.WriteLine("```");
            Console.WriteLine();
            
            try
            {
                var parsedCircle = ParseCircleFromJson(simpleCircleJson);
                Console.WriteLine("✅ Successfully parsed JSON:");
                Console.WriteLine($"   Circle: {parsedCircle.Name}");
                Console.WriteLine($"   Radius: {parsedCircle.Radius}");
                Console.WriteLine($"   Talismans: {parsedCircle.Talismans.Count}");
                
                // Convert to CNF for comparison
                var cnf = SpellSerializer.SerializeCircleToCnf(parsedCircle);
                Console.WriteLine($"   CNF Equivalent: {cnf}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error parsing JSON: {ex.Message}");
                Console.WriteLine();
            }
            
            // Example 2: Complex Spell JSON
            string complexSpellJson = @"{
  ""id"": ""complex-spell"",
  ""name"": ""Wu Xing Formation"",
  ""description"": ""Complete five-element spell formation"",
  ""circles"": [
    {
      ""id"": ""main-circle"",
      ""name"": ""Wu Xing Circle"",
      ""radius"": 5,
      ""talismans"": [
        { ""element"": ""Fire"", ""power"": 2.0 },
        { ""element"": ""Water"", ""power"": 1.8 },
        { ""element"": ""Earth"", ""power"": 1.5 },
        { ""element"": ""Metal"", ""power"": 1.3 },
        { ""element"": ""Wood"", ""power"": 1.6 }
      ]
    }
  ],
  ""effects"": [
    {
      ""name"": ""Elemental Harmony"",
      ""description"": ""Balances all five elements"",
      ""power"": 2.4
    }
  ]
}";

            Console.WriteLine("📝 Example 2: Complex Spell JSON");
            Console.WriteLine("```json");
            Console.WriteLine(complexSpellJson);
            Console.WriteLine("```");
            Console.WriteLine();
            
            try
            {
                var spellData = JsonDocument.Parse(complexSpellJson);
                var root = spellData.RootElement;
                
                Console.WriteLine("✅ Successfully parsed complex spell JSON:");
                Console.WriteLine($"   Spell: {root.GetProperty("name").GetString()}");
                
                if (root.TryGetProperty("circles", out var circlesArray))
                {
                    Console.WriteLine($"   Circles: {circlesArray.GetArrayLength()}");
                    
                    foreach (var circleElement in circlesArray.EnumerateArray())
                    {
                        var circleName = circleElement.GetProperty("name").GetString();
                        var radius = circleElement.GetProperty("radius").GetDouble();
                        var talismans = circleElement.GetProperty("talismans").GetArrayLength();
                        
                        Console.WriteLine($"     • {circleName} (R:{radius}, T:{talismans})");
                    }
                }
                
                if (root.TryGetProperty("effects", out var effectsArray))
                {
                    Console.WriteLine($"   Effects: {effectsArray.GetArrayLength()}");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error parsing complex JSON: {ex.Message}");
                Console.WriteLine();
            }
        }

        private static void ShowInteractiveJsonParser()
        {
            Console.WriteLine("🎯 INTERACTIVE JSON PARSER");
            Console.WriteLine(new string('─', 40));
            Console.WriteLine();
            Console.WriteLine("Enter JSON content to parse (or 'quit' to exit):");
            Console.WriteLine("You can paste JSON for magic circles or spell configurations.");
            Console.WriteLine();
            
            while (true)
            {
                Console.Write("JSON> ");
                var input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "quit")
                {
                    break;
                }
                
                try
                {
                    // Try to parse as generic JSON first
                    var jsonDoc = JsonDocument.Parse(input);
                    var root = jsonDoc.RootElement;
                    
                    Console.WriteLine("✅ Valid JSON structure detected!");
                    
                    // Try to identify the type of JSON
                    if (root.TryGetProperty("radius", out _) && root.TryGetProperty("talismans", out _))
                    {
                        Console.WriteLine("🔮 Detected: Magic Circle JSON");
                        var circle = ParseCircleFromJson(input);
                        Console.WriteLine($"   Circle: {circle.Name ?? "Unnamed"}");
                        Console.WriteLine($"   Radius: {circle.Radius}");
                        Console.WriteLine($"   Talismans: {circle.Talismans.Count}");
                        
                        var cnf = SpellSerializer.SerializeCircleToCnf(circle);
                        Console.WriteLine($"   CNF: {cnf}");
                    }
                    else if (root.TryGetProperty("circles", out _))
                    {
                        Console.WriteLine("✨ Detected: Spell Effect JSON");
                        var name = root.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : "Unnamed";
                        Console.WriteLine($"   Spell: {name}");
                        
                        if (root.TryGetProperty("circles", out var circlesArray))
                        {
                            Console.WriteLine($"   Circles: {circlesArray.GetArrayLength()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("📄 Detected: Generic JSON");
                        Console.WriteLine($"   Properties: {root.EnumerateObject().Count()}");
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"❌ Invalid JSON: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                }
                
                Console.WriteLine();
            }
        }

        private static MagicCircle ParseCircleFromJson(string json)
        {
            var jsonDoc = JsonDocument.Parse(json);
            var root = jsonDoc.RootElement;
            
            var id = root.TryGetProperty("id", out var idElement) ? idElement.GetString() : Guid.NewGuid().ToString();
            var name = root.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : "Parsed Circle";
            var radius = root.GetProperty("radius").GetDouble();
            
            var circle = new MagicCircle(name, radius);
            
            if (root.TryGetProperty("talismans", out var talismansArray))
            {
                foreach (var talismanElement in talismansArray.EnumerateArray())
                {
                    var elementName = talismanElement.GetProperty("element").GetString();
                    var power = talismanElement.TryGetProperty("power", out var powerElement) ? powerElement.GetDouble() : 1.0;
                    var talismanId = talismanElement.TryGetProperty("id", out var tIdElement) ? tIdElement.GetString() : null;
                    
                    if (Enum.TryParse<ElementType>(elementName, out var elementType))
                    {
                        var element = new Element(elementType);
                        var talisman = new Talisman(element, talismanId);
                        talisman.PowerLevel = power; // Set power after creation
                        circle.AddTalisman(talisman);
                    }
                }
            }
            
            return circle;
        }
    }
}
