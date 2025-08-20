using System;
using System.Linq;
using System.Numerics;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Simulation;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates how stability affects physics simulation outcomes.
    /// </summary>
    public static class StabilityPhysicsDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("⚖️ COMPREHENSIVE STABILITY PHYSICS DEMONSTRATION ⚖️");
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine();
            
            Console.WriteLine("This demo demonstrates ALL POSSIBLE casting outcomes:");
            Console.WriteLine("• ✅ Success - Normal spell effect");
            Console.WriteLine("• ✨ Enhanced Success - Amplified power and effects");
            Console.WriteLine("• 💨 Fizzle - Spell fails, partial energy loss");
            Console.WriteLine("• ⚡ Backfire - Spell affects caster negatively");
            Console.WriteLine("• 🔄 Element Inversion - Opposite elemental effect");
            Console.WriteLine("• 💥 Catastrophic Failure - Major area damage");
            Console.WriteLine("• 💀 Talisman Destruction - Permanent talisman loss");
            Console.WriteLine();

            // Demonstrate each outcome type
            DemonstrateAllOutcomes();
            
            Console.WriteLine();
            Console.WriteLine("🎯 Comprehensive Stability Demo Complete!");
            Console.WriteLine();
            Console.WriteLine("🔬 Scientific Observations:");
            Console.WriteLine("• Perfect Stability (1.0): Only Success and Enhanced Success");
            Console.WriteLine("• High Stability (0.7-0.9): Mostly Success, rare Fizzle");
            Console.WriteLine("• Moderate Stability (0.5-0.7): Success, Fizzle, occasional Wild Magic");
            Console.WriteLine("• Low Stability (0.3-0.5): Success, Fizzle, Backfire, Element Inversion");
            Console.WriteLine("• Critical Instability (0.1-0.3): All failure types, including Catastrophic");
            Console.WriteLine("• Complete Instability (0.0-0.1): Guaranteed Talisman Destruction");
            Console.WriteLine();
            Console.WriteLine("⚠️ WARNING: Low stability casting can permanently damage talismans!");
        }

        private static void DemonstrateAllOutcomes()
        {
            Console.WriteLine("🧪 OUTCOME DEMONSTRATION LAB");
            Console.WriteLine("─────────────────────────────");
            Console.WriteLine();

            // Create specific talismans designed to trigger each outcome
            var outcomeTests = CreateOutcomeTestTalismans();
            var origin = new Vector2(0, 0);
            var direction = new Vector2(1, 0);
            double spellEnergyCost = 15.0;

            foreach (var test in outcomeTests)
            {
                Console.WriteLine($"🔬 Testing for: {test.TargetOutcome}");
                Console.WriteLine($"📝 Test Setup: {test.Description}");
                Console.WriteLine($"🔮 Talisman: {test.Talisman.Name}");
                Console.WriteLine($"   Stability: {test.Talisman.Stability:F3} ({test.Talisman.GetStabilityLevel()})");
                Console.WriteLine();

                bool targetAchieved = false;
                int maxAttempts = test.TargetOutcome == CastingOutcome.TalismanDestruction ? 1 : 10;
                
                for (int attempt = 1; attempt <= maxAttempts && !targetAchieved; attempt++)
                {
                    var stabilityResult = ArtifactPhysicsEvaluator.AttemptCastingWithPhysics(
                        test.Talisman, spellEnergyCost, origin, direction, null, test.ControlledRandom);
                    
                    if (stabilityResult.CastingResult.Outcome == test.TargetOutcome)
                    {
                        targetAchieved = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"   ✅ ACHIEVED TARGET OUTCOME on attempt {attempt}!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"   Attempt {attempt}: {stabilityResult.CastingResult.Outcome}");
                    }
                    
                    // Display detailed results for the target outcome
                    if (targetAchieved || attempt == maxAttempts)
                    {
                        DisplayDetailedOutcome(stabilityResult, test.TargetOutcome);
                    }
                    
                    // Stop if talisman is destroyed
                    if (stabilityResult.CastingResult.Outcome == CastingOutcome.TalismanDestruction)
                    {
                        break;
                    }
                    
                    // Add slight delay between attempts for suspense
                    if (attempt < maxAttempts && !targetAchieved)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }

                if (!targetAchieved && test.TargetOutcome != CastingOutcome.TalismanDestruction)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"   ⚠️  Target outcome not achieved in {maxAttempts} attempts");
                    Console.WriteLine($"   💡 Try again - stability outcomes are probabilistic!");
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.WriteLine($"   Final Talisman State: {test.Talisman.Stability:F3} stability");
                if (test.Talisman.Stability < 0.1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   💀 TALISMAN CRITICALLY UNSTABLE - AVOID FURTHER USE!");
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.WriteLine(new string('─', 70));
                Console.WriteLine();
            }
        }

        private static void DisplayDetailedOutcome(StabilityPhysicsResult result, CastingOutcome targetOutcome)
        {
            var outcome = result.CastingResult;
            
            Console.ForegroundColor = GetOutcomeColor(outcome.Outcome);
            Console.WriteLine($"   🎯 Outcome: {outcome.Outcome}");
            Console.ResetColor();
            
            Console.WriteLine($"   ⚡ Power Multiplier: {outcome.PowerMultiplier:F2}x");
            Console.WriteLine($"   🔋 Energy Consumed: {outcome.EnergyConsumed:F1}");
            Console.WriteLine($"   💬 Message: {outcome.Message}");
            
            if (outcome.SecondaryEffects.Any())
            {
                Console.WriteLine("   🌀 Secondary Effects:");
                foreach (var effect in outcome.SecondaryEffects)
                {
                    Console.WriteLine($"     • {effect}");
                }
            }

            if (result.SpellResult != null)
            {
                var physics = EnhancedSpellPhysicsFactory.CreateSpellEffectFromStability(result);
                if (physics != null)
                {
                    Console.WriteLine($"   🌍 Physics Generated: {physics.Tag}");
                    Console.WriteLine($"     Type: {physics.EffectType}");
                    Console.WriteLine($"     Power: {result.SpellResult.Power:F2}");
                    
                    if (physics.EffectType == Simulation.SpellEffectType.Projectile)
                    {
                        Console.WriteLine($"     Velocity: ({physics.Velocity.X:F1}, {physics.Velocity.Y:F1}) m/s");
                    }
                    else if (physics.EffectType == Simulation.SpellEffectType.AreaEffect)
                    {
                        Console.WriteLine($"     Area Radius: {physics.Radius:F1}m");
                        Console.WriteLine($"     Duration: {physics.Duration:F1}s");
                    }
                }
            }
            else
            {
                Console.WriteLine($"   🌍 Physics: None ({result.FailureReason})");
            }
            
            if (outcome.StabilityDamage > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   💔 Stability Damage: -{outcome.StabilityDamage:F3}");
                Console.ResetColor();
            }
        }

        private static OutcomeTest[] CreateOutcomeTestTalismans()
        {
            return new OutcomeTest[]
            {
                // 1. Success - High stability talisman
                new OutcomeTest
                {
                    TargetOutcome = CastingOutcome.Success,
                    Description = "High stability fire talisman with compatible elements",
                    Talisman = CreateHighStabilityTalisman(),
                    ControlledRandom = new Random(1) // Seed for reproducible results
                },
                
                // 2. Enhanced Success - Perfect stability talisman
                new OutcomeTest
                {
                    TargetOutcome = CastingOutcome.EnhancedSuccess,
                    Description = "Perfect stability pure element for maximum enhancement chance",
                    Talisman = CreatePerfectStabilityTalisman(),
                    ControlledRandom = new Random(42) // Seed that tends toward enhanced success
                },
                
                // 3. Fizzle - Moderate stability with conflicts
                new OutcomeTest
                {
                    TargetOutcome = CastingOutcome.Fizzle,
                    Description = "Moderate stability with some elemental conflicts",
                    Talisman = CreateModerateStabilityTalisman(),
                    ControlledRandom = new Random(100) // Seed that tends toward fizzle
                },
                
                // 4. Backfire - Low stability with major conflicts
                new OutcomeTest
                {
                    TargetOutcome = CastingOutcome.Backfire,
                    Description = "Low stability with opposing elements causing energy reversal",
                    Talisman = CreateLowStabilityTalisman(),
                    ControlledRandom = new Random(200) // Seed that tends toward backfire
                },
                
                // 5. Element Inversion - Conflicting elements
                new OutcomeTest
                {
                    TargetOutcome = CastingOutcome.ElementInversion,
                    Description = "Conflicting elements causing elemental opposition",
                    Talisman = CreateConflictedTalisman(),
                    ControlledRandom = new Random(300) // Seed that tends toward inversion
                },
                
                // 6. Catastrophic Failure - Critical instability
                new OutcomeTest
                {
                    TargetOutcome = CastingOutcome.CatastrophicFailure,
                    Description = "Critical instability with chaotic elemental mixture",
                    Talisman = CreateCriticalInstabilityTalisman(),
                    ControlledRandom = new Random(400) // Seed that tends toward catastrophic failure
                },
                
                // 7. Talisman Destruction - Complete instability
                new OutcomeTest
                {
                    TargetOutcome = CastingOutcome.TalismanDestruction,
                    Description = "Complete instability - guaranteed destruction",
                    Talisman = CreateCompleteInstabilityTalisman(),
                    ControlledRandom = new Random(500) // Any seed works for complete instability
                }
            };
        }

        private static Talisman CreateHighStabilityTalisman()
        {
            var talisman = new Talisman(new Element(ElementType.Wood), "Harmonious Grove Focus");
            talisman.AddSecondaryElement(new Element(ElementType.Fire)); // Wood generates Fire (good)
            return talisman;
        }

        private static Talisman CreatePerfectStabilityTalisman()
        {
            return new Talisman(new Element(ElementType.Fire), "Pure Flame Essence");
        }

        private static Talisman CreateModerateStabilityTalisman()
        {
            var talisman = new Talisman(new Element(ElementType.Water), "Turbulent Stream");
            talisman.AddSecondaryElement(new Element(ElementType.Wood)); // Water generates Wood (good)
            talisman.AddSecondaryElement(new Element(ElementType.Earth)); // Earth generates Water (neutral)
            talisman.AddSecondaryElement(new Element(ElementType.Metal)); // Some conflict with water
            return talisman;
        }

        private static Talisman CreateLowStabilityTalisman()
        {
            var talisman = new Talisman(new Element(ElementType.Fire), "Smoldering Ember");
            talisman.AddSecondaryElement(new Element(ElementType.Water)); // Water destroys Fire (very bad)
            talisman.AddSecondaryElement(new Element(ElementType.Metal)); // Metal conflicts with Fire
            return talisman;
        }

        private static Talisman CreateConflictedTalisman()
        {
            var talisman = new Talisman(new Element(ElementType.Fire), "Opposed Flame");
            talisman.AddSecondaryElement(new Element(ElementType.Water)); // Direct opposition
            talisman.AddSecondaryElement(new Element(ElementType.Earth)); // Earth absorbs fire
            return talisman;
        }

        private static Talisman CreateCriticalInstabilityTalisman()
        {
            var talisman = new Talisman(new Element(ElementType.Chaos, 0.6), "Fractured Reality Shard");
            talisman.AddSecondaryElement(new Element(ElementType.Fire, 0.5));
            talisman.AddSecondaryElement(new Element(ElementType.Water, 0.4));
            talisman.AddSecondaryElement(new Element(ElementType.Metal, 0.3));
            talisman.AddSecondaryElement(new Element(ElementType.Void, 0.2));
            return talisman;
        }

        private static Talisman CreateCompleteInstabilityTalisman()
        {
            var talisman = new Talisman(new Element(ElementType.Chaos, 0.1), "Annihilation Core");
            talisman.AddSecondaryElement(new Element(ElementType.Fire, 0.1));
            talisman.AddSecondaryElement(new Element(ElementType.Water, 0.1));
            talisman.AddSecondaryElement(new Element(ElementType.Earth, 0.1));
            talisman.AddSecondaryElement(new Element(ElementType.Metal, 0.1));
            talisman.AddSecondaryElement(new Element(ElementType.Wood, 0.1));
            talisman.AddSecondaryElement(new Element(ElementType.Void, 0.1));
            
            // Manually force to complete instability range with a safety limit
            int attempts = 0;
            const int maxAttempts = 20; // Safety limit to prevent infinite loops
            
            while (talisman.Stability > 0.05 && attempts < maxAttempts)
            {
                var damageResult = talisman.AttemptCasting(1.0);
                attempts++;
            }
            
            return talisman;
        }

        private class OutcomeTest
        {
            public CastingOutcome TargetOutcome { get; set; }
            public string Description { get; set; } = "";
            public Talisman Talisman { get; set; } = null!;
            public Random ControlledRandom { get; set; } = new Random();
        }

        private static Talisman[] CreateTestTalismans()
        {
            // Perfect Stability - Pure Fire talisman
            var perfectTalisman = new Talisman(new Element(ElementType.Fire), "Perfect Fire Focus");
            
            // High Stability - Compatible elements
            var highTalisman = new Talisman(new Element(ElementType.Wood), "Stable Wood Catalyst");
            highTalisman.AddSecondaryElement(new Element(ElementType.Fire)); // Wood generates Fire (good)
            
            // Moderate Stability - Mixed elements
            var moderateTalisman = new Talisman(new Element(ElementType.Water), "Balanced Water Core");
            moderateTalisman.AddSecondaryElement(new Element(ElementType.Wood)); // Water generates Wood (good)
            moderateTalisman.AddSecondaryElement(new Element(ElementType.Earth)); // Neutral interaction
            
            // Low Stability - Conflicting elements
            var lowTalisman = new Talisman(new Element(ElementType.Fire), "Unstable Fire Shard");
            lowTalisman.AddSecondaryElement(new Element(ElementType.Water)); // Water destroys Fire (bad)
            lowTalisman.AddSecondaryElement(new Element(ElementType.Metal)); // Metal conflicts with Fire
            
            // Critical Instability - Highly conflicting elements
            var criticalTalisman = new Talisman(new Element(ElementType.Chaos, 0.8), "Chaotic Nexus");
            criticalTalisman.AddSecondaryElement(new Element(ElementType.Fire, 0.7));
            criticalTalisman.AddSecondaryElement(new Element(ElementType.Water, 0.6));
            criticalTalisman.AddSecondaryElement(new Element(ElementType.Metal, 0.5));
            criticalTalisman.AddSecondaryElement(new Element(ElementType.Void, 0.4));
            
            return new[] { perfectTalisman, highTalisman, moderateTalisman, lowTalisman, criticalTalisman };
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
    }
}
