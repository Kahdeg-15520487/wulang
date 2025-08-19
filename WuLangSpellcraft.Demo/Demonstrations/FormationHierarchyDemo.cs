using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates the proper hierarchy: Artifact -> Formation(s) -> Circle(s) -> Talisman(s)
    /// </summary>
    public static class FormationHierarchyDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Formation Hierarchy Demo: Artifact -> Formation -> Circles -> Talismans ===\n");
            
            DemonstrateBasicHierarchy();
            Console.WriteLine(new string('=', 80));
            
            DemonstrateComplexFormationArtifact();
            Console.WriteLine(new string('=', 80));
            
            DemonstrateMultiFormationArtifact();
        }

        /// <summary>
        /// Shows the basic hierarchy with a simple formation
        /// </summary>
        private static void DemonstrateBasicHierarchy()
        {
            Console.WriteLine("🔥 BASIC HIERARCHY DEMONSTRATION 🔥");
            Console.WriteLine("Creating: Artifact -> Formation -> Circles -> Talismans\n");

            // Step 1: Create Talismans (bottom level)
            Console.WriteLine("1. Creating Talismans:");
            var fireTalisman = new Talisman(new Element(ElementType.Fire, 2.0), "Flame Core");
            var earthTalisman = new Talisman(new Element(ElementType.Earth, 1.5), "Stone Base");
            var metalTalisman = new Talisman(new Element(ElementType.Metal, 1.0), "Iron Focus");
            Console.WriteLine($"   - {fireTalisman}");
            Console.WriteLine($"   - {earthTalisman}");
            Console.WriteLine($"   - {metalTalisman}\n");

            // Step 2: Create Circles and add Talismans (second level)
            Console.WriteLine("2. Creating Circles and adding Talismans:");
            var coreCircle = new MagicCircle("Core Power Circle", 6.0);
            coreCircle.AddTalisman(fireTalisman);
            coreCircle.AddTalisman(earthTalisman);
            
            var focusCircle = new MagicCircle("Focus Circle", 4.0);
            focusCircle.AddTalisman(metalTalisman);
            
            Console.WriteLine($"   - {coreCircle}");
            Console.WriteLine($"   - {focusCircle}\n");

            // Step 3: Create Formation and add Circles (third level)
            Console.WriteLine("3. Creating Formation and adding Circles:");
            var fireballFormation = new Formation("Basic Fireball Formation", 
                "A simple but effective fireball spell with power and focus components");
            
            fireballFormation.AddCircle("core", coreCircle);
            fireballFormation.AddCircle("focus", focusCircle);
            fireballFormation.AddConnection("core", "focus", ConnectionType.Flow, 0.8);
            fireballFormation.ResolveConnections();
            
            Console.WriteLine($"   - {fireballFormation}");
            Console.WriteLine($"     {fireballFormation.GetSummary()}\n");

            // Step 4: Create Artifact and engrave Formation (top level)
            Console.WriteLine("4. Creating FormationArtifact and engraving Formation:");
            var fireballStaff = new FormationArtifact(ArtifactType.FormationStaff, "Staff of Burning Earth", 3);
            var engraveSuccess = fireballStaff.EngraveFormation(fireballFormation);
            
            Console.WriteLine($"   - {fireballStaff}");
            Console.WriteLine($"   - Engraving success: {engraveSuccess}");
            Console.WriteLine($"   - Artifact description: {fireballStaff.Description}\n");

            // Step 5: Demonstrate the complete hierarchy in action
            Console.WriteLine("5. Using the complete hierarchy:");
            fireballStaff.SetActiveFormation(fireballFormation.Id);
            var castResult = fireballStaff.CastActiveFormation();
            
            if (castResult != null)
            {
                Console.WriteLine($"   - Cast Result: {castResult}");
                Console.WriteLine($"   - Formation Type: {castResult.Formation.Type}");
                Console.WriteLine($"   - Total Power: {castResult.TotalPower:F1}");
                Console.WriteLine($"   - Circle Effects: {castResult.CircleEffects.Count}");
                
                foreach (var effect in castResult.CircleEffects)
                {
                    Console.WriteLine($"     * {effect.Type} effect: Power {effect.Power:F1}, Element {effect.Element}");
                }
            }
            else
            {
                Console.WriteLine("   - Cast failed!");
            }
            
            Console.WriteLine("\n✅ Hierarchy demonstration complete!");
        }

        /// <summary>
        /// Shows a more complex formation with multiple connected circles
        /// </summary>
        private static void DemonstrateComplexFormationArtifact()
        {
            Console.WriteLine("⚡ COMPLEX FORMATION ARTIFACT ⚡");
            Console.WriteLine("Creating a multi-circle lightning storm formation\n");

            // Create a complex formation with multiple circles
            var stormFormation = new Formation("Lightning Storm Formation", 
                "A powerful weather manipulation spell with multiple coordinated circles");

            // Core storm circle
            var stormCore = new MagicCircle("Storm Core", 8.0);
            stormCore.AddTalisman(new Talisman(new Element(ElementType.Lightning, 3.0), "Lightning Seed"));
            stormCore.AddTalisman(new Talisman(new Element(ElementType.Wind, 2.0), "Wind Caller"));
            stormCore.SetCenterTalisman(new Talisman(new Element(ElementType.Water, 1.5), "Rain Heart"));

            // Amplification ring
            var ampCircle = new MagicCircle("Amplification Ring", 5.0) { Layer = 1.0 };
            ampCircle.AddTalisman(new Talisman(new Element(ElementType.Metal, 2.0), "Conductive Ring"));
            ampCircle.AddTalisman(new Talisman(new Element(ElementType.Fire, 1.0), "Heat Source"));

            // Control circle
            var controlCircle = new MagicCircle("Control Matrix", 4.0);
            controlCircle.AddTalisman(new Talisman(new Element(ElementType.Earth, 2.5), "Grounding Stone"));
            controlCircle.AddTalisman(new Talisman(new Element(ElementType.Wood, 1.5), "Natural Anchor"));

            // Nested precision circle
            var precisionCircle = new MagicCircle("Precision Guide", 2.0);
            precisionCircle.AddTalisman(new Talisman(new Element(ElementType.Light, 1.0), "Targeting Beam"));
            stormCore.NestCircle(precisionCircle, 0.4);

            // Add all circles to formation
            stormFormation.AddCircle("storm_core", stormCore);
            stormFormation.AddCircle("amplifier", ampCircle);
            stormFormation.AddCircle("control", controlCircle);

            // Create complex connection network
            stormFormation.AddConnection("storm_core", "amplifier", ConnectionType.Resonance, 1.2);
            stormFormation.AddConnection("storm_core", "control", ConnectionType.Flow, 0.9);
            stormFormation.AddConnection("amplifier", "control", ConnectionType.Direct, 0.7);

            stormFormation.ResolveConnections();

            Console.WriteLine($"Formation created: {stormFormation}");
            Console.WriteLine($"Formation details:\n{stormFormation.GetSummary()}\n");

            // Create a powerful formation orb
            var stormOrb = new FormationArtifact(ArtifactType.FormationOrb, "Orb of the Tempest", 1);
            stormOrb.EngraveFormation(stormFormation);

            Console.WriteLine($"Artifact: {stormOrb}");
            
            var formationInfos = stormOrb.GetFormationSummaries();
            foreach (var info in formationInfos)
            {
                Console.WriteLine($"  Engraved Formation: {info}");
                Console.WriteLine($"    Type: {info.Type}, Complexity: {info.Complexity:F1}");
                Console.WriteLine($"    Required Energy: {info.RequiredEnergy:F1}");
            }

            // Test casting
            Console.WriteLine("\nCasting the storm formation...");
            var result = stormOrb.CastFormation(stormFormation.Id);
            if (result != null)
            {
                Console.WriteLine($"Cast successful! {result}");
                Console.WriteLine($"Effects generated: {result.CircleEffects.Count}");
                foreach (var effect in result.CircleEffects)
                {
                    Console.WriteLine($"  - {effect.Type}: {effect.Element} (Power: {effect.Power:F1})");
                }
            }
        }

        /// <summary>
        /// Shows an artifact with multiple formations
        /// </summary>
        private static void DemonstrateMultiFormationArtifact()
        {
            Console.WriteLine("🌟 MULTI-FORMATION ARTIFACT 🌟");
            Console.WriteLine("Creating an artifact that can store multiple formations\n");

            // Create a versatile formation tablet
            var magicTablet = new FormationArtifact(ArtifactType.FormationTablet, "Tablet of the Five Elements", 5);

            // Create multiple simple formations
            var formations = new[]
            {
                CreateElementalFormation("Fire Burst", ElementType.Fire, ElementType.Earth),
                CreateElementalFormation("Ice Shield", ElementType.Water, ElementType.Metal),
                CreateElementalFormation("Lightning Strike", ElementType.Lightning, ElementType.Wind),
                CreateElementalFormation("Nature's Growth", ElementType.Wood, ElementType.Water),
                CreateElementalFormation("Void Drain", ElementType.Void, ElementType.Dark)
            };

            // Engrave all formations
            Console.WriteLine("Engraving formations onto the tablet:");
            foreach (var formation in formations)
            {
                var success = magicTablet.EngraveFormation(formation);
                Console.WriteLine($"  - {formation.Name}: {(success ? "✓" : "✗")}");
            }

            Console.WriteLine($"\nTablet status: {magicTablet}");
            Console.WriteLine($"Description: {magicTablet.Description}\n");

            // Show all available formations
            Console.WriteLine("Available formations:");
            var summaries = magicTablet.GetFormationSummaries();
            foreach (var summary in summaries)
            {
                Console.WriteLine($"  {summary}");
            }

            // Test switching between formations and casting
            Console.WriteLine("\nTesting formation switching and casting:");
            foreach (var formation in formations.Take(3)) // Test first 3
            {
                Console.WriteLine($"\nActivating {formation.Name}...");
                if (magicTablet.SetActiveFormation(formation.Id))
                {
                    var result = magicTablet.CastActiveFormation();
                    if (result != null)
                    {
                        Console.WriteLine($"  Cast: {result.Success} - {result.Message}");
                        Console.WriteLine($"  Power: {result.TotalPower:F1}, Stability: {result.OverallStability:F2}");
                    }
                    else
                    {
                        Console.WriteLine("  Cast failed - insufficient energy");
                    }
                }
            }

            Console.WriteLine($"\nTablet energy remaining: {magicTablet.CurrentEnergy:F1}/{magicTablet.EnergyCapacity:F1}");
        }

        /// <summary>
        /// Helper method to create a simple elemental formation
        /// </summary>
        private static Formation CreateElementalFormation(string name, ElementType primary, ElementType secondary)
        {
            var formation = new Formation(name, $"A formation combining {primary} and {secondary} elements");

            // Primary circle
            var primaryCircle = new MagicCircle($"{primary} Circle", 5.0);
            primaryCircle.AddTalisman(new Talisman(new Element(primary, 2.0), $"{primary} Core"));
            
            // Secondary circle  
            var secondaryCircle = new MagicCircle($"{secondary} Circle", 4.0);
            secondaryCircle.AddTalisman(new Talisman(new Element(secondary, 1.5), $"{secondary} Support"));

            formation.AddCircle("primary", primaryCircle);
            formation.AddCircle("secondary", secondaryCircle);
            formation.AddConnection("primary", "secondary", ConnectionType.Flow, 0.8);
            formation.ResolveConnections();

            return formation;
        }
    }
}
