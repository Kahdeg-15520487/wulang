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
        
        // Artifacts attached to this circle
        public List<Artifact> AttachedArtifacts { get; }
        
        // Connections to other circles
        public List<CircleConnection> Connections { get; }

        public MagicCircle(string? name = null, double radius = 5.0)
        {
            Id = Guid.NewGuid();
            Name = name ?? $"Circle_{Id.ToString()[..8]}";
            Talismans = new List<Talisman>();
            AttachedArtifacts = new List<Artifact>();
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

        public ElementType GetDominantElement()
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

        /// <summary>
        /// Attaches an artifact to this magic circle
        /// </summary>
        public bool AttachArtifact(Artifact artifact)
        {
            if (AttachedArtifacts.Contains(artifact))
                return false;

            AttachedArtifacts.Add(artifact);
            RecalculateProperties(); // Artifacts may affect circle properties
            return true;
        }

        /// <summary>
        /// Detaches an artifact from this magic circle
        /// </summary>
        public bool DetachArtifact(Artifact artifact)
        {
            if (AttachedArtifacts.Remove(artifact))
            {
                RecalculateProperties();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates an artifact using the Forge element with another element
        /// </summary>
        public Artifact? CreateElementalArtifact(ElementType forgeElement, ElementType targetElement)
        {
            // Check if we have forge element
            if (forgeElement != ElementType.Forge)
                return null;

            // Determine artifact type based on target element
            var artifactType = targetElement switch
            {
                ElementType.Water => ArtifactType.ChaliceOfFlow,
                ElementType.Fire => ArtifactType.CrucibleOfPower,
                ElementType.Earth => ArtifactType.FoundationStone,
                ElementType.Metal => ArtifactType.ConductorsRing,
                ElementType.Wood => ArtifactType.LivingCatalyst,
                ElementType.Lightning => ArtifactType.StormCore,
                ElementType.Wind => ArtifactType.EtherealAnchor,
                ElementType.Light => ArtifactType.BeaconOfTruth,
                ElementType.Dark => ArtifactType.ShadowVault,
                ElementType.Chaos => ArtifactType.WildcardRelic,
                ElementType.Void => ArtifactType.NullAnchor,
                _ => (ArtifactType?)null
            };

            if (artifactType == null)
                return null;

            var artifactName = $"{targetElement} {artifactType}";
            return new ElementalArtifact(artifactType.Value, forgeElement, targetElement, artifactName);
        }

        /// <summary>
        /// Creates a spell-imbued artifact by sacrificing this circle
        /// </summary>
        public SpellArtifact? CreateSpellArtifact(string artifactName, Artifact? baseArtifact = null)
        {
            // Must have at least one talisman to create a spell artifact
            if (Talismans.Count == 0)
                return null;

            // Create the spell artifact with this circle's pattern
            var spellArtifact = new SpellArtifact(artifactName, this);

            // If using a base artifact, enhance the spell artifact
            if (baseArtifact != null)
            {
                spellArtifact.PowerLevel += baseArtifact.PowerLevel * 0.5;
                spellArtifact.EnergyCapacity += baseArtifact.EnergyCapacity * 0.3;
            }

            return spellArtifact;
        }

        /// <summary>
        /// Checks if this circle can create derived elements
        /// </summary>
        public List<ElementType> GetPossibleDerivedElements()
        {
            var possibleElements = new List<ElementType>();
            var elementCounts = new Dictionary<ElementType, int>();

            // Count available elements
            foreach (var talisman in Talismans)
            {
                var elementType = talisman.PrimaryElement.Type;
                elementCounts[elementType] = elementCounts.GetValueOrDefault(elementType, 0) + 1;
            }

            // Check for possible derived elements
            var baseElements = new[] { ElementType.Water, ElementType.Fire, ElementType.Earth, ElementType.Metal, ElementType.Wood };
            
            for (int i = 0; i < baseElements.Length; i++)
            {
                for (int j = i + 1; j < baseElements.Length; j++)
                {
                    var element1 = baseElements[i];
                    var element2 = baseElements[j];
                    
                    if (elementCounts.GetValueOrDefault(element1, 0) > 0 && 
                        elementCounts.GetValueOrDefault(element2, 0) > 0)
                    {
                        var derivedElement = Element.TryCreateDerivedElement(element1, element2);
                        if (derivedElement.HasValue && !possibleElements.Contains(derivedElement.Value))
                        {
                            possibleElements.Add(derivedElement.Value);
                        }
                    }
                }
            }

            // Check for Chaos (need all 5 base elements)
            if (baseElements.All(e => elementCounts.GetValueOrDefault(e, 0) > 0))
            {
                possibleElements.Add(ElementType.Chaos);
            }

            return possibleElements;
        }

        /// <summary>
        /// Creates a copy of this magic circle for spell imbuement
        /// </summary>
        public MagicCircle Clone()
        {
            var clone = new MagicCircle(Name + " (Copy)", Radius)
            {
                CenterX = CenterX,
                CenterY = CenterY,
                Layer = Layer,
                IsActive = IsActive
            };

            // Clone talismans
            foreach (var talisman in Talismans)
            {
                var clonedTalisman = new Talisman(
                    new Element(talisman.PrimaryElement.Type, talisman.PrimaryElement.Energy),
                    talisman.Name
                )
                {
                    X = talisman.X,
                    Y = talisman.Y,
                    Z = talisman.Z,
                    PowerLevel = talisman.PowerLevel
                };

                // Clone secondary elements
                foreach (var secondaryElement in talisman.SecondaryElements)
                {
                    clonedTalisman.SecondaryElements.Add(
                        new Element(secondaryElement.Type, secondaryElement.Energy));
                }

                clone.Talismans.Add(clonedTalisman);
            }

            clone.RecalculateProperties();
            return clone;
        }

        public override string ToString()
        {
            var artifactCount = AttachedArtifacts.Count;
            var artifactText = artifactCount > 0 ? $", {artifactCount} artifacts" : "";
            
            return $"{Name}: {Talismans.Count} talismans{artifactText}, Power: {PowerOutput:F1}, " +
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
