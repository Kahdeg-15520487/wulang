using System;
using System.Linq;
using System.Numerics;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Serialization;
using WuLangSpellcraft.Simulation;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates creating and using a laser-shooting artifact
    /// </summary>
    public static class LaserArtifactDemo
    {
        public static void Run()
        {
            Console.WriteLine("⚡ LASER ARTIFACT CREATION & DEMONSTRATION");
            Console.WriteLine(new string('=', 50));
            
            try
            {
                // Step 1: Design the laser spell formation
                DemoLaserFormationDesign();
                
                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
                
                // Step 2: Create the laser artifact
                DemoLaserArtifactCreation();
                
                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
                
                // Step 3: Test the laser in action
                DemoLaserCasting();
                
                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
                
                // Step 4: Advanced laser variations
                DemoAdvancedLaserVariants();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in laser artifact demo: {ex.Message}");
            }
        }
        
        private static void DemoLaserFormationDesign()
        {
            Console.WriteLine("🔬 STEP 1: LASER FORMATION DESIGN");
            Console.WriteLine();
            
            // Design a focused light beam formation
            // Fire (F) + Wood (O) → Light (automatically created through elemental combination)
            // Metal (M) for precision and focus
            // Earth (E) for stability and structure
            string laserCnf = "C5 F O M E"; // Fire + Wood (creates Light), Metal for focus, Earth for stability
            Console.WriteLine($"📝 Laser Formation CNF: {laserCnf}");
            Console.WriteLine("   Elements Used:");
            Console.WriteLine("   • Fire (F) + Wood (O) → Light - Energy beam source (automatic combination)");
            Console.WriteLine("   • Metal (M) - Precision and beam focusing");
            Console.WriteLine("   • Earth (E) - Structural stability");
            
            var parser = new CnfParser();
            var laserFormation = parser.ParseCircle(laserCnf);
            
            Console.WriteLine();
            Console.WriteLine($"⚡ Laser Circle: {laserFormation.Name} (Radius: {laserFormation.Radius})");
            Console.WriteLine($"   Power Output: {laserFormation.PowerOutput:F2}");
            Console.WriteLine($"   Stability: {laserFormation.Stability:F3}");
            Console.WriteLine($"   Complexity: {laserFormation.ComplexityScore:F2}");
            
            // Display talisman composition
            Console.WriteLine();
            Console.WriteLine("🔮 Talisman Arrangement:");
            foreach (var talisman in laserFormation.Talismans)
            {
                string stateStr = talisman.PrimaryElement.State != ElementState.Normal ? $" ({talisman.PrimaryElement.State})" : "";
                Console.WriteLine($"   • {talisman.PrimaryElement.Type}{stateStr} - Power: {talisman.PowerLevel:F2}");
            }
        }
        
        private static void DemoLaserArtifactCreation()
        {
            Console.WriteLine("⚒️ STEP 2: LASER ARTIFACT CREATION");
            Console.WriteLine();
            
            // Create a Beacon of Truth artifact (Forge + Light) as the base
            var laserArtifact = new ElementalArtifact(
                type: ArtifactType.BeaconOfTruth,
                forgeElement: ElementType.Forge,
                primaryElement: ElementType.Light,
                name: "Precision Laser Wand"
            );
            
            // Enhance with Metal for focusing capabilities
            laserArtifact.SecondaryElements.Add(ElementType.Metal);
            laserArtifact.SecondaryElements.Add(ElementType.Fire); // Source energy
            laserArtifact.PowerLevel = 4.5; // High power for focused beam
            laserArtifact.Stability = 0.85; // Good stability for precise targeting
            
            // Configure artifact properties for laser effects
            laserArtifact.Properties["BeamFocusing"] = 0.95; // 95% beam coherence
            laserArtifact.Properties["PrecisionBonus"] = 0.8; // 80% accuracy boost
            laserArtifact.Properties["PenetrationPower"] = 0.7; // 70% armor penetration
            laserArtifact.Properties["MaxRange"] = 50.0; // 50 meter effective range
            
            Console.WriteLine($"⚡ Laser Artifact Created: {laserArtifact.Name}");
            Console.WriteLine($"   Type: {laserArtifact.Type}");
            Console.WriteLine($"   Primary Element: {laserArtifact.PrimaryElement}");
            Console.WriteLine($"   Secondary Elements: {string.Join(", ", laserArtifact.SecondaryElements)}");
            Console.WriteLine($"   Power Level: {laserArtifact.PowerLevel:F2}");
            Console.WriteLine($"   Stability: {laserArtifact.Stability:F2}");
            Console.WriteLine();
            Console.WriteLine("🎯 Laser Properties:");
            foreach (var prop in laserArtifact.Properties)
            {
                Console.WriteLine($"   • {prop.Key}: {prop.Value}");
            }
        }
        
        private static void DemoLaserCasting()
        {
            Console.WriteLine("🎯 STEP 3: LASER CASTING DEMONSTRATION");
            Console.WriteLine();
            
            // Create the laser formation for casting
            string laserCnf = "C5 F O M E"; // Fire + Wood (creates Light combination), Metal, Earth
            var parser = new CnfParser();
            var formation = parser.ParseCircle(laserCnf);
            
            // Get the fire talisman for casting (Fire + Wood combination creates light effect)
            var fireTalisman = formation.Talismans.FirstOrDefault(t => t.PrimaryElement.Type == ElementType.Fire);
            if (fireTalisman == null)
            {
                Console.WriteLine("❌ No fire talisman found in formation!");
                return;
            }
            
            Console.WriteLine($"🔮 Primary Laser Talisman: {fireTalisman.Name}");
            Console.WriteLine($"   Element: {fireTalisman.PrimaryElement.Type} (Fire + Wood combination for Light effect)");
            Console.WriteLine($"   Power Level: {fireTalisman.PowerLevel:F2}");
            Console.WriteLine($"   Stability: {fireTalisman.Stability:F2}");
            Console.WriteLine($"   Stability Level: {fireTalisman.GetStabilityLevel()}");
            
            // Test laser beam casting
            var origin = new Vector2(0, 0);
            var target = new Vector2(25, 5); // 25m away, 5m up
            var direction = Vector2.Normalize(target - origin);
            double energyCost = 20.0; // Moderate energy for focused beam
            
            Console.WriteLine();
            Console.WriteLine($"⚡ Attempting Laser Beam Cast (Energy Cost: {energyCost}):");
            Console.WriteLine($"   Origin: ({origin.X:F1}, {origin.Y:F1})");
            Console.WriteLine($"   Target: ({target.X:F1}, {target.Y:F1})");
            Console.WriteLine($"   Direction: ({direction.X:F2}, {direction.Y:F2})");
            
            // Perform stability-based casting
            var castingResult = ArtifactPhysicsEvaluator.AttemptCastingWithPhysics(
                fireTalisman, energyCost, origin, direction, target);
            
            Console.WriteLine();
            Console.WriteLine($"🎯 Laser Casting Result:");
            Console.WriteLine($"   Outcome: {castingResult.CastingResult.Outcome}");
            Console.WriteLine($"   Power Multiplier: {castingResult.CastingResult.PowerMultiplier:F2}x");
            Console.WriteLine($"   Energy Consumed: {castingResult.CastingResult.EnergyConsumed:F1}");
            Console.WriteLine($"   Message: {castingResult.CastingResult.Message}");
            
            if (castingResult.CastingSuccessful && castingResult.SpellResult != null)
            {
                var spellResult = castingResult.SpellResult;
                Console.WriteLine();
                Console.WriteLine($"⚡ Laser Beam Generated:");
                Console.WriteLine($"   Effect Type: {spellResult.EffectType}");
                Console.WriteLine($"   Element: {spellResult.Element}");
                Console.WriteLine($"   Power: {spellResult.Power:F2} (stability modified)");
                Console.WriteLine($"   Range: {Vector2.Distance(origin, target):F1} meters");
                Console.WriteLine($"   Beam Speed: {spellResult.InitialVelocity:F1} m/s");
                Console.WriteLine($"   Beam Width: {spellResult.Radius:F3} meters (highly focused)");
                
                // Create laser physics object
                var laserBeam = EnhancedSpellPhysicsFactory.CreateSpellEffectFromStability(castingResult);
                if (laserBeam != null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"🌟 Laser Physics Properties:");
                    Console.WriteLine($"   Physics Type: {laserBeam.Tag}");
                    Console.WriteLine($"   Effect Classification: {laserBeam.EffectType}");
                    Console.WriteLine($"   Instantaneous: {laserBeam.IsStatic}");
                    Console.WriteLine($"   Duration: {laserBeam.Duration:F2}s");
                    
                    // Simulate laser beam path
                    Console.WriteLine();
                    Console.WriteLine("⚡ Laser Beam Trajectory:");
                    SimulateLaserBeam(laserBeam, origin, target);
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"❌ Laser Casting Failed: {castingResult.FailureReason}");
                
                if (castingResult.SpellResult != null)
                {
                    Console.WriteLine("⚠️ Unstable laser effect generated (beam scatter/misfire)");
                }
            }
        }
        
        private static void DemoAdvancedLaserVariants()
        {
            Console.WriteLine("🚀 STEP 4: ADVANCED LASER VARIANTS");
            Console.WriteLine();
            
            // Variant 1: Cutting Laser (Light + Metal + Fire)
            Console.WriteLine("🔥 Cutting Laser Artifact:");
            var cuttingLaser = new ElementalArtifact(
                ArtifactType.BeaconOfTruth, ElementType.Forge, ElementType.Light, "Industrial Cutting Laser");
            cuttingLaser.SecondaryElements.Add(ElementType.Metal);
            cuttingLaser.SecondaryElements.Add(ElementType.Fire);
            cuttingLaser.PowerLevel = 6.0; // Very high power
            cuttingLaser.Properties["CuttingPower"] = 0.9;
            cuttingLaser.Properties["HeatGeneration"] = 0.8;
            Console.WriteLine($"   Power: {cuttingLaser.PowerLevel:F1} | Cutting: {cuttingLaser.Properties["CuttingPower"]}");
            
            Console.WriteLine();
            
            // Variant 2: Healing Laser (Light + Wood + Water)
            Console.WriteLine("💚 Medical Healing Laser:");
            var healingLaser = new ElementalArtifact(
                ArtifactType.BeaconOfTruth, ElementType.Forge, ElementType.Light, "Medical Healing Beam");
            healingLaser.SecondaryElements.Add(ElementType.Wood);
            healingLaser.SecondaryElements.Add(ElementType.Water);
            healingLaser.PowerLevel = 3.5; // Moderate power for healing
            healingLaser.Properties["HealingRate"] = 0.7;
            healingLaser.Properties["BiologicalSafety"] = 0.95;
            Console.WriteLine($"   Power: {healingLaser.PowerLevel:F1} | Healing: {healingLaser.Properties["HealingRate"]}");
            
            Console.WriteLine();
            
            // Variant 3: Scanning Laser (Light + Metal + Void)
            Console.WriteLine("🔍 Detection Scanning Laser:");
            var scanningLaser = new ElementalArtifact(
                ArtifactType.BeaconOfTruth, ElementType.Forge, ElementType.Light, "Advanced Scanner Beam");
            scanningLaser.SecondaryElements.Add(ElementType.Metal);
            scanningLaser.SecondaryElements.Add(ElementType.Void);
            scanningLaser.PowerLevel = 2.8; // Lower power for detection
            scanningLaser.Properties["ScanningResolution"] = 0.99;
            scanningLaser.Properties["PenetrationDepth"] = 0.6;
            Console.WriteLine($"   Power: {scanningLaser.PowerLevel:F1} | Resolution: {scanningLaser.Properties["ScanningResolution"]}");
            
            Console.WriteLine();
            Console.WriteLine("💡 Laser Design Tips:");
            Console.WriteLine("   • Light element provides the coherent beam");
            Console.WriteLine("   • Metal element adds precision and focusing");
            Console.WriteLine("   • Fire element increases cutting/damage power");
            Console.WriteLine("   • Wood element enables healing applications");
            Console.WriteLine("   • Void element allows penetration/scanning");
            Console.WriteLine("   • Higher stability = more precise beam control");
            Console.WriteLine("   • Lower beam radius = more focused energy");
        }
        
        private static void SimulateLaserBeam(EnhancedPhysicsObject laserBeam, Vector2 origin, Vector2 target)
        {
            float distance = Vector2.Distance(origin, target);
            Vector2 direction = Vector2.Normalize(target - origin);
            
            // Laser beams travel at near-instantaneous speed
            float beamSpeed = 299792458f; // Speed of light in m/s (simplified)
            float travelTime = distance / beamSpeed;
            
            Console.WriteLine($"   Beam travels {distance:F1}m in {travelTime * 1000000:F3} microseconds");
            Console.WriteLine($"   Beam path: straight line from origin to target");
            Console.WriteLine($"   Beam diameter: {laserBeam.Radius * 2 * 1000:F1}mm (highly focused)");
            
            // Calculate energy density
            float beamArea = (float)(Math.PI * Math.Pow(laserBeam.Radius, 2));
            float energyDensity = laserBeam.Mass / beamArea; // Using mass as energy proxy
            
            Console.WriteLine($"   Energy density: {energyDensity:F1} J/m²");
            Console.WriteLine($"   Effect: Instantaneous heat/light at target location");
            
            // Show impact characteristics
            Console.WriteLine();
            Console.WriteLine("💥 Impact Analysis:");
            Console.WriteLine($"   Target position: ({target.X:F1}, {target.Y:F1})");
            Console.WriteLine($"   Beam precision: ±{laserBeam.Radius * 1000:F1}mm accuracy");
            Console.WriteLine($"   Duration: {laserBeam.Duration:F2}s continuous beam");
            
            if (energyDensity > 100)
                Console.WriteLine("   Result: High-energy impact - cutting/damage effect");
            else if (energyDensity > 50)
                Console.WriteLine("   Result: Medium-energy impact - heating/marking effect");
            else
                Console.WriteLine("   Result: Low-energy impact - illumination/scanning effect");
        }
    }
}
