using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates stability-based casting with different outcomes and risk levels
    /// </summary>
    public static class StabilityCastingDemo
    {
        public static void Run()
        {
            Console.WriteLine("⚖️ STABILITY-BASED CASTING DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("🎯 Testing Casting Outcomes Based on Stability:");
            Console.WriteLine();
            
            ShowHighStabilityCasting();
            Console.WriteLine();
            ShowLowStabilityCasting();
            Console.WriteLine();
            ShowStabilityLevels();
        }

        private static void ShowHighStabilityCasting()
        {
            var stableTalisman = new Talisman(new Element(ElementType.Earth, 0.9), "Stable Earth Talisman");
            
            Console.WriteLine("🔥 High Stability Casting:");
            DemonstrateCasting(stableTalisman, 0.3);
        }

        private static void ShowLowStabilityCasting()
        {
            var unstableTalisman = new Talisman(new Element(ElementType.Chaos, 0.9), "Chaotic Talisman");
            unstableTalisman.AddSecondaryElement(new Element(ElementType.Fire, 0.8));
            unstableTalisman.AddSecondaryElement(new Element(ElementType.Water, 0.7));
            unstableTalisman.AddSecondaryElement(new Element(ElementType.Metal, 0.6));
            
            Console.WriteLine("💥 Low Stability Casting:");
            DemonstrateCasting(unstableTalisman, 0.9);
        }

        public static void DemonstrateCasting(Talisman talisman, double targetEnergy)
        {
            Console.WriteLine($"Casting with {talisman.Name}:");
            Console.WriteLine($"  Talisman Stability: {talisman.Stability:F2}");
            Console.WriteLine($"  Target Energy: {targetEnergy:F2}");
            
            var result = talisman.AttemptCasting(targetEnergy);
            
            Console.ForegroundColor = GetOutcomeColor(result.Outcome);
            Console.WriteLine($"  Result: {result.Outcome}");
            Console.ResetColor();
            Console.WriteLine($"  Power Multiplier: {result.PowerMultiplier:F2}x");
            Console.WriteLine($"  Energy Consumed: {result.EnergyConsumed:F2}");
            Console.WriteLine($"  Stability Damage: {result.StabilityDamage:F2}");
            
            if (result.SecondaryEffects.Any())
            {
                Console.WriteLine("  Secondary Effects:");
                foreach (var effect in result.SecondaryEffects)
                {
                    Console.WriteLine($"    • {effect}");
                }
            }
            
            if (!string.IsNullOrEmpty(result.Message))
            {
                Console.WriteLine($"  Message: {result.Message}");
            }
            Console.WriteLine();
        }

        private static ConsoleColor GetOutcomeColor(CastingOutcome outcome)
        {
            return outcome switch
            {
                CastingOutcome.Success => ConsoleColor.Green,
                CastingOutcome.EnhancedSuccess => ConsoleColor.Cyan,
                CastingOutcome.Fizzle => ConsoleColor.Yellow,
                CastingOutcome.Backfire => ConsoleColor.Magenta,
                CastingOutcome.ElementInversion => ConsoleColor.Blue,
                CastingOutcome.CatastrophicFailure => ConsoleColor.Red,
                CastingOutcome.TalismanDestruction => ConsoleColor.DarkRed,
                _ => ConsoleColor.Gray
            };
        }

        private static void ShowStabilityLevels()
        {
            Console.WriteLine("📊 Stability Level Breakdown:");
            
            var levels = new[]
            {
                (StabilityLevel.PerfectStability, "0.9 - 1.0", "Perfect control, enhanced effects"),
                (StabilityLevel.HighStability, "0.7 - 0.89", "Reliable casting, minor risks"),
                (StabilityLevel.ModerateStability, "0.5 - 0.69", "Some variability, moderate risks"),
                (StabilityLevel.LowStability, "0.3 - 0.49", "Unpredictable, significant risks"),
                (StabilityLevel.CriticalInstability, "0.1 - 0.29", "Dangerous, high failure rate"),
                (StabilityLevel.CompleteInstability, "0.0 - 0.09", "Catastrophic failures likely")
            };
            
            foreach (var (level, range, description) in levels)
            {
                Console.WriteLine($"  {level}: {range} - {description}");
            }
        }
    }
}
