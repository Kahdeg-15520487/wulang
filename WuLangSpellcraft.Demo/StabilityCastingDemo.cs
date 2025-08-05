using System;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo
{
    /// <summary>
    /// Demonstrates the new stability-based spell casting mechanics
    /// </summary>
    public static class StabilityCastingDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Wu Lang Spellcraft - Stability Casting Demo ===\n");
            
            // Create talismans with different stability levels
            var talismans = CreateTestTalismans();
            
            foreach (var talisman in talismans)
            {
                Console.WriteLine($"Testing: {talisman}");
                Console.WriteLine($"Stability Level: {talisman.GetStabilityLevel()}");
                Console.WriteLine($"Description: {talisman.GetStabilityDescription()}\n");
                
                // Attempt multiple spell casts to show variation
                Console.WriteLine("Casting Results:");
                for (int i = 0; i < 3; i++)
                {
                    var result = talisman.AttemptCasting(10.0); // 10 energy cost spell
                    
                    Console.WriteLine($"  Cast {i + 1}: {result.Outcome} " +
                                    $"(Power: {result.PowerMultiplier:F2}x, Energy: {result.EnergyConsumed:F1})");
                    
                    if (!string.IsNullOrEmpty(result.Message))
                        Console.WriteLine($"    {result.Message}");
                    
                    if (result.SecondaryEffects.Count > 0)
                    {
                        Console.WriteLine($"    Effects: {string.Join(", ", result.SecondaryEffects)}");
                    }
                    
                    if (result.TalismanDestroyed)
                    {
                        Console.WriteLine("    ⚠️ TALISMAN DESTROYED! No further casting possible.");
                        break;
                    }
                }
                
                Console.WriteLine($"Final Stability: {talisman.Stability:F3}\n");
                Console.WriteLine(new string('-', 60));
            }
        }
        
        private static Talisman[] CreateTestTalismans()
        {
            // Create talismans with varying stability levels for testing
            
            // Perfect Stability - Pure Fire talisman
            var perfectTalisman = new Talisman(new Element(ElementType.Fire), "Perfect Fire Talisman");
            
            // High Stability - Fire with compatible Wood secondary
            var highTalisman = new Talisman(new Element(ElementType.Wood), "High Stability Wood Talisman");
            highTalisman.AddSecondaryElement(new Element(ElementType.Fire)); // Wood generates Fire
            
            // Moderate Stability - Mixed elements with some conflict
            var moderateTalisman = new Talisman(new Element(ElementType.Water), "Moderate Water Talisman");
            moderateTalisman.AddSecondaryElement(new Element(ElementType.Wood)); // Water generates Wood (good)
            moderateTalisman.AddSecondaryElement(new Element(ElementType.Earth)); // Earth generates Water (neutral to good)
            
            // Low Stability - Conflicting elements
            var lowTalisman = new Talisman(new Element(ElementType.Fire), "Low Stability Fire Talisman");
            lowTalisman.AddSecondaryElement(new Element(ElementType.Water)); // Water destroys Fire (bad)
            lowTalisman.AddSecondaryElement(new Element(ElementType.Metal)); // Metal and Fire conflict
            
            // Critical Instability - Highly conflicting elements
            var criticalTalisman = new Talisman(new Element(ElementType.Wood), "Critical Wood Talisman");
            criticalTalisman.AddSecondaryElement(new Element(ElementType.Metal)); // Metal destroys Wood (very bad)
            criticalTalisman.AddSecondaryElement(new Element(ElementType.Fire)); // Fire destroys Wood (very bad)
            criticalTalisman.AddSecondaryElement(new Element(ElementType.Earth)); // Creates more conflicts
            
            // Apply some stability damage to the critical one to push it lower
            var criticalResult = criticalTalisman.AttemptCasting(1.0);
            
            return new[] { perfectTalisman, highTalisman, moderateTalisman, lowTalisman, criticalTalisman };
        }
    }
}
