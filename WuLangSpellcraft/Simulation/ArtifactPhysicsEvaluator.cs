using System.Numerics;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Evaluates an Artifact and produces a SpellResult for physics simulation.
    /// </summary>
    public static class ArtifactPhysicsEvaluator
    {
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
