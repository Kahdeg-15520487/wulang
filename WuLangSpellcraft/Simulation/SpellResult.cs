using System.Numerics;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Represents the result of a spell cast for physics simulation.
    /// </summary>
    public class SpellResult
    {
        public Vector2 Origin { get; set; }
        public Vector2 Direction { get; set; }
        public float Power { get; set; }
        public float Mass { get; set; }
        public float Radius { get; set; }
        public string Element { get; set; }
        public float InitialVelocity { get; set; }

        public SpellResult(Vector2 origin, Vector2 direction, float power, float mass, float radius, string element, float initialVelocity)
        {
            Origin = origin;
            Direction = Vector2.Normalize(direction);
            Power = power;
            Mass = mass;
            Radius = radius;
            Element = element;
            InitialVelocity = initialVelocity;
        }
    }
}
