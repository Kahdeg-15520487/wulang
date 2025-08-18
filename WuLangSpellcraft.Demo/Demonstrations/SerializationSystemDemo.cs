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
    }
}
