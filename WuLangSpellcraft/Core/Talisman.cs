using System;
using System.Collections.Generic;
using System.Linq;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Represents different stability levels for spell casting
    /// </summary>
    public enum StabilityLevel
    {
        CompleteInstability,    // 0.0 - 0.09
        CriticalInstability,    // 0.1 - 0.29
        LowStability,          // 0.3 - 0.49
        ModerateStability,     // 0.5 - 0.69
        HighStability,         // 0.7 - 0.89
        PerfectStability       // 0.9 - 1.0
    }

    /// <summary>
    /// Represents the outcome of a spell casting attempt
    /// </summary>
    public enum CastingOutcome
    {
        Success,
        EnhancedSuccess,
        Fizzle,
        Backfire,
        ElementInversion,
        CatastrophicFailure,
        TalismanDestruction
    }

    /// <summary>
    /// Contains the results of a spell casting attempt
    /// </summary>
    public class CastingResult
    {
        public CastingOutcome Outcome { get; set; }
        public double PowerMultiplier { get; set; } = 1.0;
        public double EnergyConsumed { get; set; }
        public double StabilityDamage { get; set; } = 0.0;
        public bool TalismanDestroyed { get; set; } = false;
        public string Message { get; set; } = "";
        public List<string> SecondaryEffects { get; set; } = new();

        public bool IsSuccessful => Outcome is CastingOutcome.Success or CastingOutcome.EnhancedSuccess;
    }

    /// <summary>
    /// Represents a magical talisman containing elemental power and spell components
    /// </summary>
    public class Talisman
    {
        public string Id { get; }
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
            Id = Utilities.GenerateShortId();
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
        /// Gets the current stability level category
        /// </summary>
        public StabilityLevel GetStabilityLevel()
        {
            return Stability switch
            {
                >= 0.9 => StabilityLevel.PerfectStability,
                >= 0.7 => StabilityLevel.HighStability,
                >= 0.5 => StabilityLevel.ModerateStability,
                >= 0.3 => StabilityLevel.LowStability,
                >= 0.1 => StabilityLevel.CriticalInstability,
                _ => StabilityLevel.CompleteInstability
            };
        }

        /// <summary>
        /// Attempts to cast a spell using this talisman
        /// </summary>
        public CastingResult AttemptCasting(double spellEnergyCost, Random? random = null)
        {
            random ??= new Random();
            var stabilityLevel = GetStabilityLevel();
            var result = new CastingResult
            {
                EnergyConsumed = spellEnergyCost
            };

            // Determine casting outcome based on stability level
            switch (stabilityLevel)
            {
                case StabilityLevel.PerfectStability:
                    result = HandlePerfectStabilityCasting(result, random);
                    break;
                case StabilityLevel.HighStability:
                    result = HandleHighStabilityCasting(result, random);
                    break;
                case StabilityLevel.ModerateStability:
                    result = HandleModerateStabilityCasting(result, random);
                    break;
                case StabilityLevel.LowStability:
                    result = HandleLowStabilityCasting(result, random);
                    break;
                case StabilityLevel.CriticalInstability:
                    result = HandleCriticalInstabilityCasting(result, random);
                    break;
                case StabilityLevel.CompleteInstability:
                    result = HandleCompleteInstabilityCasting(result, random);
                    break;
            }

            // Apply stability damage if any
            if (result.StabilityDamage > 0)
            {
                ApplyStabilityDamage(result.StabilityDamage);
            }

            return result;
        }

        /// <summary>
        /// Handles casting with perfect stability (0.9-1.0)
        /// </summary>
        private CastingResult HandlePerfectStabilityCasting(CastingResult result, Random random)
        {
            result.Outcome = random.NextDouble() < 0.2 ? CastingOutcome.EnhancedSuccess : CastingOutcome.Success;
            result.PowerMultiplier = 1.1 + (random.NextDouble() * 0.1); // 1.1 - 1.2x power
            result.EnergyConsumed *= random.NextDouble() < 0.3 ? 0.8 : 1.0; // 30% chance reduced cost
            result.Message = result.Outcome == CastingOutcome.EnhancedSuccess 
                ? "Perfect harmony amplifies your spell beyond expectations!"
                : "The talisman resonates perfectly with your intent.";
            
            if (result.Outcome == CastingOutcome.EnhancedSuccess)
            {
                result.SecondaryEffects.Add("Enhanced duration");
                result.SecondaryEffects.Add("Improved precision");
            }
            
            return result;
        }

        /// <summary>
        /// Handles casting with high stability (0.7-0.89)
        /// </summary>
        private CastingResult HandleHighStabilityCasting(CastingResult result, Random random)
        {
            if (random.NextDouble() < 0.95) // 95% success rate
            {
                result.Outcome = CastingOutcome.Success;
                result.PowerMultiplier = 1.05; // 5% power bonus
                result.Message = "The talisman responds smoothly to your will.";
                
                if (random.NextDouble() < 0.1) // 10% chance for minor enhancement
                {
                    result.SecondaryEffects.Add("Slightly enhanced range");
                }
            }
            else
            {
                result.Outcome = CastingOutcome.Fizzle;
                result.PowerMultiplier = 0;
                result.EnergyConsumed *= 0.3; // Minor energy loss
                result.Message = "The spell wavers slightly and dissipates.";
            }
            
            return result;
        }

        /// <summary>
        /// Handles casting with moderate stability (0.5-0.69)
        /// </summary>
        private CastingResult HandleModerateStabilityCasting(CastingResult result, Random random)
        {
            var roll = random.NextDouble();
            
            if (roll < 0.75) // 75% success rate
            {
                result.Outcome = CastingOutcome.Success;
                result.PowerMultiplier = 0.8 + (random.NextDouble() * 0.4); // ±20% power variation
                result.Message = "The spell succeeds despite some instability.";
            }
            else if (roll < 0.95) // 20% fizzle chance
            {
                result.Outcome = CastingOutcome.Fizzle;
                result.PowerMultiplier = 0;
                result.EnergyConsumed *= 0.5; // 50% energy waste
                result.Message = "The talisman wavers and the spell fizzles out.";
            }
            else // 5% wild magic
            {
                result.Outcome = CastingOutcome.Success;
                result.PowerMultiplier = 0.5 + (random.NextDouble() * 1.0); // Wild variation
                result.Message = "Wild magic courses through the unstable talisman!";
                result.SecondaryEffects.Add("Unpredictable secondary effect");
            }
            
            return result;
        }

        /// <summary>
        /// Handles casting with low stability (0.3-0.49)
        /// </summary>
        private CastingResult HandleLowStabilityCasting(CastingResult result, Random random)
        {
            var roll = random.NextDouble();
            
            if (roll < 0.5) // 50% success rate
            {
                result.Outcome = CastingOutcome.Success;
                result.PowerMultiplier = 0.5 + (random.NextDouble() * 0.5); // Reduced power
                result.Message = "The spell barely holds together through the chaos.";
            }
            else if (roll < 0.75) // 25% fizzle
            {
                result.Outcome = CastingOutcome.Fizzle;
                result.PowerMultiplier = 0;
                result.EnergyConsumed *= 0.75; // 75% energy waste
                result.Message = "The unstable talisman fails to channel your intent.";
            }
            else if (roll < 0.9) // 15% backfire
            {
                result.Outcome = CastingOutcome.Backfire;
                result.PowerMultiplier = 0.3; // Reduced power affects caster
                result.Message = "The chaotic energies turn back on you!";
                result.SecondaryEffects.Add("Caster takes feedback damage");
            }
            else if (roll < 0.95) // 5% element inversion
            {
                result.Outcome = CastingOutcome.ElementInversion;
                result.PowerMultiplier = 0.7;
                result.Message = "The conflicting elements invert your spell!";
                result.SecondaryEffects.Add("Opposite elemental effect manifested");
            }
            else // 5% stability damage
            {
                result.Outcome = CastingOutcome.Fizzle;
                result.PowerMultiplier = 0;
                result.EnergyConsumed *= 1.0;
                result.StabilityDamage = 0.05; // Permanent stability loss
                result.Message = "The talisman cracks under the strain!";
            }
            
            return result;
        }

        /// <summary>
        /// Handles casting with critical instability (0.1-0.29)
        /// </summary>
        private CastingResult HandleCriticalInstabilityCasting(CastingResult result, Random random)
        {
            var roll = random.NextDouble();
            
            if (roll < 0.25) // 25% success rate
            {
                result.Outcome = CastingOutcome.Success;
                result.PowerMultiplier = 0.3 + (random.NextDouble() * 0.4); // Very reduced power
                result.Message = "Miraculously, the spell succeeds despite the chaos.";
                result.StabilityDamage = 0.02; // Minor damage even on success
            }
            else if (roll < 0.5) // 25% fizzle
            {
                result.Outcome = CastingOutcome.Fizzle;
                result.PowerMultiplier = 0;
                result.EnergyConsumed *= 1.0; // Full energy waste
                result.StabilityDamage = 0.03;
                result.Message = "The talisman screams with conflicting energies!";
            }
            else if (roll < 0.7) // 20% backfire
            {
                result.Outcome = CastingOutcome.Backfire;
                result.PowerMultiplier = 0.5; // Moderate backfire damage
                result.StabilityDamage = 0.05;
                result.Message = "Chaotic energies explode back at you!";
                result.SecondaryEffects.Add("Moderate caster injury");
            }
            else if (roll < 0.85) // 15% catastrophic failure
            {
                result.Outcome = CastingOutcome.CatastrophicFailure;
                result.PowerMultiplier = 0;
                result.EnergyConsumed *= 1.5; // Overconsumption
                result.StabilityDamage = 0.1; // Major stability loss
                result.Message = "The talisman unleashes a catastrophic magical explosion!";
                result.SecondaryEffects.Add("Area damage around caster");
                result.SecondaryEffects.Add("Nearby talismans destabilized");
            }
            else // 15% talisman destruction
            {
                result.Outcome = CastingOutcome.TalismanDestruction;
                result.PowerMultiplier = 0;
                result.TalismanDestroyed = true;
                result.Message = "The talisman shatters into magical fragments!";
                result.SecondaryEffects.Add("Talisman permanently destroyed");
                result.SecondaryEffects.Add("Magical shrapnel damage");
            }
            
            return result;
        }

        /// <summary>
        /// Handles casting with complete instability (0.0-0.09)
        /// </summary>
        private CastingResult HandleCompleteInstabilityCasting(CastingResult result, Random random)
        {
            result.Outcome = CastingOutcome.TalismanDestruction;
            result.PowerMultiplier = 0;
            result.TalismanDestroyed = true;
            result.EnergyConsumed *= 2.0; // Massive energy drain
            result.Message = "The talisman explodes in a burst of chaotic magic!";
            result.SecondaryEffects.Add("Immediate talisman destruction");
            result.SecondaryEffects.Add("Explosive magical release");
            result.SecondaryEffects.Add("Severe caster injury");
            result.SecondaryEffects.Add("Magic circle damage");
            
            return result;
        }

        /// <summary>
        /// Applies stability damage to the talisman
        /// </summary>
        private void ApplyStabilityDamage(double damage)
        {
            // Reduce stability but don't recalculate based on elements
            // This represents permanent wear/damage
            var currentCalculatedStability = Stability;
            CalculateStability(); // Get the base stability
            var baseStability = Stability;
            
            // Apply damage as a permanent reduction
            Stability = Math.Max(0.0, baseStability - damage);
        }

        /// <summary>
        /// Gets a description of the current stability level
        /// </summary>
        public string GetStabilityDescription()
        {
            return GetStabilityLevel() switch
            {
                StabilityLevel.PerfectStability => "Perfect Harmony - The talisman hums with perfect elemental balance",
                StabilityLevel.HighStability => "High Stability - Reliable and responsive to magical intent",
                StabilityLevel.ModerateStability => "Moderate Stability - Some unpredictability in spell outcomes",
                StabilityLevel.LowStability => "Low Stability - Dangerous to use, high risk of failure",
                StabilityLevel.CriticalInstability => "Critical Instability - Extremely dangerous, likely to cause harm",
                StabilityLevel.CompleteInstability => "Complete Instability - A magical bomb waiting to explode",
                _ => "Unknown stability state"
            };
        }
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
            
            var stabilityLevel = GetStabilityLevel();
            var stabilityIcon = stabilityLevel switch
            {
                StabilityLevel.PerfectStability => "✨",
                StabilityLevel.HighStability => "✅",
                StabilityLevel.ModerateStability => "⚠️",
                StabilityLevel.LowStability => "⚠️⚠️",
                StabilityLevel.CriticalInstability => "⚠️⚠️⚠️",
                StabilityLevel.CompleteInstability => "💀",
                _ => "?"
            };
            
            return $"{Name}: {PrimaryElement.ChineseName}{secondaryInfo} " +
                   $"(Power: {PowerLevel:F1}, Stability: {Stability:F2} {stabilityIcon})";
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
                // Base Elements
                ElementType.Water => (new List<string> { "~", "≋", "◦" }, "Circle"),
                ElementType.Fire => (new List<string> { "▲", "◊", "※" }, "Triangle"),
                ElementType.Earth => (new List<string> { "■", "⬜", "◾" }, "Square"),
                ElementType.Metal => (new List<string> { "◇", "⬟", "⟐" }, "Diamond"),
                ElementType.Wood => (new List<string> { "※", "⟡", "⬢" }, "Hexagon"),
                
                // Derived Elements
                ElementType.Lightning => (new List<string> { "⚡", "⟲", "※" }, "Zigzag"),
                ElementType.Wind => (new List<string> { "○", "◐", "◑" }, "Spiral"),
                ElementType.Light => (new List<string> { "☀", "✦", "◊" }, "Star"),
                ElementType.Dark => (new List<string> { "●", "◉", "■" }, "Void"),
                ElementType.Forge => (new List<string> { "⚒", "◈", "⬟" }, "Anvil"),
                ElementType.Chaos => (new List<string> { "※", "⚡", "◉" }, "Fractal"),
                ElementType.Void => (new List<string> { "○", "●", "◯" }, "Hollow"),
                
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
