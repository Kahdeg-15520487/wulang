using System;
using System.Collections.Generic;
using System.Linq;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Represents a magic circle containing arranged talismans
    /// </summary>
    public class MagicCircle
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public List<Talisman> Talismans { get; }
        public double Radius { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Layer { get; set; } // Z-coordinate for 3D stacking
        
        // Circle properties
        public double PowerOutput { get; private set; }
        public double Stability { get; private set; }
        public double Efficiency { get; private set; }
        public bool IsActive { get; set; }
        
        // Connections to other circles
        public List<CircleConnection> Connections { get; }

        public MagicCircle(string? name = null, double radius = 5.0)
        {
            Id = Guid.NewGuid();
            Name = name ?? $"Circle_{Id.ToString()[..8]}";
            Talismans = new List<Talisman>();
            Connections = new List<CircleConnection>();
            Radius = radius;
            CenterX = 0;
            CenterY = 0;
            Layer = 0;
            IsActive = true;
        }

        /// <summary>
        /// Adds a talisman to the circle at a specific angle
        /// </summary>
        public bool AddTalisman(Talisman talisman, double angleRadians)
        {
            // Check if position is available
            var targetX = CenterX + Radius * Math.Cos(angleRadians);
            var targetY = CenterY + Radius * Math.Sin(angleRadians);
            
            if (IsTalismanPositionOccupied(targetX, targetY))
            {
                return false;
            }

            // Position the talisman
            talisman.X = targetX;
            talisman.Y = targetY;
            talisman.Z = Layer;
            
            Talismans.Add(talisman);
            RecalculateProperties();
            
            return true;
        }

        /// <summary>
        /// Adds a talisman to the next available position
        /// </summary>
        public bool AddTalisman(Talisman talisman)
        {
            var maxTalismans = CalculateMaxTalismans();
            if (Talismans.Count >= maxTalismans)
            {
                return false;
            }

            var angleStep = 2 * Math.PI / maxTalismans;
            var angle = Talismans.Count * angleStep;
            
            return AddTalisman(talisman, angle);
        }

        /// <summary>
        /// Removes a talisman from the circle
        /// </summary>
        public bool RemoveTalisman(Talisman talisman)
        {
            if (Talismans.Remove(talisman))
            {
                RecalculateProperties();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a connection to another magic circle
        /// </summary>
        public CircleConnection ConnectTo(MagicCircle targetCircle, ConnectionType connectionType)
        {
            var connection = new CircleConnection
            {
                Source = this,
                Target = targetCircle,
                Type = connectionType,
                Strength = CalculateConnectionStrength(targetCircle, connectionType)
            };

            Connections.Add(connection);
            targetCircle.Connections.Add(connection);
            
            return connection;
        }

        /// <summary>
        /// Calculates the spell effect of this circle
        /// </summary>
        public SpellEffect CalculateSpellEffect()
        {
            if (Talismans.Count == 0)
            {
                return new SpellEffect { Type = SpellEffectType.None, Power = 0 };
            }

            var dominantElement = GetDominantElement();
            var effectType = DetermineSpellEffectType(dominantElement);
            var power = PowerOutput * Efficiency;

            return new SpellEffect
            {
                Type = effectType,
                Element = dominantElement,
                Power = power,
                Stability = Stability,
                Range = CalculateRange(),
                Duration = CalculateDuration()
            };
        }

        /// <summary>
        /// Gets all talisman interactions within the circle
        /// </summary>
        public List<TalismanInteraction> GetTalismanInteractions()
        {
            var interactions = new List<TalismanInteraction>();
            
            for (int i = 0; i < Talismans.Count; i++)
            {
                for (int j = i + 1; j < Talismans.Count; j++)
                {
                    var interaction = Talismans[i].GetInteractionWith(Talismans[j]);
                    interactions.Add(interaction);
                }
            }

            return interactions;
        }

        private bool IsTalismanPositionOccupied(double x, double y, double tolerance = 1.0)
        {
            return Talismans.Any(t => 
                Math.Sqrt(Math.Pow(t.X - x, 2) + Math.Pow(t.Y - y, 2)) < tolerance);
        }

        private int CalculateMaxTalismans()
        {
            // Based on circle circumference and minimum talisman spacing
            var circumference = 2 * Math.PI * Radius;
            var minSpacing = 2.0; // Minimum space between talismans
            return Math.Max(3, (int)(circumference / minSpacing));
        }

        private void RecalculateProperties()
        {
            if (Talismans.Count == 0)
            {
                PowerOutput = 0;
                Stability = 1.0;
                Efficiency = 0;
                return;
            }

            // Calculate total power
            PowerOutput = Talismans.Sum(t => t.PowerLevel);

            // Calculate stability based on elemental balance and interactions
            var interactions = GetTalismanInteractions();
            var stableInteractions = interactions.Count(i => i.IsStable);
            var totalInteractions = Math.Max(1, interactions.Count);
            
            var interactionStability = (double)stableInteractions / totalInteractions;
            var averageTalismanStability = Talismans.Average(t => t.Stability);
            
            Stability = (interactionStability + averageTalismanStability) / 2;

            // Calculate efficiency based on elemental harmony
            var harmonies = interactions.Count(i => i.Relation == ElementRelation.Generates);
            var conflicts = interactions.Count(i => i.Relation == ElementRelation.Destroys);
            
            if (totalInteractions == 0)
            {
                Efficiency = 0.8; // Default efficiency for single talisman
            }
            else
            {
                var harmonyRatio = (double)harmonies / totalInteractions;
                var conflictPenalty = (double)conflicts / totalInteractions * 0.5;
                Efficiency = Math.Max(0.1, harmonyRatio - conflictPenalty + 0.3);
            }
        }

        private ElementType GetDominantElement()
        {
            var elementCounts = Talismans
                .GroupBy(t => t.PrimaryElement.Type)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.PowerLevel));

            return elementCounts.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        private SpellEffectType DetermineSpellEffectType(ElementType dominantElement)
        {
            return dominantElement switch
            {
                ElementType.Water => SpellEffectType.Flow,
                ElementType.Fire => SpellEffectType.Projectile,
                ElementType.Earth => SpellEffectType.Barrier,
                ElementType.Metal => SpellEffectType.Enhancement,
                ElementType.Wood => SpellEffectType.Growth,
                _ => SpellEffectType.None
            };
        }

        private double CalculateRange()
        {
            return PowerOutput * Efficiency * 2.0; // Base range calculation
        }

        private double CalculateDuration()
        {
            return Stability * PowerOutput * 0.5; // Base duration calculation
        }

        private double CalculateConnectionStrength(MagicCircle target, ConnectionType connectionType)
        {
            var distance = Math.Sqrt(Math.Pow(CenterX - target.CenterX, 2) + 
                                   Math.Pow(CenterY - target.CenterY, 2) + 
                                   Math.Pow(Layer - target.Layer, 2));
            
            var maxDistance = connectionType switch
            {
                ConnectionType.Direct => 15.0,
                ConnectionType.Resonance => 25.0,
                ConnectionType.Flow => 20.0,
                _ => 10.0
            };

            var distanceFactor = Math.Max(0, 1 - (distance / maxDistance));
            var compatibilityFactor = CalculateCompatibility(target);

            return distanceFactor * compatibilityFactor * Math.Min(Stability, target.Stability);
        }

        private double CalculateCompatibility(MagicCircle target)
        {
            var thisDominant = GetDominantElement();
            var targetDominant = target.GetDominantElement();
            
            return Element.GetElementRelation(thisDominant, targetDominant) switch
            {
                ElementRelation.Generates => 1.0,
                ElementRelation.Destroys => 0.3,
                ElementRelation.Neutral => 0.6,
                _ => 0.5
            };
        }

        public override string ToString()
        {
            return $"{Name}: {Talismans.Count} talismans, Power: {PowerOutput:F1}, " +
                   $"Stability: {Stability:F2}, Efficiency: {Efficiency:F2}";
        }
    }

    /// <summary>
    /// Types of connections between magic circles
    /// </summary>
    public enum ConnectionType
    {
        Direct,     // Direct energy transfer
        Resonance,  // Harmonic amplification
        Flow,       // Continuous energy flow
        Trigger     // Conditional activation
    }

    /// <summary>
    /// Represents a connection between two magic circles
    /// </summary>
    public class CircleConnection
    {
        public required MagicCircle Source { get; set; }
        public required MagicCircle Target { get; set; }
        public ConnectionType Type { get; set; }
        public double Strength { get; set; }
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            var typeSymbol = Type switch
            {
                ConnectionType.Direct => "→",
                ConnectionType.Resonance => "≋",
                ConnectionType.Flow => "~",
                ConnectionType.Trigger => "⚡",
                _ => "?"
            };

            return $"{Source.Name} {typeSymbol} {Target.Name} (Strength: {Strength:F2})";
        }
    }
}
