using System;
using System.Collections.Generic;
using System.Linq;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Represents a magical talisman containing elemental power and spell components
    /// </summary>
    public class Talisman
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public Element PrimaryElement { get; }
        public List<Element> SecondaryElements { get; }
        public double PowerLevel { get; set; }
        public double Stability { get; private set; }
        public TalismanPattern Pattern { get; set; }
        
        // Position in 2D space (for magic circle arrangement)
        public double X { get; set; }
        public double Y { get; set; }
        
        // Position in 3D space (for stacking)
        public double Z { get; set; }

        public Talisman(Element primaryElement, string? name = null)
        {
            Id = Guid.NewGuid();
            Name = name ?? $"{primaryElement.ChineseName} Talisman";
            PrimaryElement = primaryElement;
            SecondaryElements = new List<Element>();
            PowerLevel = primaryElement.Energy;
            Pattern = new TalismanPattern(primaryElement.Type);
            CalculateStability();
        }

        /// <summary>
        /// Adds a secondary element to the talisman
        /// </summary>
        public bool AddSecondaryElement(Element element)
        {
            if (SecondaryElements.Count >= 3) // Limit secondary elements
            {
                return false;
            }

            SecondaryElements.Add(element);
            RecalculatePower();
            CalculateStability();
            return true;
        }

        /// <summary>
        /// Calculates the interaction between this talisman and another
        /// </summary>
        public TalismanInteraction GetInteractionWith(Talisman other)
        {
            var primaryRelation = PrimaryElement.GetRelationTo(other.PrimaryElement);
            var resonance = CalculateResonance(other);
            var energyFlow = CalculateEnergyFlow(other);

            return new TalismanInteraction
            {
                Source = this,
                Target = other,
                Relation = primaryRelation,
                Resonance = resonance,
                EnergyFlow = energyFlow,
                IsStable = resonance > 0.3 && energyFlow > 0
            };
        }

        /// <summary>
        /// Calculates resonance between talismans based on elemental harmony
        /// </summary>
        private double CalculateResonance(Talisman other)
        {
            var baseResonance = PrimaryElement.GetRelationTo(other.PrimaryElement) switch
            {
                ElementRelation.Generates => 0.8,
                ElementRelation.Destroys => -0.5,
                ElementRelation.Neutral => 0.2,
                _ => 0.0
            };

            // Secondary elements can enhance or diminish resonance
            foreach (var secondaryA in SecondaryElements)
            {
                foreach (var secondaryB in other.SecondaryElements)
                {
                    var secondaryRelation = secondaryA.GetRelationTo(secondaryB);
                    baseResonance += secondaryRelation switch
                    {
                        ElementRelation.Generates => 0.1,
                        ElementRelation.Destroys => -0.1,
                        _ => 0
                    };
                }
            }

            return Math.Max(-1.0, Math.Min(1.0, baseResonance));
        }

        /// <summary>
        /// Calculates energy flow potential between talismans
        /// </summary>
        private double CalculateEnergyFlow(Talisman other)
        {
            var distance = Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2));
            var maxDistance = 10.0; // Maximum effective distance
            var distanceFactor = Math.Max(0, 1 - (distance / maxDistance));

            var relationMultiplier = PrimaryElement.GetRelationTo(other.PrimaryElement) switch
            {
                ElementRelation.Generates => 1.2,
                ElementRelation.Destroys => 0.3,
                ElementRelation.Neutral => 0.8,
                _ => 0.5
            };

            return PowerLevel * other.PowerLevel * distanceFactor * relationMultiplier * Stability;
        }

        /// <summary>
        /// Recalculates power level based on all elements
        /// </summary>
        private void RecalculatePower()
        {
            PowerLevel = PrimaryElement.Energy;
            
            foreach (var secondary in SecondaryElements)
            {
                var relation = PrimaryElement.GetRelationTo(secondary);
                PowerLevel += relation switch
                {
                    ElementRelation.Generates => secondary.Energy * 0.5,
                    ElementRelation.Destroys => secondary.Energy * -0.3,
                    ElementRelation.Neutral => secondary.Energy * 0.2,
                    _ => 0
                };
            }

            PowerLevel = Math.Max(0.1, PowerLevel); // Minimum power level
        }

        /// <summary>
        /// Calculates talisman stability based on elemental balance
        /// </summary>
        private void CalculateStability()
        {
            if (SecondaryElements.Count == 0)
            {
                Stability = 1.0; // Pure elements are perfectly stable
                return;
            }

            var conflicts = 0;
            var harmonies = 0;

            // Check primary vs secondary relationships
            foreach (var secondary in SecondaryElements)
            {
                switch (PrimaryElement.GetRelationTo(secondary))
                {
                    case ElementRelation.Generates:
                        harmonies++;
                        break;
                    case ElementRelation.Destroys:
                        conflicts++;
                        break;
                }
            }

            // Check secondary vs secondary relationships
            for (int i = 0; i < SecondaryElements.Count; i++)
            {
                for (int j = i + 1; j < SecondaryElements.Count; j++)
                {
                    switch (SecondaryElements[i].GetRelationTo(SecondaryElements[j]))
                    {
                        case ElementRelation.Generates:
                            harmonies++;
                            break;
                        case ElementRelation.Destroys:
                            conflicts++;
                            break;
                    }
                }
            }

            // Calculate stability (0.0 to 1.0)
            var totalRelations = harmonies + conflicts;
            if (totalRelations == 0)
            {
                Stability = 0.8; // Neutral is relatively stable
            }
            else
            {
                var harmonyRatio = (double)harmonies / totalRelations;
                Stability = 0.2 + (harmonyRatio * 0.8); // Range from 0.2 to 1.0
            }
        }

        public override string ToString()
        {
            var secondaryInfo = SecondaryElements.Any() 
                ? $" + {string.Join(", ", SecondaryElements.Select(e => e.ChineseName))}"
                : "";
            
            return $"{Name}: {PrimaryElement.ChineseName}{secondaryInfo} (Power: {PowerLevel:F1}, Stability: {Stability:F1})";
        }
    }

    /// <summary>
    /// Represents the visual pattern of a talisman
    /// </summary>
    public class TalismanPattern
    {
        public ElementType Element { get; }
        public List<string> Symbols { get; }
        public string Shape { get; }
        public ConsoleColor Color { get; }

        public TalismanPattern(ElementType element)
        {
            Element = element;
            Color = new Element(element).Color;
            
            (Symbols, Shape) = element switch
            {
                ElementType.Water => (new List<string> { "~", "≋", "◦" }, "Circle"),
                ElementType.Fire => (new List<string> { "▲", "◊", "※" }, "Triangle"),
                ElementType.Earth => (new List<string> { "■", "⬜", "◾" }, "Square"),
                ElementType.Metal => (new List<string> { "◇", "⬟", "⟐" }, "Diamond"),
                ElementType.Wood => (new List<string> { "※", "⟡", "⬢" }, "Hexagon"),
                _ => (new List<string> { "?" }, "Circle")
            };
        }
    }

    /// <summary>
    /// Represents an interaction between two talismans
    /// </summary>
    public class TalismanInteraction
    {
        public required Talisman Source { get; set; }
        public required Talisman Target { get; set; }
        public ElementRelation Relation { get; set; }
        public double Resonance { get; set; }
        public double EnergyFlow { get; set; }
        public bool IsStable { get; set; }

        public override string ToString()
        {
            var relationSymbol = Relation switch
            {
                ElementRelation.Generates => "→",
                ElementRelation.Destroys => "⚔",
                ElementRelation.Neutral => "~",
                _ => "?"
            };

            return $"{Source.PrimaryElement.ChineseName} {relationSymbol} {Target.PrimaryElement.ChineseName} " +
                   $"(Resonance: {Resonance:F2}, Flow: {EnergyFlow:F1}, Stable: {IsStable})";
        }
    }
}
