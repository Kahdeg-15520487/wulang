using System.Collections.Generic;
using System.Numerics;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Manages the 2D physics simulation world and tick updates.
    /// </summary>
    public class PhysicsWorld
    {
        public List<PhysicsObject> Objects { get; } = new();
        public Vector2 Gravity { get; set; } = new Vector2(0, -9.8f);
        public float TickDuration { get; set; } = 0.05f; // 20 ticks/sec

        public void AddObject(PhysicsObject obj) => Objects.Add(obj);

        public void Tick()
        {
            foreach (var obj in Objects)
            {
                if (obj.IsStatic) continue;
                // Apply gravity
                obj.Velocity += Gravity * TickDuration;
                // Update position
                obj.Position += obj.Velocity * TickDuration;
            }
            // Simple collision detection (circle vs. circle)
            HandleCollisions();
        }

        private void HandleCollisions()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                for (int j = i + 1; j < Objects.Count; j++)
                {
                    var a = Objects[i];
                    var b = Objects[j];
                    if (a.IsStatic && b.IsStatic) continue;
                    float dist = Vector2.Distance(a.Position, b.Position);
                    if (dist < a.Radius + b.Radius)
                    {
                        // Simple elastic collision response
                        ResolveCollision(a, b);
                    }
                }
            }
        }

        private void ResolveCollision(PhysicsObject a, PhysicsObject b)
        {
            // Only basic elastic collision for now
            Vector2 normal = Vector2.Normalize(b.Position - a.Position);
            float relativeVelocity = Vector2.Dot(b.Velocity - a.Velocity, normal);
            if (relativeVelocity > 0) return;
            float e = 1.0f; // perfectly elastic
            float j = -(1 + e) * relativeVelocity;
            j /= (1 / a.Mass) + (1 / b.Mass);
            a.Velocity -= (j / a.Mass) * normal;
            b.Velocity += (j / b.Mass) * normal;
        }
    }
}
