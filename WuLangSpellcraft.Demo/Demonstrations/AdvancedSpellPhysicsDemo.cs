using System;
using System.Linq;
using System.Numerics;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Simulation;
using WuLangSpellcraft.Serialization;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates three different spell types: projectile, healing, and artifact creation.
    /// </summary>
    public static class AdvancedSpellPhysicsDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("🌟 Advanced Spell Physics Demonstration 🌟");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            // Demo 1: Fireball (Projectile Effect)
            DemoFireball();
            Console.WriteLine();

            // Demo 2: Healing Spell (Growth Effect)
            DemoHealingSpell();
            Console.WriteLine();

            // Demo 3: Forge Formation - Ring of Protection Artifact
            DemoArtifactCreation();
            Console.WriteLine();

            Console.WriteLine("🎯 Physics Simulation Complete!");
        }

        private static void DemoFireball()
        {
            Console.WriteLine("🔥 Demo 1: Fireball Spell with Stability Casting");
            Console.WriteLine("──────────────────────────────────────────────────");

            // Create fireball formation using CNF
            string fireballCnf = "C4 F F~ E M";
            Console.WriteLine($"📝 Formation CNF: {fireballCnf}");
            
            try
            {
                var parser = new CnfParser();
                var formation = parser.ParseCircle(fireballCnf);
                Console.WriteLine($"⚪ Circle: {formation.Name} (Radius: {formation.Radius})");
                Console.WriteLine($"🧿 Talismans: {formation.Talismans.Count}");
                
                // Display talisman details
                foreach (var talisman in formation.Talismans)
                {
                    string stateStr = talisman.PrimaryElement.State != ElementState.Normal ? $"{talisman.PrimaryElement.State}" : "Normal";
                    Console.WriteLine($"   • {talisman.PrimaryElement.Type} ({stateStr})");
                }

                // Get the primary talisman for casting
                var primaryTalisman = formation.Talismans.FirstOrDefault(t => t.PrimaryElement.Type == ElementType.Fire);
                if (primaryTalisman == null)
                {
                    Console.WriteLine("❌ No fire talisman found in formation!");
                    return;
                }

                Console.WriteLine($"🔮 Primary Casting Talisman: {primaryTalisman.Name}");
                Console.WriteLine($"   Power Level: {primaryTalisman.PowerLevel:F2}");
                Console.WriteLine($"   Stability: {primaryTalisman.Stability:F2}");
                Console.WriteLine($"   Stability Level: {primaryTalisman.GetStabilityLevel()}");

                // Simulate physics effect with stability casting
                var origin = new Vector2(0, 0);
                var direction = new Vector2(1, 0); // Fire east
                double spellEnergyCost = 25.0; // Medium energy spell
                
                Console.WriteLine();
                Console.WriteLine($"🎯 Attempting Stability-Based Casting (Energy Cost: {spellEnergyCost}):");
                
                // Use the new stability-based casting system
                var stabilityResult = ArtifactPhysicsEvaluator.AttemptCastingWithPhysics(
                    primaryTalisman, spellEnergyCost, origin, direction);
                
                Console.WriteLine($"   Casting Outcome: {stabilityResult.CastingResult.Outcome}");
                Console.WriteLine($"   Power Multiplier: {stabilityResult.CastingResult.PowerMultiplier:F2}x");
                Console.WriteLine($"   Energy Consumed: {stabilityResult.CastingResult.EnergyConsumed:F1}");
                Console.WriteLine($"   Message: {stabilityResult.CastingResult.Message}");
                
                if (stabilityResult.CastingResult.SecondaryEffects.Any())
                {
                    Console.WriteLine("   Secondary Effects:");
                    foreach (var effect in stabilityResult.CastingResult.SecondaryEffects)
                    {
                        Console.WriteLine($"     • {effect}");
                    }
                }

                if (stabilityResult.CastingSuccessful && stabilityResult.SpellResult != null)
                {
                    var spellResult = stabilityResult.SpellResult;
                    Console.WriteLine();
                    Console.WriteLine($"🎯 Physics Effect Generated:");
                    Console.WriteLine($"   Effect Type: {spellResult.EffectType}");
                    Console.WriteLine($"   Element: {spellResult.Element}");
                    Console.WriteLine($"   Origin: ({spellResult.Origin.X:F1}, {spellResult.Origin.Y:F1})");
                    Console.WriteLine($"   Direction: ({spellResult.Direction.X:F1}, {spellResult.Direction.Y:F1})");
                    Console.WriteLine($"   Power: {spellResult.Power:F2} (modified by stability)");
                    Console.WriteLine($"   Initial Velocity: {spellResult.InitialVelocity:F1} m/s");
                    Console.WriteLine($"   Mass: {spellResult.Mass:F2} kg");
                    Console.WriteLine($"   Radius: {spellResult.Radius:F2} m");

                    // Create physics object using stability result
                    var physicsObject = EnhancedSpellPhysicsFactory.CreateSpellEffectFromStability(stabilityResult);
                    if (physicsObject != null)
                    {
                        Console.WriteLine($"🌍 Physics Object: {physicsObject.Tag}");
                        Console.WriteLine($"   Position: ({physicsObject.Position.X:F1}, {physicsObject.Position.Y:F1})");
                        Console.WriteLine($"   Velocity: ({physicsObject.Velocity.X:F1}, {physicsObject.Velocity.Y:F1})");
                        
                        // Create target for testing
                        var target = new EnhancedPhysicsObject(
                            position: new Vector2(20, 0), // 20 meters away
                            velocity: Vector2.Zero,
                            mass: 1000f, // Heavy target
                            radius: 2f, // Large target
                            effectType: WuLangSpellcraft.Simulation.SpellEffectType.Barrier,
                            duration: float.MaxValue, // Infinite durability
                            isStatic: true,
                            tag: "Target Dummy"
                        );
                        
                        Console.WriteLine();
                        Console.WriteLine($"🎯 Target: {target.Tag} at ({target.Position.X:F1}, {target.Position.Y:F1})");
                        
                        // Run physics simulation
                        Console.WriteLine();
                        Console.WriteLine("⚡ Stability-Affected Physics Simulation:");
                        RunFireballSimulation(physicsObject, target);
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"❌ Casting Failed: {stabilityResult.FailureReason}");
                    
                    // Some failures still create physics effects (backfire, explosions)
                    if (stabilityResult.SpellResult != null)
                    {
                        Console.WriteLine("⚠️ Failure Effect Generated:");
                        var failureObject = EnhancedSpellPhysicsFactory.CreateSpellEffectFromStability(stabilityResult);
                        if (failureObject != null)
                        {
                            Console.WriteLine($"   Effect: {failureObject.Tag}");
                            Console.WriteLine($"   Type: {failureObject.EffectType}");
                            Console.WriteLine($"   Area: {failureObject.Radius:F1}m radius");
                            Console.WriteLine($"   Duration: {failureObject.Duration:F1}s");
                        }
                    }
                }
                
                // Show talisman condition after casting
                Console.WriteLine();
                Console.WriteLine($"🔮 Talisman Condition After Casting:");
                Console.WriteLine($"   Stability: {primaryTalisman.Stability:F3} ({primaryTalisman.GetStabilityLevel()})");
                if (stabilityResult.CastingResult.StabilityDamage > 0)
                {
                    Console.WriteLine($"   Stability Damage: -{stabilityResult.CastingResult.StabilityDamage:F3}");
                }
                if (stabilityResult.CastingResult.TalismanDestroyed)
                {
                    Console.WriteLine("   ⚠️ TALISMAN DESTROYED!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in fireball demo: {ex.Message}");
            }
        }

        private static void DemoHealingSpell()
        {
            Console.WriteLine("🌱 Demo 2: Healing Spell with Stability Casting");
            Console.WriteLine("─────────────────────────────────────────────────");

            // Create healing formation using Wood elements
            string healingCnf = "C3 O O~ O";
            Console.WriteLine($"📝 Formation CNF: {healingCnf}");

            try
            {
                var parser = new CnfParser();
                var formation = parser.ParseCircle(healingCnf);
                Console.WriteLine($"⚪ Circle: {formation.Name} (Radius: {formation.Radius})");
                
                // Display talisman details
                foreach (var talisman in formation.Talismans)
                {
                    string stateStr = talisman.PrimaryElement.State != ElementState.Normal ? $"{talisman.PrimaryElement.State}" : "Normal";
                    Console.WriteLine($"   • {talisman.PrimaryElement.Type} ({stateStr})");
                }

                // Get the primary wood talisman for casting
                var primaryTalisman = formation.Talismans.FirstOrDefault(t => t.PrimaryElement.Type == ElementType.Wood);
                if (primaryTalisman == null)
                {
                    Console.WriteLine("❌ No wood talisman found in formation!");
                    return;
                }

                Console.WriteLine($"🌿 Primary Casting Talisman: {primaryTalisman.Name}");
                Console.WriteLine($"   Power Level: {primaryTalisman.PowerLevel:F2}");
                Console.WriteLine($"   Stability: {primaryTalisman.Stability:F2}");
                Console.WriteLine($"   Stability Level: {primaryTalisman.GetStabilityLevel()}");

                // Simulate physics effect with stability casting
                var origin = new Vector2(5, 5);
                var direction = new Vector2(0, 0); // No directional movement for healing
                var targetArea = new Vector2(5, 5); // Heal at origin
                double spellEnergyCost = 15.0; // Lower energy for healing spell
                
                Console.WriteLine();
                Console.WriteLine($"🎯 Attempting Stability-Based Healing (Energy Cost: {spellEnergyCost}):");
                
                // Use the new stability-based casting system
                var stabilityResult = ArtifactPhysicsEvaluator.AttemptCastingWithPhysics(
                    primaryTalisman, spellEnergyCost, origin, direction, targetArea);
                
                Console.WriteLine($"   Casting Outcome: {stabilityResult.CastingResult.Outcome}");
                Console.WriteLine($"   Power Multiplier: {stabilityResult.CastingResult.PowerMultiplier:F2}x");
                Console.WriteLine($"   Energy Consumed: {stabilityResult.CastingResult.EnergyConsumed:F1}");
                Console.WriteLine($"   Message: {stabilityResult.CastingResult.Message}");
                
                if (stabilityResult.CastingResult.SecondaryEffects.Any())
                {
                    Console.WriteLine("   Secondary Effects:");
                    foreach (var effect in stabilityResult.CastingResult.SecondaryEffects)
                    {
                        Console.WriteLine($"     • {effect}");
                    }
                }

                if (stabilityResult.CastingSuccessful && stabilityResult.SpellResult != null)
                {
                    var spellResult = stabilityResult.SpellResult;
                    Console.WriteLine();
                    Console.WriteLine($"🎯 Healing Effect Generated:");
                    Console.WriteLine($"   Effect Type: {spellResult.EffectType}");
                    Console.WriteLine($"   Element: {spellResult.Element}");
                    Console.WriteLine($"   Origin: ({spellResult.Origin.X:F1}, {spellResult.Origin.Y:F1})");
                    Console.WriteLine($"   Target Area: ({spellResult.TargetArea.X:F1}, {spellResult.TargetArea.Y:F1})");
                    Console.WriteLine($"   Power: {spellResult.Power:F2} (modified by stability)");
                    Console.WriteLine($"   Duration: {spellResult.Duration:F1} seconds");

                    // Create physics object using stability result
                    var physicsObject = EnhancedSpellPhysicsFactory.CreateSpellEffectFromStability(stabilityResult);
                    if (physicsObject != null)
                    {
                        Console.WriteLine($"🌍 Physics Object: {physicsObject.Tag}");
                        Console.WriteLine($"   Static: {physicsObject.IsStatic}");
                        Console.WriteLine($"   Duration: {physicsObject.Duration:F1}s");
                        Console.WriteLine($"   Effect Type: {physicsObject.EffectType}");
                        Console.WriteLine($"   Healing Area Radius: {physicsObject.Radius:F2}m");
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"❌ Healing Failed: {stabilityResult.FailureReason}");
                    
                    // Some failures still create physics effects
                    if (stabilityResult.SpellResult != null)
                    {
                        Console.WriteLine("⚠️ Failure Effect Generated:");
                        var failureObject = EnhancedSpellPhysicsFactory.CreateSpellEffectFromStability(stabilityResult);
                        if (failureObject != null)
                        {
                            Console.WriteLine($"   Effect: {failureObject.Tag}");
                            Console.WriteLine($"   Type: {failureObject.EffectType}");
                        }
                    }
                }
                
                // Show talisman condition after casting
                Console.WriteLine();
                Console.WriteLine($"🌿 Talisman Condition After Casting:");
                Console.WriteLine($"   Stability: {primaryTalisman.Stability:F3} ({primaryTalisman.GetStabilityLevel()})");
                if (stabilityResult.CastingResult.StabilityDamage > 0)
                {
                    Console.WriteLine($"   Stability Damage: -{stabilityResult.CastingResult.StabilityDamage:F3}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in healing spell demo: {ex.Message}");
            }
        }

        private static void DemoArtifactCreation()
        {
            Console.WriteLine("🔨 Demo 3: Ring of Protection Artifact Creation");
            Console.WriteLine("─────────────────────────────────────────────────");

            // Step 1: Create Barrier Formation
            string barrierCnf = "C5 E E~ E E V"; // Earth barrier with void stabilization
            Console.WriteLine($"📝 Barrier Formation CNF: {barrierCnf}");

            try
            {
                var parser1 = new CnfParser();
                var barrierFormation = parser1.ParseCircle(barrierCnf);
                Console.WriteLine($"🛡️ Barrier Circle: {barrierFormation.Name} (Radius: {barrierFormation.Radius})");
                
                // Step 2: Create Forge Formation
                string forgeCnf = "C4 O M W F"; // Forge + Metal + Wood + Fire for crafting
                Console.WriteLine($"📝 Forge Formation CNF: {forgeCnf}");
                
                var parser2 = new CnfParser();
                var forgeFormation = parser2.ParseCircle(forgeCnf);
                Console.WriteLine($"⚒️ Forge Circle: {forgeFormation.Name} (Radius: {forgeFormation.Radius})");

                // Step 3: Create the binding process
                Console.WriteLine();
                Console.WriteLine("🔗 Artifact Creation Process:");
                Console.WriteLine("   1. Activate barrier formation for protective pattern");
                Console.WriteLine("   2. Channel forge energy to transform pattern into ring");
                Console.WriteLine("   3. Bind the protection spell into physical artifact");

                // Create the barrier artifact first
                var barrierArtifact = new ElementalArtifact(
                    type: ArtifactType.FormationTablet,
                    forgeElement: ElementType.Forge,
                    primaryElement: ElementType.Earth,
                    name: "Barrier Pattern"
                );
                barrierArtifact.PowerLevel = barrierFormation.PowerOutput;
                barrierArtifact.SecondaryElements.Add(ElementType.Void); // For stability

                // Create the forge artifact
                var forgeArtifact = new ElementalArtifact(
                    type: ArtifactType.SpellWand,
                    forgeElement: ElementType.Forge,
                    primaryElement: ElementType.Forge,
                    name: "Crafting Focus"
                );
                forgeArtifact.PowerLevel = forgeFormation.PowerOutput;
                forgeArtifact.SecondaryElements.Add(ElementType.Metal);
                forgeArtifact.SecondaryElements.Add(ElementType.Wood);
                forgeArtifact.SecondaryElements.Add(ElementType.Fire);

                Console.WriteLine($"🛡️ Barrier Pattern Power: {barrierArtifact.PowerLevel:F2}");
                Console.WriteLine($"⚒️ Forge Focus Power: {forgeArtifact.PowerLevel:F2}");

                // Step 4: Simulate the binding process
                var bindingOrigin = new Vector2(0, 0);
                var bindingTarget = new Vector2(2, 0); // Ring formation
                
                // Evaluate forge effect
                var forgeResult = ArtifactPhysicsEvaluator.EvaluateEnhanced(
                    forgeArtifact, bindingOrigin, Vector2.UnitX, bindingTarget);
                
                Console.WriteLine();
                Console.WriteLine($"🔗 Binding Process:");
                Console.WriteLine($"   Effect Type: {forgeResult.EffectType}");
                Console.WriteLine($"   Duration: {forgeResult.Duration:F1} seconds");
                Console.WriteLine($"   Power Required: {forgeResult.Power:F2}");

                // Create binding physics object
                var bindingObject = EnhancedSpellPhysicsFactory.CreateBinding(
                    bindingOrigin, bindingTarget, forgeResult.Power, forgeResult.Duration);
                
                Console.WriteLine($"   Binding Distance: {bindingObject.Radius * 2:F1} units");
                Console.WriteLine($"   Binding Center: ({bindingObject.Position.X:F1}, {bindingObject.Position.Y:F1})");

                // Step 5: Create the final artifact
                var ringOfProtection = new ElementalArtifact(
                    type: ArtifactType.FormationRing,
                    forgeElement: ElementType.Forge,
                    primaryElement: ElementType.Earth,
                    name: "Ring of Protection"
                );
                
                // Combine powers and add bonus from forge process
                ringOfProtection.PowerLevel = (barrierArtifact.PowerLevel + forgeArtifact.PowerLevel) * 1.2; // 20% crafting bonus
                ringOfProtection.Stability = Math.Min(barrierArtifact.Stability * 1.5, 1.0); // Improved stability
                ringOfProtection.Rarity = ArtifactRarity.Rare; // Crafted artifacts are rare
                ringOfProtection.SecondaryElements.Add(ElementType.Forge);
                ringOfProtection.SecondaryElements.Add(ElementType.Void);

                Console.WriteLine();
                Console.WriteLine($"💍 Final Artifact Created: {ringOfProtection.Name}");
                Console.WriteLine($"   Type: {ringOfProtection.Type}");
                Console.WriteLine($"   Rarity: {ringOfProtection.Rarity}");
                Console.WriteLine($"   Power Level: {ringOfProtection.PowerLevel:F2}");
                Console.WriteLine($"   Stability: {ringOfProtection.Stability:F2}");
                Console.WriteLine($"   Primary Element: {ringOfProtection.PrimaryElement}");
                Console.WriteLine($"   Secondary Elements: {string.Join(", ", ringOfProtection.SecondaryElements)}");

                // Demonstrate the ring's protective effect
                Console.WriteLine();
                Console.WriteLine("🛡️ Ring's Protective Effect:");
                var protectionResult = ArtifactPhysicsEvaluator.EvaluateEnhanced(
                    ringOfProtection, new Vector2(0, 0), Vector2.Zero);
                
                Console.WriteLine($"   Effect Type: {protectionResult.EffectType}");
                Console.WriteLine($"   Protection Radius: {protectionResult.Radius:F2} meters");
                Console.WriteLine($"   Duration: {protectionResult.Duration:F1} seconds");
                Console.WriteLine($"   Barrier Strength: {protectionResult.Power:F2}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in artifact creation: {ex.Message}");
            }
        }

        private static void RunFireballSimulation(EnhancedPhysicsObject fireball, EnhancedPhysicsObject target)
        {
            // Create physics world
            var world = new PhysicsWorld();
            world.AddObject(fireball);
            world.AddObject(target);
            
            const float simulationTime = 3.0f; // 3 seconds max
            const float secondsPerUpdate = 1.0f; // Display every second
            float currentTime = 0f;
            float lastDisplayTime = 0f;
            int tickCount = 0;
            bool collisionOccurred = false;
            Vector2 collisionPosition = Vector2.Zero;
            float collisionTime = 0f;
            
            Console.WriteLine("📊 Second-by-Second Simulation:");
            Console.WriteLine("Time | Fireball Position | Fireball Velocity | Distance to Target");
            Console.WriteLine("─────┼───────────────────┼───────────────────┼───────────────────");
            
            // Display initial state
            float distanceToTarget = Vector2.Distance(fireball.Position, target.Position);
            Console.WriteLine($"{currentTime:F1}s | ({fireball.Position.X:F1}, {fireball.Position.Y:F1})      | ({fireball.Velocity.X:F1}, {fireball.Velocity.Y:F1})      | {distanceToTarget:F1}m");
            
            while (currentTime < simulationTime && !collisionOccurred)
            {
                // Run one physics tick
                world.Tick();
                currentTime += world.TickDuration;
                tickCount++;
                
                // Check for collision
                float distance = Vector2.Distance(fireball.Position, target.Position);
                if (distance <= fireball.Radius + target.Radius && !collisionOccurred)
                {
                    collisionOccurred = true;
                    collisionPosition = fireball.Position;
                    collisionTime = currentTime;
                }
                
                // Display every second
                if (currentTime - lastDisplayTime >= secondsPerUpdate)
                {
                    distanceToTarget = Vector2.Distance(fireball.Position, target.Position);
                    Console.WriteLine($"{currentTime:F1}s | ({fireball.Position.X:F1}, {fireball.Position.Y:F1})      | ({fireball.Velocity.X:F1}, {fireball.Velocity.Y:F1})      | {distanceToTarget:F1}m");
                    lastDisplayTime = currentTime;
                }
                
                // Stop if fireball goes too far off target
                if (fireball.Position.X > target.Position.X + 10 && !collisionOccurred)
                    break;
            }
            
            Console.WriteLine();
            
            if (collisionOccurred)
            {
                Console.WriteLine("💥 COLLISION DETECTED!");
                Console.WriteLine($"   Collision Time: {collisionTime:F3} seconds");
                Console.WriteLine($"   Collision Position: ({collisionPosition.X:F2}, {collisionPosition.Y:F2})");
                Console.WriteLine($"   Impact Velocity: ({fireball.Velocity.X:F2}, {fireball.Velocity.Y:F2})");
                
                // Calculate damage based on kinetic energy
                float kineticEnergy = 0.5f * fireball.Mass * fireball.Velocity.LengthSquared();
                float damage = kineticEnergy / 10f; // Arbitrary damage scaling
                
                Console.WriteLine($"   Kinetic Energy: {kineticEnergy:F2} J");
                Console.WriteLine($"   Estimated Damage: {damage:F1} HP");
                
                // Calculate trajectory
                float horizontalDistance = collisionPosition.X;
                float timeToTarget = collisionTime;
                float averageSpeed = horizontalDistance / timeToTarget;
                
                Console.WriteLine($"   Horizontal Distance Traveled: {horizontalDistance:F2}m");
                Console.WriteLine($"   Average Speed: {averageSpeed:F2} m/s");
            }
            else
            {
                Console.WriteLine("❌ Fireball missed the target!");
                Console.WriteLine($"   Final Position: ({fireball.Position.X:F2}, {fireball.Position.Y:F2})");
                Console.WriteLine($"   Final Velocity: ({fireball.Velocity.X:F2}, {fireball.Velocity.Y:F2})");
            }
            
            Console.WriteLine($"   Total Simulation Time: {currentTime:F3} seconds");
            Console.WriteLine($"   Total Physics Ticks: {tickCount}");
            Console.WriteLine($"   Gravity Effect: {world.Gravity.Y * currentTime:F2} m/s downward velocity gained");
        }
    }
}
