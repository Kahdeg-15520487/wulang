using System.Numerics;

namespace WuLangSpellcraft.Simulation
{
    /// <summary>
    /// Factory to convert SpellResult into PhysicsObject for simulation.
    /// </summary>
    public static class SpellPhysicsFactory
    {
        public static PhysicsObject CreateProjectile(SpellResult result)
        {
            var velocity = result.Direction * result.InitialVelocity;
            return new PhysicsObject(
                position: result.Origin,
                velocity: velocity,
                mass: result.Mass,
                radius: result.Radius,
                isStatic: false,
                tag: result.Element
            );
        }
    }
}
