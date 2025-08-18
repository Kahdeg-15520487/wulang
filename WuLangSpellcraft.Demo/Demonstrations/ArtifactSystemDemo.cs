using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates artifact creation, types, and interaction systems
    /// </summary>
    public static class ArtifactSystemDemo
    {
        public static void Run()
        {
            Console.WriteLine("🏺 ARTIFACT SYSTEM DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("🔨 Creating Different Types of Artifacts:");
            Console.WriteLine();
            
            ShowElementalArtifacts();
            Console.WriteLine();
            ShowSpellArtifact();
            Console.WriteLine();
            ShowArtifactInteractions();
        }

        private static void ShowElementalArtifacts()
        {
            var fireArtifact = new ElementalArtifact(ArtifactType.CrucibleOfPower, ElementType.Forge, ElementType.Fire, "Master's Crucible");
            var waterArtifact = new ElementalArtifact(ArtifactType.ChaliceOfFlow, ElementType.Forge, ElementType.Water, "Eternal Chalice");
            var earthArtifact = new ElementalArtifact(ArtifactType.FoundationStone, ElementType.Forge, ElementType.Earth, "Cornerstone of Stability");
            
            Console.WriteLine("🔥 Elemental Artifacts:");
            DisplayArtifact("Fire Crucible", fireArtifact);
            DisplayArtifact("Water Chalice", waterArtifact);
            DisplayArtifact("Earth Foundation", earthArtifact);
        }

        private static void ShowSpellArtifact()
        {
            // Create a circle for spell artifact
            var circle = new MagicCircle("Spell Source Circle", 3.0);
            circle.AddTalisman(new Talisman(new Element(ElementType.Lightning, 0.8), "Storm Core"));
            circle.AddTalisman(new Talisman(new Element(ElementType.Light, 0.7), "Illumination Focus"));
            
            var spellArtifact = new SpellArtifact("Lightning Bolt Wand", circle);
            
            Console.WriteLine("⚡ Spell-Imbued Artifact:");
            DisplayArtifact("Lightning Wand", spellArtifact);
        }

        private static void ShowArtifactInteractions()
        {
            var fireArtifact = new ElementalArtifact(ArtifactType.CrucibleOfPower, ElementType.Forge, ElementType.Fire, "Fire Artifact");
            var waterArtifact = new ElementalArtifact(ArtifactType.ChaliceOfFlow, ElementType.Forge, ElementType.Water, "Water Artifact");
            
            // Create a simple spell artifact for interaction testing
            var circle = new MagicCircle("Test Circle", 2.0);
            circle.AddTalisman(new Talisman(new Element(ElementType.Lightning, 0.8), "Test Core"));
            var spellArtifact = new SpellArtifact("Test Wand", circle);
            
            Console.WriteLine("⚡ Artifact Energy Interactions:");
            DemonstrateArtifactInteraction(fireArtifact, waterArtifact);
            DemonstrateArtifactInteraction(fireArtifact, spellArtifact);
        }

        public static void DisplayArtifact(string displayName, Artifact artifact)
        {
            Console.WriteLine($"  {displayName}:");
            Console.WriteLine($"    Type: {artifact.Type}");
            Console.WriteLine($"    Rarity: {artifact.Rarity}");
            Console.WriteLine($"    Power Level: {artifact.PowerLevel:F2}");
            Console.WriteLine($"    Durability: {artifact.Durability:F2}");
            Console.WriteLine($"    Stability: {artifact.Stability:F2}");
            Console.WriteLine($"    Primary Element: {artifact.PrimaryElement}");
            
            if (artifact.SecondaryElements.Any())
            {
                Console.WriteLine($"    Secondary Elements: {string.Join(", ", artifact.SecondaryElements)}");
            }
            
            Console.WriteLine($"    Energy: {artifact.CurrentEnergy:F2}/{artifact.EnergyCapacity:F2}");
            Console.WriteLine($"    Uses: {artifact.CurrentUses}/{artifact.MaxUses}");
            Console.WriteLine();
        }

        private static void DemonstrateArtifactInteraction(Artifact artifact1, Artifact artifact2)
        {
            Console.WriteLine($"  Interaction: {artifact1.Name} ↔ {artifact2.Name}");
            
            // Calculate basic elemental compatibility
            var element1 = artifact1.PrimaryElement;
            var element2 = artifact2.PrimaryElement;
            var relation = Element.GetElementRelation(element1, element2);
            
            Console.WriteLine($"    Element Relation: {element1} → {element2} ({relation})");
            
            var compatibilityScore = relation switch
            {
                ElementRelation.Generates => 0.8,
                ElementRelation.Destroys => 0.2,
                _ => 0.5
            };
            
            Console.ForegroundColor = compatibilityScore switch
            {
                > 0.6 => ConsoleColor.Green,
                > 0.4 => ConsoleColor.Yellow,
                _ => ConsoleColor.Red
            };
            
            Console.WriteLine($"    Compatibility: {compatibilityScore:F2} ({GetCompatibilityDescription(compatibilityScore)})");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static string GetCompatibilityDescription(double score)
        {
            return score switch
            {
                > 0.6 => "Excellent synergy",
                > 0.4 => "Moderate compatibility", 
                _ => "Poor interaction"
            };
        }
    }
}
