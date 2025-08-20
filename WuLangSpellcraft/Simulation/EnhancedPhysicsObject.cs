using System.Numerics;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Enhanced physics object that can represent different spell effects.
    /// </summary>
    public class EnhancedPhysicsObject : PhysicsObject
    {
        public SpellEffectType EffectType { get; set; }
        public float Duration { get; set; }
        public float RemainingDuration { get; set; }
        public Vector2 TargetArea { get; set; }
        public bool IsActive { get; set; } = true;

        public EnhancedPhysicsObject(Vector2 position, Vector2 velocity, float mass, float radius, 
            SpellEffectType effectType, float duration = 0f, bool isStatic = false, string? tag = null)
            : base(position, velocity, mass, radius, isStatic, tag)
        {
            EffectType = effectType;
            Duration = duration;
            RemainingDuration = duration;
            TargetArea = position;
        }

        /// <summary>
        /// Updates the object for one physics tick.
        /// </summary>
        public virtual void UpdateTick(float deltaTime)
        {
            if (Duration > 0)
            {
                RemainingDuration -= deltaTime;
                if (RemainingDuration <= 0)
                {
                    IsActive = false;
                }
            }
        }

        /// <summary>
        /// Checks if this object can bind to another object.
        /// </summary>
        public virtual bool CanBindTo(PhysicsObject other)
        {
            return EffectType == SpellEffectType.Binding && IsActive;
        }

        /// <summary>
        /// Applies enhancement effect to another object.
        /// </summary>
        public virtual void ApplyEnhancement(PhysicsObject target)
        {
            if (EffectType == SpellEffectType.Enhancement && IsActive)
            {
                // Example: increase velocity or mass based on enhancement
                target.Velocity *= 1.2f; // 20% speed boost
            }
        }
    }
}
