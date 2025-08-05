using System;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft
{
    class Program
    {
        static void Main(string[] args)
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
        }
    }
}
