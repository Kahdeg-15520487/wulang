using System;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Interactive
{
    public static class StabilityTestingLab
    {
        public static void Run()
        {
            Console.WriteLine("🧪 STABILITY TESTING LABORATORY");
            Console.WriteLine(new string('═', 35));
            Console.WriteLine();
            Console.WriteLine("Welcome to the Stability Testing Lab!");
            Console.WriteLine("Here you can experiment with different elemental combinations");
            Console.WriteLine("and observe how they affect casting stability.");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("🔬 Stability Testing Options:");
                Console.WriteLine("  1. Test Single Element Stability");
                Console.WriteLine("  2. Test Multi-Element Combinations");
                Console.WriteLine("  3. Compare Elemental Balance Effects");
                Console.WriteLine("  4. Casting Outcome Simulation");
                Console.WriteLine("  5. Return to Workshop");
                Console.WriteLine();
                Console.Write("Choose an option (1-5): ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        TestSingleElementStability();
                        break;
                    case "2":
                        TestMultiElementCombinations();
                        break;
                    case "3":
                        CompareElementalBalance();
                        break;
                    case "4":
                        SimulateCastingOutcomes();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice. Please enter 1-5.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.WriteLine();
            }
        }

        private static void TestSingleElementStability()
        {
            Console.WriteLine("🔥 Single Element Stability Test");
            Console.WriteLine("Testing how element intensity affects stability...");
            Console.WriteLine();

            var elementTypes = new[] { ElementType.Fire, ElementType.Water, ElementType.Earth, ElementType.Metal, ElementType.Wood };

            foreach (var elementType in elementTypes)
            {
                Console.WriteLine($"--- {elementType} Element Tests ---");
                
                for (double intensity = 0.5; intensity <= 3.0; intensity += 0.5)
                {
                    var element = new Element(elementType, intensity);
                    var talisman = new Talisman(element);
                    var stabilityLevel = talisman.GetStabilityLevel();
                    
                    var stabilityColor = GetStabilityLevelColor(stabilityLevel);
                    Console.ForegroundColor = stabilityColor;
                    Console.WriteLine($"  Intensity {intensity:F1}: {stabilityLevel}");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        private static void TestMultiElementCombinations()
        {
            Console.WriteLine("🌀 Multi-Element Combination Test");
            Console.WriteLine("Testing stability of different elemental combinations...");
            Console.WriteLine();

            // Test complementary combinations
            Console.WriteLine("--- Complementary Combinations ---");
            TestCombination("Fire + Water", ElementType.Fire, ElementType.Water);
            TestCombination("Earth + Metal", ElementType.Earth, ElementType.Metal);
            TestCombination("Wood + Fire", ElementType.Wood, ElementType.Fire);
            Console.WriteLine();

            // Test conflicting combinations
            Console.WriteLine("--- Conflicting Combinations ---");
            TestCombination("Fire + Metal", ElementType.Fire, ElementType.Metal);
            TestCombination("Water + Earth", ElementType.Water, ElementType.Earth);
            TestCombination("Wood + Metal", ElementType.Wood, ElementType.Metal);
            Console.WriteLine();

            // Test balanced combinations
            Console.WriteLine("--- Balanced Combinations ---");
            var balancedTalisman = new Talisman(new Element(ElementType.Fire, 1.0));
            balancedTalisman.AddSecondaryElement(new Element(ElementType.Water, 1.0));
            balancedTalisman.AddSecondaryElement(new Element(ElementType.Earth, 1.0));
            
            Console.WriteLine($"Three Element Balance: {balancedTalisman.GetStabilityLevel()}");
        }

        private static void TestCombination(string name, ElementType type1, ElementType type2)
        {
            var talisman = new Talisman(new Element(type1, 1.5));
            talisman.AddSecondaryElement(new Element(type2, 1.5));
            
            var stabilityLevel = talisman.GetStabilityLevel();
            var stabilityColor = GetStabilityLevelColor(stabilityLevel);
            
            Console.ForegroundColor = stabilityColor;
            Console.WriteLine($"  {name}: {stabilityLevel}");
            Console.ResetColor();
        }

        private static void CompareElementalBalance()
        {
            Console.WriteLine("⚖️ Elemental Balance Comparison");
            Console.WriteLine("Comparing different balance strategies...");
            Console.WriteLine();

            // Unbalanced (single element)
            var unbalanced = new Talisman(new Element(ElementType.Fire, 3.0));
            
            // Moderately balanced (two elements)
            var moderate = new Talisman(new Element(ElementType.Fire, 1.5));
            moderate.AddSecondaryElement(new Element(ElementType.Water, 1.5));
            
            // Well balanced (three elements)
            var wellBalanced = new Talisman(new Element(ElementType.Fire, 1.0));
            wellBalanced.AddSecondaryElement(new Element(ElementType.Water, 1.0));
            wellBalanced.AddSecondaryElement(new Element(ElementType.Earth, 1.0));
            
            // Perfectly balanced (four elements - max secondary elements is 3)
            var perfect = new Talisman(new Element(ElementType.Fire, 1.0));
            perfect.AddSecondaryElement(new Element(ElementType.Water, 1.0));
            perfect.AddSecondaryElement(new Element(ElementType.Earth, 1.0));
            perfect.AddSecondaryElement(new Element(ElementType.Metal, 1.0));

            Console.WriteLine("Balance Strategy Comparison:");
            ShowBalanceResult("Unbalanced (1 element)", unbalanced);
            ShowBalanceResult("Moderate (2 elements)", moderate);
            ShowBalanceResult("Well Balanced (3 elements)", wellBalanced);
            ShowBalanceResult("Perfect Balance (4 elements)", perfect);
        }

        private static void ShowBalanceResult(string strategy, Talisman talisman)
        {
            var stabilityLevel = talisman.GetStabilityLevel();
            var power = talisman.PowerLevel;
            var stabilityColor = GetStabilityLevelColor(stabilityLevel);
            
            Console.Write($"  {strategy}: ");
            Console.ForegroundColor = stabilityColor;
            Console.Write($"{stabilityLevel}");
            Console.ResetColor();
            Console.WriteLine($", Power {power:F1}");
        }

        private static void SimulateCastingOutcomes()
        {
            Console.WriteLine("🎯 Casting Outcome Simulation");
            Console.WriteLine("Simulating casting attempts with different stability levels...");
            Console.WriteLine();

            var testTalisman = new Talisman(new Element(ElementType.Fire, 1.5));
            testTalisman.AddSecondaryElement(new Element(ElementType.Water, 1.2));

            Console.WriteLine($"Test Talisman: {testTalisman.Name}");
            Console.WriteLine($"Base Stability: {testTalisman.GetStabilityLevel()}");
            Console.WriteLine($"Power: {testTalisman.PowerLevel:F1}");
            Console.WriteLine();

            Console.WriteLine("Casting Attempts at Different Energy Levels:");
            for (double energy = 0.5; energy <= 3.0; energy += 0.5)
            {
                // Create a fresh talisman for each test
                var freshTalisman = new Talisman(new Element(ElementType.Fire, 1.5));
                freshTalisman.AddSecondaryElement(new Element(ElementType.Water, 1.2));
                
                var result = freshTalisman.AttemptCasting(energy);
                var outcomeColor = GetOutcomeColor(result.Outcome);
                
                Console.Write($"  Energy {energy:F1}: ");
                Console.ForegroundColor = outcomeColor;
                Console.WriteLine($"{result.Outcome}");
                Console.ResetColor();
            }
        }

        private static ConsoleColor GetStabilityLevelColor(StabilityLevel level)
        {
            return level switch
            {
                StabilityLevel.PerfectStability => ConsoleColor.Green,
                StabilityLevel.HighStability => ConsoleColor.Yellow,
                StabilityLevel.ModerateStability => ConsoleColor.DarkYellow,
                StabilityLevel.LowStability => ConsoleColor.Red,
                StabilityLevel.CriticalInstability => ConsoleColor.DarkRed,
                StabilityLevel.CompleteInstability => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
        }

        private static ConsoleColor GetOutcomeColor(CastingOutcome outcome)
        {
            return outcome switch
            {
                CastingOutcome.EnhancedSuccess => ConsoleColor.Magenta,
                CastingOutcome.Success => ConsoleColor.Green,
                CastingOutcome.Fizzle => ConsoleColor.Yellow,
                CastingOutcome.Backfire => ConsoleColor.Red,
                CastingOutcome.ElementInversion => ConsoleColor.DarkRed,
                CastingOutcome.CatastrophicFailure => ConsoleColor.DarkRed,
                CastingOutcome.TalismanDestruction => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
        }
    }
}
