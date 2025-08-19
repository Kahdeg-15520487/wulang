using System;
using System.Collections.Generic;
using System.Linq;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Represents a magic circle containing arranged talismans with support for unified composition
    /// </summary>
    public class MagicCircle
    {
        public string Id { get; }
        public string Name { get; set; }
        public List<Talisman> Talismans { get; }
        
        // Center talisman support (for @element syntax in CNF)
        public Talisman? CenterTalisman { get; set; }
        
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
        
        // Connections to other circles (Network Approach)
        public List<CircleConnection> Connections { get; }
        
        // Nested circles support (Nested Approach)
        public List<MagicCircle> NestedCircles { get; }
        public MagicCircle? ParentCircle { get; set; }
        public double NestedScale { get; set; } = 1.0; // Size scaling when nested
        
        // Composition complexity properties
        public double CastingTime { get; private set; }
        public double ComplexityScore { get; private set; }
        public CompositionType CompositionType { get; private set; }

        public MagicCircle(string? name = null, double radius = 5.0)
        {
            Id = Utilities.GenerateShortId();
            Name = name ?? $"Circle_{Id}";
            Talismans = new List<Talisman>();
            AttachedArtifacts = new List<Artifact>();
            Connections = new List<CircleConnection>();
            NestedCircles = new List<MagicCircle>();
            Radius = radius;
            CenterX = 0;
            CenterY = 0;
            Layer = 0;
            IsActive = true;
            NestedScale = 1.0;
            CompositionType = CompositionType.Simple;
        }

        /// <summary>
        /// Internal constructor for deserialization that preserves the ID
        /// </summary>
        internal MagicCircle(string id, string? name = null, double radius = 5.0)
        {
            Id = id;
            Name = name ?? $"Circle_{Id}";
            Talismans = new List<Talisman>();
            AttachedArtifacts = new List<Artifact>();
            Connections = new List<CircleConnection>();
            NestedCircles = new List<MagicCircle>();
            Radius = radius;
            CenterX = 0;
            CenterY = 0;
            Layer = 0;
            IsActive = true;
            NestedScale = 1.0;
            CompositionType = CompositionType.Simple;
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
        /// Sets the center talisman for this circle
        /// </summary>
        public void SetCenterTalisman(Talisman talisman)
        {
            CenterTalisman = talisman;
            
            // Position center talisman at the circle's center
            talisman.X = CenterX;
            talisman.Y = CenterY;
            talisman.Z = Layer;
            
            RecalculateProperties();
        }

        /// <summary>
        /// Removes the center talisman from this circle
        /// </summary>
        public void RemoveCenterTalisman()
        {
            CenterTalisman = null;
            RecalculateProperties();
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
            var allTalismans = new List<Talisman>(Talismans);
            if (CenterTalisman != null)
            {
                allTalismans.Add(CenterTalisman);
            }
            
            if (allTalismans.Count == 0)
            {
                PowerOutput = 0;
                Stability = 1.0;
                Efficiency = 0;
                RecalculateComposition();
                return;
            }

            // Calculate total power (including center talisman)
            PowerOutput = allTalismans.Sum(t => t.PowerLevel);

            // Calculate stability based on elemental balance and interactions
            var interactions = GetTalismanInteractions();
            var stableInteractions = interactions.Count(i => i.IsStable);
            var totalInteractions = Math.Max(1, interactions.Count);
            
            var interactionStability = (double)stableInteractions / totalInteractions;
            var averageTalismanStability = allTalismans.Average(t => t.Stability);
            
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
            
            // Update composition complexity and casting time
            RecalculateComposition();
        }

        public ElementType GetDominantElement()
        {
            if (!Talismans.Any())
            {
                return ElementType.Void; // Default element for empty circles
            }

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
            var compositionText = CompositionType != CompositionType.Simple ? $", {CompositionType}" : "";
            
            return $"{Name}: {Talismans.Count} talismans{artifactText}{compositionText}, " +
                   $"Power: {PowerOutput:F1}, Stability: {Stability:F2}, Complexity: {ComplexityScore:F1}";
        }

        /// <summary>
        /// Adds a nested circle inside this circle
        /// </summary>
        public bool NestCircle(MagicCircle nestedCircle, double scale = 0.6)
        {
            // Check if the nested circle can fit
            var maxNestedRadius = Radius * scale;
            if (nestedCircle.Radius > maxNestedRadius)
            {
                return false;
            }

            // Set up nesting relationship
            nestedCircle.ParentCircle = this;
            nestedCircle.NestedScale = scale;
            nestedCircle.Layer = Layer; // Same layer as parent
            
            // Position nested circle (can be customized)
            var availablePositions = CalculateNestedPositions();
            if (availablePositions.Count == 0)
            {
                return false;
            }

            var position = availablePositions[0];
            nestedCircle.CenterX = CenterX + position.X;
            nestedCircle.CenterY = CenterY + position.Y;

            NestedCircles.Add(nestedCircle);
            RecalculateComposition();
            
            return true;
        }

        /// <summary>
        /// Removes a nested circle
        /// </summary>
        public bool UnnestCircle(MagicCircle nestedCircle)
        {
            if (NestedCircles.Remove(nestedCircle))
            {
                nestedCircle.ParentCircle = null;
                nestedCircle.NestedScale = 1.0;
                RecalculateComposition();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates available positions for nested circles
        /// </summary>
        private List<(double X, double Y)> CalculateNestedPositions()
        {
            var positions = new List<(double, double)>();
            var maxDistance = Radius * 0.4; // Keep nested circles away from edge
            
            // For now, just offer center and cardinal directions
            positions.Add((0, 0)); // Center
            positions.Add((maxDistance, 0)); // East
            positions.Add((-maxDistance, 0)); // West
            positions.Add((0, maxDistance)); // North
            positions.Add((0, -maxDistance)); // South
            
            // Filter out occupied positions
            return positions.Where(pos => !IsNestedPositionOccupied(pos.Item1, pos.Item2)).ToList();
        }

        /// <summary>
        /// Checks if a nested position is already occupied
        /// </summary>
        private bool IsNestedPositionOccupied(double relativeX, double relativeY)
        {
            var absoluteX = CenterX + relativeX;
            var absoluteY = CenterY + relativeY;
            
            return NestedCircles.Any(circle => 
            {
                var distance = Math.Sqrt(Math.Pow(circle.CenterX - absoluteX, 2) + 
                                       Math.Pow(circle.CenterY - absoluteY, 2));
                return distance < (circle.Radius + 1.0); // Minimum separation
            });
        }

        /// <summary>
        /// Recalculates composition type and complexity
        /// </summary>
        private void RecalculateComposition()
        {
            // Determine composition type
            var hasNested = NestedCircles.Count > 0;
            var hasConnections = Connections.Count > 0;
            var hasStacking = Layer > 0 || (ParentCircle?.NestedCircles.Any(c => c.Layer != Layer) ?? false);

            CompositionType = (hasNested, hasConnections, hasStacking) switch
            {
                (true, true, true) => CompositionType.Unified,
                (true, true, false) => CompositionType.NestedNetwork,
                (true, false, true) => CompositionType.StackedNested,
                (false, true, true) => CompositionType.StackedNetwork,
                (true, false, false) => CompositionType.Nested,
                (false, true, false) => CompositionType.Network,
                (false, false, true) => CompositionType.Stacked,
                _ => CompositionType.Simple
            };

            // Calculate complexity score
            ComplexityScore = CalculateComplexityScore();
            
            // Calculate casting time
            CastingTime = CalculateCastingTime();
            
            // Do NOT call RecalculateProperties() here to avoid infinite recursion
        }

        /// <summary>
        /// Calculates the overall complexity score of this circle composition
        /// </summary>
        public double CalculateComplexityScore()
        {
            var baseComplexity = Talismans.Count * 1.0; // Base talisman complexity
            var radiusComplexity = Radius * 0.1; // Larger circles are more complex
            
            // Stacking complexity (layer-based)
            var stackingComplexity = Layer * 0.3;
            
            // Nested circles complexity
            var nestingComplexity = NestedCircles.Count * 1.5;
            foreach (var nested in NestedCircles)
            {
                nestingComplexity += nested.CalculateComplexityScore() * 0.7; // Recursive complexity
            }
            
            // Connection complexity
            var connectionComplexity = 0.0;
            foreach (var connection in Connections)
            {
                connectionComplexity += connection.Type switch
                {
                    ConnectionType.Direct => 0.5,
                    ConnectionType.Flow => 0.8,
                    ConnectionType.Resonance => 1.2,
                    ConnectionType.Trigger => 1.5,
                    _ => 0.0
                };
                
                // Add target complexity factor
                connectionComplexity += connection.Target.Talismans.Count * 0.2;
            }
            
            // Stability penalty (unstable compositions are harder to manage)
            var stabilityPenalty = (1.0 - Stability) * 2.0;
            
            return baseComplexity + radiusComplexity + stackingComplexity + 
                   nestingComplexity + connectionComplexity + stabilityPenalty;
        }

        /// <summary>
        /// Calculates casting time based on composition complexity
        /// </summary>
        public double CalculateCastingTime()
        {
            var baseTime = Math.Max(1.0, Radius / 3.0); // Minimum 1 second, scales with size
            var complexityTime = ComplexityScore * 0.4; // Each complexity point adds 0.4 seconds
            
            // Composition type modifiers
            var compositionModifier = CompositionType switch
            {
                CompositionType.Simple => 1.0,
                CompositionType.Stacked => 1.2,
                CompositionType.Nested => 1.4,
                CompositionType.Network => 1.6,
                CompositionType.StackedNested => 1.8,
                CompositionType.StackedNetwork => 2.0,
                CompositionType.NestedNetwork => 2.2,
                CompositionType.Unified => 2.5,
                _ => 1.0
            };
            
            // Stability affects casting time (more stable = faster casting)
            var stabilityModifier = 2.0 - Stability; // Range: 1.0 (perfect) to 2.0 (unstable)
            
            return Math.Max(0.5, (baseTime + complexityTime) * compositionModifier * stabilityModifier);
        }

        /// <summary>
        /// Gets all circles in this composition (including nested and connected)
        /// </summary>
        public List<MagicCircle> GetAllCompositionCircles()
        {
            var circles = new HashSet<MagicCircle> { this };
            
            // Add nested circles recursively
            foreach (var nested in NestedCircles)
            {
                foreach (var circle in nested.GetAllCompositionCircles())
                {
                    circles.Add(circle);
                }
            }
            
            // Add connected circles (non-recursive to avoid infinite loops)
            foreach (var connection in Connections)
            {
                circles.Add(connection.Target);
            }
            
            return circles.ToList();
        }

        /// <summary>
        /// Gets the total power output of the entire composition
        /// </summary>
        public double GetCompositionPowerOutput()
        {
            var totalPower = PowerOutput;
            
            // Add nested circle power (with diminishing returns)
            foreach (var nested in NestedCircles)
            {
                totalPower += nested.GetCompositionPowerOutput() * 0.8; // 80% efficiency
            }
            
            // Add connected circle power (based on connection type)
            foreach (var connection in Connections)
            {
                if (connection.IsActive)
                {
                    var connectionEfficiency = connection.Type switch
                    {
                        ConnectionType.Direct => 0.9,
                        ConnectionType.Flow => 0.7,
                        ConnectionType.Resonance => 1.2, // Can amplify power
                        ConnectionType.Trigger => 0.5, // Lower continuous power
                        _ => 0.5
                    };
                    
                    totalPower += connection.Target.PowerOutput * connection.Strength * connectionEfficiency;
                }
            }
            
            return totalPower;
        }
    }

    /// <summary>
    /// Types of magic circle composition
    /// </summary>
    public enum CompositionType
    {
        Simple,           // Single circle
        Stacked,          // Multiple layers (Z-axis)
        Nested,           // Circles inside circles
        Network,          // Connected circles
        StackedNested,    // Stacked + Nested
        StackedNetwork,   // Stacked + Network
        NestedNetwork,    // Nested + Network  
        Unified           // All three approaches combined
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
