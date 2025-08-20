using System.Numerics;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Enhanced factory to convert EnhancedSpellResult into appropriate PhysicsObject for simulation.
    /// </summary>
    public static class EnhancedSpellPhysicsFactory
    {
        /// <summary>
        /// Creates a spell effect from a stability-based casting result.
        /// </summary>
        /// <param name="stabilityResult">The result of stability-based casting.</param>
        /// <returns>An EnhancedPhysicsObject if successful, null if casting failed without physics effects.</returns>
        public static EnhancedPhysicsObject? CreateSpellEffectFromStability(StabilityPhysicsResult stabilityResult)
        {
            if (stabilityResult.SpellResult == null)
            {
                return null; // No physics effect (e.g., fizzle, talisman destruction)
            }
            
            var physicsObject = CreateSpellEffect(stabilityResult.SpellResult);
            
            // Add stability-specific modifications
            if (stabilityResult.CastingResult.Outcome == CastingOutcome.Backfire)
            {
                physicsObject.Tag = "⚠️ " + physicsObject.Tag + " (Backfire)";
            }
            else if (stabilityResult.CastingResult.Outcome == CastingOutcome.ElementInversion)
            {
                physicsObject.Tag = "🔄 " + physicsObject.Tag + " (Inverted)";
            }
            else if (stabilityResult.CastingResult.Outcome == CastingOutcome.EnhancedSuccess)
            {
                physicsObject.Tag = "✨ " + physicsObject.Tag + " (Enhanced)";
            }
            else if (stabilityResult.CastingResult.Outcome == CastingOutcome.CatastrophicFailure)
            {
                physicsObject.Tag = "💥 " + physicsObject.Tag + " (Catastrophic)";
            }
            
            return physicsObject;
        }
        public static EnhancedPhysicsObject CreateSpellEffect(EnhancedSpellResult result)
        {
            Vector2 velocity = result.EffectType == SpellEffectType.Projectile 
                ? result.Direction * result.InitialVelocity 
                : Vector2.Zero;

            bool isStatic = result.EffectType switch
            {
                SpellEffectType.Barrier => true,
                SpellEffectType.Binding => true,
                SpellEffectType.Enhancement => true,
                SpellEffectType.AreaEffect => true,
                _ => false
            };

            return new EnhancedPhysicsObject(
                position: result.Origin,
                velocity: velocity,
                mass: result.Mass,
                radius: result.Radius,
                effectType: result.EffectType,
                duration: result.Duration,
                isStatic: isStatic,
                tag: result.Element
            )
            {
                TargetArea = result.TargetArea
            };
        }

        /// <summary>
        /// Creates a binding effect between two points.
        /// </summary>
        public static EnhancedPhysicsObject CreateBinding(Vector2 origin, Vector2 target, float power, float duration)
        {
            float distance = Vector2.Distance(origin, target);
            Vector2 center = (origin + target) / 2f;
            
            return new EnhancedPhysicsObject(
                position: center,
                velocity: Vector2.Zero,
                mass: 0.1f, // Light binding effect
                radius: distance / 2f, // Radius spans the binding distance
                effectType: SpellEffectType.Binding,
                duration: duration,
                isStatic: true,
                tag: "Binding"
            )
            {
                TargetArea = target
            };
        }

        /// <summary>
        /// Creates a barrier at the specified location.
        /// </summary>
        public static EnhancedPhysicsObject CreateBarrier(Vector2 position, float width, float height, float power, float duration)
        {
            return new EnhancedPhysicsObject(
                position: position,
                velocity: Vector2.Zero,
                mass: power * 10f, // Heavy barriers
                radius: Math.Max(width, height) / 2f,
                effectType: SpellEffectType.Barrier,
                duration: duration,
                isStatic: true,
                tag: "Barrier"
            );
        }
    }
}
