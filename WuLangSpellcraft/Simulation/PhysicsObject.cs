using System.Numerics;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Represents a physical object in the 2D simulation.
    /// </summary>
    public class PhysicsObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Mass { get; set; }
        public float Radius { get; set; }
        public bool IsStatic { get; set; }
        public string? Tag { get; set; }

        public PhysicsObject(Vector2 position, Vector2 velocity, float mass, float radius, bool isStatic = false, string? tag = null)
        {
            Position = position;
            Velocity = velocity;
            Mass = mass;
            Radius = radius;
            IsStatic = isStatic;
            Tag = tag;
        }
    }
}
