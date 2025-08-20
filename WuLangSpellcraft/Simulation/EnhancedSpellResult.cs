using System.Numerics;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Represents different types of spell effects in the physics simulation.
    /// </summary>
    public enum SpellEffectType
    {
        Projectile,     // Fire: fireballs, energy bolts, explosive projectiles
        Barrier,        // Earth: shields, walls, protective barriers
        Enhancement,    // Metal: weapon enhancement, precision effects
        Growth,         // Wood: healing, expansion, organic manipulation
        Flow,           // Water: streams, currents, fluid manipulation
        Binding,        // Combination effects: holds, restraints, connections
        AreaEffect,     // Lightning/Wind/Light: area-of-effect spells
        Teleportation,  // Void/Chaos: movement effects
        Transformation  // Forge: changing matter/energy
    }

    /// <summary>
    /// Enhanced spell result that includes effect type and additional properties.
    /// </summary>
    public class EnhancedSpellResult
    {
        public Vector2 Origin { get; set; }
        public Vector2 Direction { get; set; }
        public Vector2 TargetArea { get; set; }  // For area effects and binding
        public float Power { get; set; }
        public float Mass { get; set; }
        public float Radius { get; set; }
        public string Element { get; set; }
        public SpellEffectType EffectType { get; set; }
        public float Duration { get; set; }      // For binding, barriers, enhancements
        public float InitialVelocity { get; set; }
        public bool IsStatic { get; set; }       // For barriers and binding points

        public EnhancedSpellResult(Vector2 origin, Vector2 direction, float power, float mass, 
            float radius, string element, SpellEffectType effectType, float duration = 0f, 
            float initialVelocity = 0f, Vector2? targetArea = null)
        {
            Origin = origin;
            Direction = Vector2.Normalize(direction);
            Power = power;
            Mass = mass;
            Radius = radius;
            Element = element;
            EffectType = effectType;
            Duration = duration;
            InitialVelocity = initialVelocity;
            TargetArea = targetArea ?? origin;
            IsStatic = effectType == SpellEffectType.Barrier || 
                      effectType == SpellEffectType.Binding ||
                      effectType == SpellEffectType.Enhancement;
        }
    }
}
