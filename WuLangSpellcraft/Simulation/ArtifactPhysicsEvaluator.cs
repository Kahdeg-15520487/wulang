using System;
using System.Numerics;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Represents the result of stability-based spell casting in physics simulation.
    /// </summary>
    public class StabilityPhysicsResult
    {
        public bool CastingSuccessful { get; set; }
        public CastingResult CastingResult { get; set; }
        public EnhancedSpellResult? SpellResult { get; set; }
        public string FailureReason { get; set; } = "";
        
        public StabilityPhysicsResult(CastingResult castingResult)
        {
            CastingResult = castingResult;
            CastingSuccessful = castingResult.IsSuccessful;
            if (!CastingSuccessful)
            {
                FailureReason = GetFailureReason(castingResult.Outcome);
            }
        }
        
        private string GetFailureReason(CastingOutcome outcome)
        {
            return outcome switch
            {
                CastingOutcome.Fizzle => "The spell fizzles out due to instability",
                CastingOutcome.Backfire => "The spell backfires on the caster",
                CastingOutcome.ElementInversion => "Unstable elements invert the spell effect",
                CastingOutcome.CatastrophicFailure => "Catastrophic magical explosion occurs",
                CastingOutcome.TalismanDestruction => "The talisman is destroyed in the casting attempt",
                _ => "Spell casting failed for unknown reasons"
            };
        }
    }

    /// <summary>
    /// Evaluates an Artifact with stability-based casting and produces SpellResult for physics simulation.
    /// </summary>
    public static class ArtifactPhysicsEvaluator
    {
        /// <summary>
        /// Attempts to cast a spell using the artifact with stability mechanics, then evaluates physics.
        /// </summary>
        /// <param name="artifact">The artifact to cast with.</param>
        /// <param name="spellEnergyCost">The energy cost of the spell being cast.</param>
        /// <param name="origin">The origin point for the effect.</param>
        /// <param name="direction">The direction of the effect.</param>
        /// <param name="targetArea">Optional target area for area effects.</param>
        /// <param name="random">Optional random instance for deterministic testing.</param>
        /// <returns>A StabilityPhysicsResult containing casting outcome and physics result.</returns>
        public static StabilityPhysicsResult AttemptCastingWithPhysics(
            Artifact artifact, 
            double spellEnergyCost, 
            Vector2 origin, 
            Vector2 direction, 
            Vector2? targetArea = null,
            Random? random = null)
        {
            // Create a virtual casting result based on artifact stability
            var castingResult = SimulateCastingFromArtifactStability(artifact, spellEnergyCost, random);
            
            var result = new StabilityPhysicsResult(castingResult);
            
            // If casting was successful, generate physics result
            if (castingResult.IsSuccessful)
            {
                result.SpellResult = EvaluateEnhancedWithCastingResult(
                    artifact, castingResult, origin, direction, targetArea);
            }
            else
            {
                // Handle failure effects that might still have physics implications
                result.SpellResult = HandleCastingFailurePhysics(
                    artifact, castingResult, origin, direction);
            }
            
            return result;
        }

        /// <summary>
        /// Overload that accepts a talisman for direct stability casting.
        /// </summary>
        /// <param name="talisman">The talisman to cast with.</param>
        /// <param name="spellEnergyCost">The energy cost of the spell being cast.</param>
        /// <param name="origin">The origin point for the effect.</param>
        /// <param name="direction">The direction of the effect.</param>
        /// <param name="targetArea">Optional target area for area effects.</param>
        /// <param name="random">Optional random instance for deterministic testing.</param>
        /// <returns>A StabilityPhysicsResult containing casting outcome and physics result.</returns>
        public static StabilityPhysicsResult AttemptCastingWithPhysics(
            Talisman talisman,
            double spellEnergyCost, 
            Vector2 origin, 
            Vector2 direction, 
            Vector2? targetArea = null,
            Random? random = null)
        {
            // Use actual talisman casting mechanics
            var castingResult = talisman.AttemptCasting(spellEnergyCost, random);
            var result = new StabilityPhysicsResult(castingResult);
            
            // Create a temporary artifact from the talisman for physics evaluation
            var tempArtifact = CreateArtifactFromTalisman(talisman);
            
            // If casting was successful, generate physics result
            if (castingResult.IsSuccessful)
            {
                result.SpellResult = EvaluateEnhancedWithCastingResult(
                    tempArtifact, castingResult, origin, direction, targetArea);
            }
            else
            {
                // Handle failure effects that might still have physics implications
                result.SpellResult = HandleCastingFailurePhysics(
                    tempArtifact, castingResult, origin, direction);
            }
            
            return result;
        }

        /// <summary>
        /// Creates a temporary artifact from a talisman for physics evaluation.
        /// </summary>
        private static ElementalArtifact CreateArtifactFromTalisman(Talisman talisman)
        {
            var artifact = new ElementalArtifact(
                type: ArtifactType.SpellOrb,
                forgeElement: ElementType.Forge,
                primaryElement: talisman.PrimaryElement.Type,
                name: $"Spell from {talisman.Name}"
            );
            
            artifact.PowerLevel = talisman.PowerLevel;
            artifact.Stability = talisman.Stability;
            
            // Add secondary elements
            foreach (var element in talisman.SecondaryElements)
            {
                artifact.SecondaryElements.Add(element.Type);
            }
            
            return artifact;
        }

        /// <summary>
        /// Simulates casting result for artifacts that don't have source talismans.
        /// </summary>
        private static CastingResult SimulateCastingFromArtifactStability(
            Artifact artifact, double spellEnergyCost, Random? random = null)
        {
            random ??= new Random();
            var stability = artifact.Stability;
            
            // Create a virtual casting result based on artifact stability
            var result = new CastingResult
            {
                EnergyConsumed = spellEnergyCost
            };
            
            // Simplified stability-based outcome (mirrors Talisman logic)
            if (stability >= 0.9)
            {
                result.Outcome = random.NextDouble() < 0.2 ? CastingOutcome.EnhancedSuccess : CastingOutcome.Success;
                result.PowerMultiplier = 1.1 + (random.NextDouble() * 0.1);
                result.Message = "The artifact channels power smoothly.";
            }
            else if (stability >= 0.7)
            {
                result.Outcome = random.NextDouble() < 0.95 ? CastingOutcome.Success : CastingOutcome.Fizzle;
                result.PowerMultiplier = result.Outcome == CastingOutcome.Success ? 1.05 : 0;
                result.EnergyConsumed *= result.Outcome == CastingOutcome.Fizzle ? 0.3 : 1.0;
                result.Message = result.Outcome == CastingOutcome.Success ? 
                    "The artifact responds reliably." : "The artifact wavers slightly.";
            }
            else if (stability >= 0.5)
            {
                var roll = random.NextDouble();
                if (roll < 0.75)
                {
                    result.Outcome = CastingOutcome.Success;
                    result.PowerMultiplier = 0.8 + (random.NextDouble() * 0.4);
                    result.Message = "The artifact functions despite some instability.";
                }
                else
                {
                    result.Outcome = CastingOutcome.Fizzle;
                    result.PowerMultiplier = 0;
                    result.EnergyConsumed *= 0.5;
                    result.Message = "The artifact's instability causes the spell to fizzle.";
                }
            }
            else
            {
                // Low stability - high chance of failure
                var roll = random.NextDouble();
                if (roll < 0.5)
                {
                    result.Outcome = CastingOutcome.Success;
                    result.PowerMultiplier = 0.5 + (random.NextDouble() * 0.5);
                    result.Message = "The spell barely succeeds through the chaos.";
                }
                else if (roll < 0.8)
                {
                    result.Outcome = CastingOutcome.Fizzle;
                    result.PowerMultiplier = 0;
                    result.EnergyConsumed *= 0.75;
                    result.Message = "The unstable artifact fails to channel the spell.";
                }
                else
                {
                    result.Outcome = CastingOutcome.Backfire;
                    result.PowerMultiplier = 0.3;
                    result.Message = "The chaotic energies turn back on the user!";
                    result.SecondaryEffects.Add("Caster takes feedback damage");
                }
            }
            
            return result;
        }

        /// <summary>
        /// Creates physics effects for casting failures.
        /// </summary>
        private static EnhancedSpellResult? HandleCastingFailurePhysics(
            Artifact artifact, CastingResult castingResult, Vector2 origin, Vector2 direction)
        {
            return castingResult.Outcome switch
            {
                CastingOutcome.Backfire => CreateBackfireEffect(artifact, castingResult, origin),
                CastingOutcome.CatastrophicFailure => CreateExplosionEffect(artifact, castingResult, origin),
                CastingOutcome.ElementInversion => CreateInvertedEffect(artifact, castingResult, origin, direction),
                _ => null // Fizzle and talisman destruction produce no physics effects
            };
        }

        /// <summary>
        /// Creates a backfire effect that affects the caster's position.
        /// </summary>
        private static EnhancedSpellResult CreateBackfireEffect(
            Artifact artifact, CastingResult castingResult, Vector2 origin)
        {
            float power = (float)(artifact.PowerLevel * castingResult.PowerMultiplier);
            
            return new EnhancedSpellResult(
                origin: origin,
                direction: Vector2.Zero, // No specific direction for backfire
                power: power,
                mass: 0.5f + power * 0.2f,
                radius: 1.0f + power * 0.3f, // Larger area for backfire
                element: "Chaotic " + artifact.PrimaryElement.ToString(),
                effectType: SpellEffectType.AreaEffect,
                duration: 2.0f + power * 0.5f,
                initialVelocity: 0f,
                targetArea: origin // Targets the caster
            );
        }

        /// <summary>
        /// Creates an explosion effect for catastrophic failures.
        /// </summary>
        private static EnhancedSpellResult CreateExplosionEffect(
            Artifact artifact, CastingResult castingResult, Vector2 origin)
        {
            float power = (float)artifact.PowerLevel * 1.5f; // Catastrophic failures are more powerful
            
            return new EnhancedSpellResult(
                origin: origin,
                direction: Vector2.Zero,
                power: power,
                mass: 2.0f + power * 0.5f,
                radius: 3.0f + power * 0.5f, // Large explosion radius
                element: "Explosive " + artifact.PrimaryElement.ToString(),
                effectType: SpellEffectType.AreaEffect,
                duration: 3.0f + power * 0.3f,
                initialVelocity: 0f,
                targetArea: origin
            );
        }

        /// <summary>
        /// Creates an inverted element effect.
        /// </summary>
        private static EnhancedSpellResult CreateInvertedEffect(
            Artifact artifact, CastingResult castingResult, Vector2 origin, Vector2 direction)
        {
            float power = (float)(artifact.PowerLevel * castingResult.PowerMultiplier);
            
            // Invert the element type for the effect
            var invertedElement = GetInvertedElement(artifact.PrimaryElement);
            var invertedEffectType = DetermineEffectType(invertedElement);
            
            return new EnhancedSpellResult(
                origin: origin,
                direction: direction,
                power: power,
                mass: 1.0f + power * 0.5f,
                radius: 0.3f + power * 0.1f,
                element: invertedElement.ToString(),
                effectType: invertedEffectType,
                duration: invertedEffectType == SpellEffectType.Projectile ? 0f : 5.0f + power * 2.0f,
                initialVelocity: invertedEffectType == SpellEffectType.Projectile ? 10.0f + power * 2.0f : 0f,
                targetArea: null
            );
        }

        /// <summary>
        /// Gets the opposite element for element inversion effects.
        /// </summary>
        private static ElementType GetInvertedElement(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => ElementType.Water,
                ElementType.Water => ElementType.Fire,
                ElementType.Earth => ElementType.Wind,
                ElementType.Metal => ElementType.Wood,
                ElementType.Wood => ElementType.Metal,
                ElementType.Light => ElementType.Dark,
                ElementType.Dark => ElementType.Light,
                ElementType.Lightning => ElementType.Earth,
                ElementType.Wind => ElementType.Earth,
                ElementType.Chaos => ElementType.Void,
                ElementType.Void => ElementType.Chaos,
                _ => ElementType.Chaos // Default inversion
            };
        }

        /// <summary>
        /// Enhanced evaluation that applies casting result modifiers to physics.
        /// </summary>
        private static EnhancedSpellResult EvaluateEnhancedWithCastingResult(
            Artifact artifact, 
            CastingResult castingResult, 
            Vector2 origin, 
            Vector2 direction, 
            Vector2? targetArea = null)
        {
            // Get base physics evaluation
            var baseResult = EvaluateEnhanced(artifact, origin, direction, targetArea);
            
            // Apply casting result modifiers
            baseResult.Power *= (float)castingResult.PowerMultiplier;
            
            // Enhanced success gets additional benefits
            if (castingResult.Outcome == CastingOutcome.EnhancedSuccess)
            {
                baseResult.Duration *= 1.2f; // 20% longer duration
                baseResult.Radius *= 1.1f;   // 10% larger radius
                baseResult.InitialVelocity *= 1.15f; // 15% faster for projectiles
                baseResult.Element = "Enhanced " + baseResult.Element;
            }
            
            // Add stability effects to the element description
            if (castingResult.PowerMultiplier < 0.8)
            {
                baseResult.Element = "Unstable " + baseResult.Element;
            }
            else if (castingResult.PowerMultiplier > 1.1)
            {
                baseResult.Element = "Empowered " + baseResult.Element;
            }
            
            return baseResult;
        }

        /// <summary>
        /// Legacy method - now redirects to stability-based casting.
        /// </summary>
        [Obsolete("Use AttemptCastingWithPhysics for stability-based casting")]
        /// <summary>
        /// Evaluates the artifact and returns a SpellResult representing its physical effect.
        /// </summary>
        /// <param name="artifact">The artifact to evaluate.</param>
        /// <param name="origin">The origin point for the effect.</param>
        /// <param name="direction">The direction of the effect (should be normalized).</param>
        /// <returns>A SpellResult describing the physical effect.</returns>
        public static SpellResult Evaluate(Artifact artifact, Vector2 origin, Vector2 direction)
        {
            // Example logic: Use artifact's properties to determine effect
            // (In a real system, this would analyze the artifact's spell pattern, power, etc.)
            float power = (float)artifact.PowerLevel;
            float mass = 1.0f + power * 0.5f; // Mass scales with power
            float radius = 0.3f + power * 0.1f; // Radius scales with power
            string element = artifact.PrimaryElement.ToString();
            float initialVelocity = 10.0f + power * 2.0f; // Example scaling

            return new SpellResult(
                origin: origin,
                direction: direction,
                power: power,
                mass: mass,
                radius: radius,
                element: element,
                initialVelocity: initialVelocity
            );
        }

        /// <summary>
        /// Evaluates the artifact and returns an enhanced SpellResult with proper effect type.
        /// </summary>
        /// <param name="artifact">The artifact to evaluate.</param>
        /// <param name="origin">The origin point for the effect.</param>
        /// <param name="direction">The direction of the effect.</param>
        /// <param name="targetArea">Optional target area for area effects.</param>
        /// <returns>An EnhancedSpellResult describing the physical effect.</returns>
        public static EnhancedSpellResult EvaluateEnhanced(Artifact artifact, Vector2 origin, Vector2 direction, Vector2? targetArea = null)
        {
            float power = (float)artifact.PowerLevel;
            float mass = 1.0f + power * 0.5f;
            float radius = 0.3f + power * 0.1f;
            string element = artifact.PrimaryElement.ToString();
            
            // Determine effect type based on primary element
            SpellEffectType effectType = DetermineEffectType(artifact.PrimaryElement);
            
            // Calculate properties based on effect type
            float duration = effectType == SpellEffectType.Projectile ? 0f : 5.0f + power * 2.0f;
            float initialVelocity = effectType == SpellEffectType.Projectile ? 10.0f + power * 2.0f : 0f;

            return new EnhancedSpellResult(
                origin: origin,
                direction: direction,
                power: power,
                mass: mass,
                radius: radius,
                element: element,
                effectType: effectType,
                duration: duration,
                initialVelocity: initialVelocity,
                targetArea: targetArea
            );
        }

        /// <summary>
        /// Determines the spell effect type based on the primary element.
        /// </summary>
        private static SpellEffectType DetermineEffectType(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => SpellEffectType.Projectile,      // Fireballs, energy bolts
                ElementType.Earth => SpellEffectType.Barrier,        // Shields, walls, barriers
                ElementType.Metal => SpellEffectType.Enhancement,    // Weapon enhancement, precision
                ElementType.Wood => SpellEffectType.Growth,          // Healing, expansion, organic
                ElementType.Water => SpellEffectType.Flow,           // Streams, currents, fluid
                ElementType.Lightning => SpellEffectType.AreaEffect, // Lightning strikes
                ElementType.Wind => SpellEffectType.AreaEffect,      // Wind effects
                ElementType.Light => SpellEffectType.AreaEffect,     // Light area effects
                ElementType.Dark => SpellEffectType.Binding,         // Shadow binding
                ElementType.Forge => SpellEffectType.Transformation, // Matter transformation
                ElementType.Void => SpellEffectType.Teleportation,   // Movement/displacement
                ElementType.Chaos => SpellEffectType.AreaEffect,     // Chaotic area effects
                _ => SpellEffectType.Projectile                      // Default to projectile
            };
        }
    }
}
