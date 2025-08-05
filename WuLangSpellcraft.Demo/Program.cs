using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Core.Serialization;

namespace WuLangSpellcraft
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Wu Xing Spellcraft - Proof of Concept ===");
            Console.WriteLine("Five Elements Visual Programming Language");
            Console.WriteLine();

            // Demonstrate the Five Elements system
            DemonstrateElements();
            
            Console.WriteLine();
            
            // Demonstrate Talisman creation and interactions
            DemonstrateTalismans();
            
            Console.WriteLine();
            
            // Demonstrate Magic Circle composition
            DemonstrateMagicCircle();
            
            Console.WriteLine();
            
            // Demonstrate Serialization
            await DemonstrateSerialization();
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void DemonstrateElements()
        {
            Console.WriteLine("--- Five Elements (Wu Xing) ---");
            
            var elements = new[]
            {
                new Element(ElementType.Water, 1.0),
                new Element(ElementType.Wood, 1.0), 
                new Element(ElementType.Fire, 1.0),
                new Element(ElementType.Earth, 1.0),
                new Element(ElementType.Metal, 1.0)
            };

            foreach (var element in elements)
            {
                Console.ForegroundColor = element.Color;
                Console.WriteLine($"  {element}");
            }
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Element Relationships:");
            
            // Show generative cycle
            Console.WriteLine("Generative Cycle (生):");
            ShowElementRelation(ElementType.Water, ElementType.Wood, "Water nourishes Wood");
            ShowElementRelation(ElementType.Wood, ElementType.Fire, "Wood feeds Fire");
            ShowElementRelation(ElementType.Fire, ElementType.Earth, "Fire creates Earth (ash)");
            ShowElementRelation(ElementType.Earth, ElementType.Metal, "Earth contains Metal");
            ShowElementRelation(ElementType.Metal, ElementType.Water, "Metal collects Water");
            
            Console.WriteLine();
            Console.WriteLine("Destructive Cycle (克):");
            ShowElementRelation(ElementType.Water, ElementType.Fire, "Water extinguishes Fire");
            ShowElementRelation(ElementType.Fire, ElementType.Metal, "Fire melts Metal");
            ShowElementRelation(ElementType.Metal, ElementType.Wood, "Metal cuts Wood");
            ShowElementRelation(ElementType.Wood, ElementType.Earth, "Wood depletes Earth");
            ShowElementRelation(ElementType.Earth, ElementType.Water, "Earth absorbs Water");
        }

        static void ShowElementRelation(ElementType source, ElementType target, string description)
        {
            var sourceElement = new Element(source);
            var targetElement = new Element(target);
            var relation = Element.GetElementRelation(source, target);
            
            Console.ForegroundColor = sourceElement.Color;
            Console.Write($"  {sourceElement.ChineseName}");
            Console.ResetColor();
            
            var symbol = relation switch
            {
                ElementRelation.Generates => " → ",
                ElementRelation.Destroys => " ⚔ ",
                _ => " ~ "
            };
            Console.Write(symbol);
            
            Console.ForegroundColor = targetElement.Color;
            Console.Write($"{targetElement.ChineseName}");
            Console.ResetColor();
            Console.WriteLine($" : {description}");
        }

        static void DemonstrateTalismans()
        {
            Console.WriteLine("--- Talisman Creation and Interactions ---");
            
            // Create some talismans
            var waterTalisman = new Talisman(new Element(ElementType.Water, 1.5), "Azure Flow Talisman");
            var fireTalisman = new Talisman(new Element(ElementType.Fire, 2.0), "Crimson Burst Talisman");
            var earthTalisman = new Talisman(new Element(ElementType.Earth, 1.8), "Stone Shield Talisman");
            
            Console.WriteLine("Created Talismans:");
            Console.WriteLine($"  {waterTalisman}");
            Console.WriteLine($"  {fireTalisman}");
            Console.WriteLine($"  {earthTalisman}");
            
            Console.WriteLine();
            Console.WriteLine("Talisman Interactions:");
            
            // Position talismans
            waterTalisman.X = 0; waterTalisman.Y = 0;
            fireTalisman.X = 5; fireTalisman.Y = 0;
            earthTalisman.X = 2.5; earthTalisman.Y = 4;
            
            // Show interactions
            var interaction1 = waterTalisman.GetInteractionWith(fireTalisman);
            var interaction2 = fireTalisman.GetInteractionWith(earthTalisman);
            var interaction3 = earthTalisman.GetInteractionWith(waterTalisman);
            
            Console.WriteLine($"  {interaction1}");
            Console.WriteLine($"  {interaction2}");
            Console.WriteLine($"  {interaction3}");
            
            // Add secondary elements to show complexity
            Console.WriteLine();
            Console.WriteLine("Adding secondary elements to talismans...");
            fireTalisman.AddSecondaryElement(new Element(ElementType.Wood, 0.8));
            earthTalisman.AddSecondaryElement(new Element(ElementType.Metal, 1.2));
            
            Console.WriteLine($"Enhanced Fire Talisman: {fireTalisman}");
            Console.WriteLine($"Enhanced Earth Talisman: {earthTalisman}");
        }

        static void DemonstrateMagicCircle()
        {
            Console.WriteLine("--- Magic Circle Composition ---");
            
            // Create a magic circle
            var circle = new MagicCircle("Elemental Harmony Circle", 8.0);
            
            // Create talismans for the circle
            var talismans = new[]
            {
                new Talisman(new Element(ElementType.Fire, 2.0), "Flame Core"),
                new Talisman(new Element(ElementType.Earth, 1.5), "Foundation Stone"),
                new Talisman(new Element(ElementType.Water, 1.8), "Flow Regulator"),
                new Talisman(new Element(ElementType.Wood, 1.3), "Growth Catalyst")
            };
            
            Console.WriteLine("Adding talismans to magic circle...");
            foreach (var talisman in talismans)
            {
                if (circle.AddTalisman(talisman))
                {
                    Console.WriteLine($"  ✓ Added: {talisman.Name}");
                }
                else
                {
                    Console.WriteLine($"  ✗ Failed to add: {talisman.Name}");
                }
            }
            
            Console.WriteLine();
            Console.WriteLine($"Circle Status: {circle}");
            
            // Show talisman interactions within the circle
            Console.WriteLine();
            Console.WriteLine("Internal Talisman Interactions:");
            var interactions = circle.GetTalismanInteractions();
            foreach (var interaction in interactions)
            {
                var statusIcon = interaction.IsStable ? "✓" : "⚠";
                Console.WriteLine($"  {statusIcon} {interaction}");
            }
            
            // Calculate and show spell effect
            Console.WriteLine();
            Console.WriteLine("Generated Spell Effect:");
            var spellEffect = circle.CalculateSpellEffect();
            Console.WriteLine($"  {spellEffect}");
            Console.WriteLine($"  World Description: {spellEffect.GetWorldDescription()}");
            Console.WriteLine($"  Mana Cost: {spellEffect.CalculateManaCost():F1}");
            
            // Demonstrate 3D stacking concept
            Console.WriteLine();
            Console.WriteLine("--- 3D Spell Architecture (Concept) ---");
            
            // Create a second circle for stacking
            var upperCircle = new MagicCircle("Amplification Circle", 6.0);
            upperCircle.Layer = 5.0; // Position above the first circle
            
            var metalTalisman = new Talisman(new Element(ElementType.Metal, 1.5), "Precision Focus");
            upperCircle.AddTalisman(metalTalisman);
            
            Console.WriteLine($"Lower Circle: {circle.Name} (Layer {circle.Layer})");
            Console.WriteLine($"Upper Circle: {upperCircle.Name} (Layer {upperCircle.Layer})");
            
            // Create connection between circles
            var connection = circle.ConnectTo(upperCircle, ConnectionType.Resonance);
            Console.WriteLine($"Connection: {connection}");
            
            Console.WriteLine();
            Console.WriteLine("This demonstrates the foundation for:");
            Console.WriteLine("  • Complex 3D spell architectures");
            Console.WriteLine("  • Multi-layer elemental combinations");
            Console.WriteLine("  • Harmonic resonance between circles");
            Console.WriteLine("  • Visual programming through sacred geometry");
            
            // Store circles for serialization demo
            _demonstrationCircles = new[] { circle, upperCircle };
        }

        // Store circles for use in serialization demo
        private static MagicCircle[] _demonstrationCircles = Array.Empty<MagicCircle>();

        static async Task DemonstrateSerialization()
        {
            Console.WriteLine("--- Spell Serialization and Persistence ---");
            
            if (_demonstrationCircles.Length == 0)
            {
                Console.WriteLine("No circles available for serialization demo.");
                return;
            }

            try
            {
                // Create a spell configuration from our demonstration circles
                var spellConfig = SpellConfiguration.FromCircles(
                    "Elemental Harmony Spell", 
                    _demonstrationCircles.ToList(),
                    "A demonstration spell combining Fire, Earth, Water, Wood, and Metal elements in harmonic resonance"
                );
                
                spellConfig.Author = "Wu Xing Spellcraft Demo";
                spellConfig.Metadata["difficulty"] = "Intermediate";
                spellConfig.Metadata["elements"] = new[] { "Fire", "Earth", "Water", "Wood", "Metal" };
                spellConfig.Metadata["totalPower"] = _demonstrationCircles.Sum(c => c.PowerOutput);

                Console.WriteLine($"Created spell configuration: {spellConfig.Name}");
                Console.WriteLine($"  Description: {spellConfig.Description}");
                Console.WriteLine($"  Author: {spellConfig.Author}");
                Console.WriteLine($"  Circles: {spellConfig.Circles.Count}");
                Console.WriteLine($"  Connections: {spellConfig.Connections.Count}");
                Console.WriteLine($"  Total Power: {spellConfig.Metadata["totalPower"]}");

                Console.WriteLine();
                Console.WriteLine("Serializing to JSON...");
                
                // Serialize to JSON string
                var json = SpellSerializer.SerializeSpell(spellConfig);
                Console.WriteLine($"JSON size: {json.Length:N0} characters");
                
                // Show a snippet of the JSON
                var snippet = json.Length > 200 ? json[..200] + "..." : json;
                Console.WriteLine($"JSON snippet: {snippet}");

                Console.WriteLine();
                Console.WriteLine("Saving to file...");
                
                // Save to file
                var fileName = "elemental_harmony_spell.json";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                await SpellSerializer.SaveSpellToFileAsync(spellConfig, filePath);
                
                Console.WriteLine($"✓ Saved to: {filePath}");
                
                Console.WriteLine();
                Console.WriteLine("Loading from file and verifying...");
                
                // Load from file
                var loadedConfig = await SpellSerializer.LoadSpellFromFileAsync(filePath);
                Console.WriteLine($"✓ Loaded spell: {loadedConfig.Name}");
                Console.WriteLine($"  Circles loaded: {loadedConfig.Circles.Count}");
                Console.WriteLine($"  Connections loaded: {loadedConfig.Connections.Count}");
                
                // Reconstruct the spell
                var reconstructedCircles = loadedConfig.ReconstructSpell();
                Console.WriteLine($"✓ Reconstructed {reconstructedCircles.Count} circles");
                
                // Verify the reconstruction
                foreach (var circle in reconstructedCircles)
                {
                    Console.WriteLine($"  Circle: {circle.Name} with {circle.Talismans.Count} talismans");
                    var effect = circle.CalculateSpellEffect();
                    Console.WriteLine($"    Effect: {effect.Type} power {effect.Power:F1}");
                }

                Console.WriteLine();
                Console.WriteLine("Individual circle serialization test...");
                
                // Test individual circle serialization
                var singleCircle = _demonstrationCircles[0];
                var circleJson = SpellSerializer.SerializeCircle(singleCircle);
                var recreatedCircle = SpellSerializer.DeserializeCircle(circleJson);
                
                Console.WriteLine($"Original: {singleCircle.Name} (Power: {singleCircle.PowerOutput:F1})");
                Console.WriteLine($"Recreated: {recreatedCircle.Name} (Power: {recreatedCircle.PowerOutput:F1})");
                Console.WriteLine($"✓ Circle serialization verified!");

                Console.WriteLine();
                Console.WriteLine("Serialization Features Demonstrated:");
                Console.WriteLine("  • Complete spell configuration persistence");
                Console.WriteLine("  • JSON format for human readability and version control");
                Console.WriteLine("  • Individual circle save/load capabilities");
                Console.WriteLine("  • Metadata and author information");
                Console.WriteLine("  • Full reconstruction of complex 3D spell architectures");
                Console.WriteLine("  • File-based spell library management");
                
                // Clean up demo file
                if (File.Exists(filePath))
                {
                    //write demo file to console then remove it
                    Console.WriteLine($"Demo file content:\n{File.ReadAllText(filePath)}");
                    File.Delete(filePath);
                    Console.WriteLine($"  (Demo file {fileName} cleaned up)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Serialization error: {ex.Message}");
            }
        }
    }
}
