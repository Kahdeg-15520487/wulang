using System;
using System.Linq;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Core.Serialization;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates the serialization system for saving and loading spell configurations
    /// </summary>
    public static class SerializationSystemDemo
    {
        public static void Run()
        {
            Console.WriteLine("💾 SERIALIZATION SYSTEM DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            var circles = GetOrCreateDemonstrationCircles();
            
            if (circles.Length == 0)
            {
                Console.WriteLine("⚠️ No circles available for serialization. Creating demo circles...");
                circles = CreateDemoCircles();
                Console.WriteLine();
            }
            
            Console.WriteLine("📦 Serializing Magic Circles and Spell Configurations:");
            Console.WriteLine();
            
            try
            {
                DemonstrateSpellSerialization(circles);
                Console.WriteLine();
                DemonstrateCnfSerialization(circles);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Serialization error: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static MagicCircle[] GetOrCreateDemonstrationCircles()
        {
            var circles = MagicCircleDemo.GetDemonstrationCircles();
            return circles.Length > 0 ? circles : CreateDemoCircles();
        }

        private static MagicCircle[] CreateDemoCircles()
        {
            var circle1 = new MagicCircle("Demo Circle 1", 4.0);
            circle1.AddTalisman(new Talisman(new Element(ElementType.Fire, 0.8), "Fire Node"));
            circle1.AddTalisman(new Talisman(new Element(ElementType.Water, 0.7), "Water Node"));
            
            var circle2 = new MagicCircle("Demo Circle 2", 3.0);
            circle2.AddTalisman(new Talisman(new Element(ElementType.Lightning, 0.9), "Lightning Node"));
            
            return new[] { circle1, circle2 };
        }

        private static void DemonstrateSpellSerialization(MagicCircle[] circles)
        {
            // Create a spell configuration for serialization
            var spellConfig = new SpellConfiguration("Demonstration Multi-Circle Spell", "A complex spell showcasing circle interactions");
            
            // Add circles to the configuration
            foreach (var circle in circles)
            {
                spellConfig.AddCircle(circle);
            }
            
            spellConfig.Metadata["creator"] = "WuLang Demo System";
            spellConfig.Metadata["version"] = "1.0";
            spellConfig.Metadata["totalPower"] = circles.Sum(c => c.PowerOutput);
            
            Console.WriteLine("🔄 Serializing to JSON...");
            string json = SpellSerializer.SerializeSpell(spellConfig);
            
            Console.WriteLine("✅ Serialization complete!");
            Console.WriteLine($"   JSON size: {json.Length:N0} characters");
            Console.WriteLine();
            
            // Show a sample of the JSON
            var sample = json.Length > 200 ? json.Substring(0, 200) + "..." : json;
            Console.WriteLine("📄 JSON Sample:");
            Console.WriteLine($"   {sample}");
            Console.WriteLine();
            
            Console.WriteLine("🔄 Deserializing back to object...");
            var deserializedConfig = SpellSerializer.DeserializeSpell(json);
            
            Console.WriteLine("✅ Deserialization complete!");
            Console.WriteLine($"   Spell Name: {deserializedConfig.Name}");
            Console.WriteLine($"   Circle Count: {deserializedConfig.Circles.Count}");
            Console.WriteLine($"   Created: {deserializedConfig.Created}");
            Console.WriteLine($"   Author: {deserializedConfig.Author}");
            Console.WriteLine();
            
            ValidateIntegrity(spellConfig, deserializedConfig);
        }

        private static void ValidateIntegrity(SpellConfiguration original, SpellConfiguration deserialized)
        {
            // Validate integrity 
            var originalPower = original.Metadata["totalPower"];
            var deserializedCircles = deserialized.Circles.Select(cd => cd.ToMagicCircle()).ToList();
            var deserializedPower = deserializedCircles.Sum(c => c.PowerOutput);
            
            Console.WriteLine("🔍 Integrity Check:");
            Console.WriteLine($"   Original Power: {originalPower}");
            Console.WriteLine($"   Deserialized Power: {deserializedPower:F2}");
            
            if (Math.Abs((double)originalPower - deserializedPower) < 0.01)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   ✅ Data integrity verified!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Data integrity check failed!");
            }
            Console.ResetColor();
        }

        private static void DemonstrateCnfSerialization(MagicCircle[] circles)
        {
            Console.WriteLine("🌀 CIRCLE NOTATION FORMAT (CNF) DEMONSTRATION");
            Console.WriteLine(new string('─', 50));
            Console.WriteLine();
            
            foreach (var circle in circles)
            {
                Console.WriteLine($"📍 Circle: {circle.Name}");
                Console.WriteLine($"   Radius: {circle.Radius}, Talismans: {circle.Talismans.Count}");
                
                // Test basic CNF serialization
                var basicCnf = SpellSerializer.SerializeCircleToCnf(circle);
                Console.WriteLine($"   Basic CNF: {basicCnf}");
                
                // Test CNF with power levels
                var detailedOptions = new CnfOptions 
                { 
                    IncludePowerLevels = true,
                    IncludeTalismanIds = false 
                };
                var detailedCnf = SpellSerializer.SerializeCircleToCnf(circle, detailedOptions);
                Console.WriteLine($"   With Power: {detailedCnf}");
                
                // Test CNF with IDs
                var idOptions = new CnfOptions 
                { 
                    IncludePowerLevels = true,
                    IncludeTalismanIds = true,
                    UseReadableIds = true
                };
                var idCnf = SpellSerializer.SerializeCircleToCnf(circle, idOptions);
                Console.WriteLine($"   With IDs: {idCnf}");
                
                // Test round-trip conversion
                try
                {
                    var parsedCircle = SpellSerializer.DeserializeCircleFromCnf(basicCnf);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"   ✅ CNF Round-trip: Success (R:{parsedCircle.Radius}, T:{parsedCircle.Talismans.Count})");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"   ❌ CNF Round-trip: {ex.Message}");
                    Console.ResetColor();
                }
                
                Console.WriteLine();
            }
            
            // Demonstrate parsing various CNF formats
            DemonstrateCnfParsing();
        }

        private static void DemonstrateCnfParsing()
        {
            Console.WriteLine("📖 CNF PARSING EXAMPLES");
            Console.WriteLine(new string('─', 30));
            
            var testCases = new[]
            {
                "C3 F W E",                    // Basic circle
                "C5 F2.5 W1.2 L0.8",          // With power levels
                "C4 F:core W:shield E:ground", // With IDs
                "C2.5 F2:flame W1.5:water",   // Combined power and IDs
                "C6 FWEMO",                    // Compact format
                "C1 V",                       // Single void element
                "C10 FWEML",                  // All base elements
                "C0.5 C:chaos D:dark",        // Derived elements
                "C7.5 F3.14:pi W2.71:euler",  // Mathematical constants
                "C12 FWEMOLINDGCV"            // All elements compact
            };
            
            foreach (var cnf in testCases)
            {
                try
                {
                    var circle = SpellSerializer.DeserializeCircleFromCnf(cnf);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"   ✅ '{cnf}' -> R:{circle.Radius}, {circle.Talismans.Count} talismans");
                    
                    // Show details of parsed talismans
                    foreach (var talisman in circle.Talismans)
                    {
                        var symbol = ElementSymbols.GetSymbol(talisman.PrimaryElement.Type);
                        var nameInfo = talisman.Name.Contains("Talisman") ? "" : $" '{talisman.Name}'";
                        Console.WriteLine($"      {symbol} ({talisman.PrimaryElement.Type}): Power {talisman.PrimaryElement.Energy:F2}{nameInfo}");
                    }
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"   ❌ '{cnf}' -> Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("📊 CNF FORMAT SUMMARY:");
            Console.WriteLine("   • C<radius> - Circle with specified radius");
            Console.WriteLine("   • F W E M O - Element symbols (Fire, Water, Earth, Metal, wOod)");
            Console.WriteLine("   • L N I D G - Derived elements (Lightning, wiNd, lIght, Dark, forGe)");
            Console.WriteLine("   • C V - Special elements (Chaos, Void)");
            Console.WriteLine("   • F2.5 - Element with power level");
            Console.WriteLine("   • F:id - Element with named ID");
            Console.WriteLine("   • F2.5:flame - Element with both power and ID");
            Console.WriteLine("   • FWEMO - Compact format (no spaces)");
        }
    }
}
