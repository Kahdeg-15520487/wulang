using System;
using System.Collections.Generic;
using System.Linq;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Represents the type of artifact and its primary function
    /// </summary>
    public enum ArtifactType
    {
        // Elemental Artifacts (Forge + single element)
        ChaliceOfFlow,      // Forge + Water
        CrucibleOfPower,    // Forge + Fire
        FoundationStone,    // Forge + Earth
        ConductorsRing,     // Forge + Metal
        LivingCatalyst,     // Forge + Wood
        
        // Derived Elemental Artifacts
        StormCore,          // Forge + Lightning
        EtherealAnchor,     // Forge + Wind
        BeaconOfTruth,      // Forge + Light
        ShadowVault,        // Forge + Dark
        WildcardRelic,      // Forge + Chaos
        NullAnchor,         // Forge + Void
        
        // Spell-Imbued Artifacts
        SpellWand,          // Contains a complete spell pattern (single circle)
        SpellOrb,           // Alternative spell storage form (single circle)
        SpellScroll,        // Temporary spell storage (single circle)
        
        // Formation-Imbued Artifacts (NEW - implements proper hierarchy)
        FormationTablet,    // Contains a complete formation (multiple circles)
        FormationStaff,     // Powerful formation artifact
        FormationOrb,       // Spherical formation storage
        FormationRing,      // Wearable formation artifact
        FormationAmulet,    // Protective formation artifact
        
        // Composite Artifacts
        ArtifactSet,        // Multiple artifacts working together
        EvolvedArtifact     // Artifact that has grown/changed over time
    }

    /// <summary>
    /// Represents the rarity and power level of an artifact
    /// </summary>
    public enum ArtifactRarity
    {
        Common,     // Basic elemental artifacts
        Uncommon,   // Enhanced or derived artifacts
        Rare,       // Complex spell-imbued artifacts
        Epic,       // Multi-spell or evolved artifacts
        Legendary   // Unique or extremely powerful artifacts
    }

    /// <summary>
    /// Base class for all artifacts in the Wu Lang Spellcraft system
    /// </summary>
    public abstract class Artifact
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ArtifactType Type { get; }
        public ArtifactRarity Rarity { get; set; }
        public ElementType PrimaryElement { get; }
        public List<ElementType> SecondaryElements { get; }
        
        // Artifact Properties
        public double PowerLevel { get; set; }
        public double Stability { get; set; }
        public double Durability { get; set; }
        public int MaxUses { get; set; }
        public int CurrentUses { get; set; }
        
        // Energy System
        public double EnergyCapacity { get; set; }
        public double CurrentEnergy { get; set; }
        public double EnergyEfficiency { get; set; }
        
        // Metadata
        public DateTime CreatedAt { get; }
        public Dictionary<string, object> Properties { get; }

        protected Artifact(ArtifactType type, ElementType primaryElement, string name)
        {
            Type = type;
            PrimaryElement = primaryElement;
            Name = name;
            Description = "";
            SecondaryElements = new List<ElementType>();
            Properties = new Dictionary<string, object>();
            
            // Default values
            PowerLevel = 1.0;
            Stability = 1.0;
            Durability = 100.0;
            MaxUses = 10;
            CurrentUses = MaxUses;
            EnergyCapacity = 100.0;
            CurrentEnergy = EnergyCapacity;
            EnergyEfficiency = 1.0;
            Rarity = ArtifactRarity.Common;
            CreatedAt = DateTime.Now;
        }

        /// <summary>
        /// Checks if the artifact can be used (has energy and durability)
        /// </summary>
        public virtual bool CanUse()
        {
            return CurrentUses > 0 && Durability > 0 && CurrentEnergy > 0;
        }

        /// <summary>
        /// Uses the artifact, consuming energy and durability
        /// </summary>
        public virtual bool TryUse(double energyRequired = 10.0)
        {
            if (!CanUse() || CurrentEnergy < energyRequired)
                return false;

            CurrentEnergy -= energyRequired;
            CurrentUses--;
            Durability -= 1.0;

            // Trigger any special effects
            OnUse();
            
            return true;
        }

        /// <summary>
        /// Recharges the artifact's energy
        /// </summary>
        public virtual void Recharge(double energy)
        {
            CurrentEnergy = Math.Min(CurrentEnergy + energy, EnergyCapacity);
        }

        /// <summary>
        /// Repairs the artifact, restoring durability and uses
        /// </summary>
        public virtual void Repair(double durabilityRestore = 50.0, int usesRestore = 5)
        {
            Durability = Math.Min(Durability + durabilityRestore, 100.0);
            CurrentUses = Math.Min(CurrentUses + usesRestore, MaxUses);
        }

        /// <summary>
        /// Override in derived classes for specific artifact effects
        /// </summary>
        protected virtual void OnUse() { }

        /// <summary>
        /// Gets the artifact's current condition as a percentage
        /// </summary>
        public double GetCondition()
        {
            return (Durability / 100.0) * (CurrentEnergy / EnergyCapacity);
        }

        public override string ToString()
        {
            return $"{Name} ({Type}) - Power: {PowerLevel:F1}, Condition: {GetCondition():P0}";
        }
    }

    /// <summary>
    /// Elemental artifacts created by combining Forge with base elements
    /// </summary>
    public class ElementalArtifact : Artifact
    {
        public double ElementalResonance { get; set; }
        public ElementType ForgeElement { get; }

        public ElementalArtifact(ArtifactType type, ElementType forgeElement, ElementType primaryElement, string name)
            : base(type, primaryElement, name)
        {
            ForgeElement = forgeElement;
            ElementalResonance = 1.0;
            
            // Set properties based on type
            InitializeElementalProperties();
        }

        private void InitializeElementalProperties()
        {
            switch (Type)
            {
                case ArtifactType.ChaliceOfFlow:
                    Description = "Increases water element efficiency and spell adaptation";
                    Properties["AdaptationBonus"] = 0.25;
                    Properties["WaterEfficiency"] = 1.5;
                    break;
                    
                case ArtifactType.CrucibleOfPower:
                    Description = "Amplifies all elemental energies with instability risk";
                    Properties["PowerAmplification"] = 1.5;
                    Properties["InstabilityRisk"] = 0.1;
                    break;
                    
                case ArtifactType.FoundationStone:
                    Description = "Increases circle stability and talisman capacity";
                    Properties["StabilityBonus"] = 0.5;
                    Properties["CapacityIncrease"] = 2;
                    break;
                    
                case ArtifactType.ConductorsRing:
                    Description = "Enhances connections between magic circles";
                    Properties["ConnectionRange"] = 2.0;
                    Properties["EnergyTransferEfficiency"] = 1.25;
                    break;
                    
                case ArtifactType.LivingCatalyst:
                    Description = "Enables artifact evolution and self-repair";
                    Properties["GrowthRate"] = 0.05;
                    Properties["SelfRepair"] = true;
                    break;
            }
        }

        protected override void OnUse()
        {
            // Living Catalyst can self-repair
            if (Type == ArtifactType.LivingCatalyst && Properties.ContainsKey("SelfRepair"))
            {
                var growthRate = (double)Properties["GrowthRate"];
                Repair(growthRate * 10, 0);
                
                // Potentially evolve
                if (PowerLevel < 5.0 && GetCondition() > 0.8)
                {
                    PowerLevel += growthRate;
                }
            }
        }
    }

    /// <summary>
    /// Artifacts that contain complete spell patterns
    /// </summary>
    public class SpellArtifact : Artifact
    {
        public MagicCircle? StoredSpell { get; set; }
        public SpellEffect StoredEffect { get; set; }
        public double CastingEfficiency { get; set; }
        
        public SpellArtifact(string name, MagicCircle spellToStore) 
            : base(ArtifactType.SpellWand, spellToStore.GetDominantElement(), name)
        {
            StoredSpell = spellToStore.Clone();
            StoredEffect = spellToStore.CalculateSpellEffect();
            CastingEfficiency = 0.9; // Slight efficiency loss compared to manual casting
            
            // Set properties based on stored spell
            PowerLevel = StoredEffect.Power;
            Description = $"Contains the spell pattern: {StoredSpell.Name}";
            EnergyCapacity = StoredEffect.Power * 20; // Scale energy capacity with spell power
            CurrentEnergy = EnergyCapacity;
        }

        /// <summary>
        /// Casts the stored spell if enough energy is available
        /// </summary>
        public SpellEffect? CastStoredSpell(double energyInput = 0)
        {
            var requiredEnergy = StoredEffect.Power * 10;
            var totalEnergy = CurrentEnergy + energyInput;
            
            if (totalEnergy < requiredEnergy)
                return null;

            if (!TryUse(requiredEnergy))
                return null;

            // Return modified spell effect based on artifact condition and efficiency
            var condition = GetCondition();
            return new SpellEffect
            {
                Type = StoredEffect.Type,
                Element = StoredEffect.Element,
                Power = StoredEffect.Power * CastingEfficiency * condition,
                Stability = StoredEffect.Stability * condition,
                Range = StoredEffect.Range,
                Duration = StoredEffect.Duration,
                Target = StoredEffect.Target,
                KineticForce = StoredEffect.KineticForce,
                HealingPower = StoredEffect.HealingPower,
                BarrierStrength = StoredEffect.BarrierStrength,
                EnhancementMultiplier = StoredEffect.EnhancementMultiplier
            };
        }
    }

    /// <summary>
    /// Artifacts that contain complete formations with multiple interconnected circles
    /// This properly implements the Artifact -> Formation -> Circles -> Talismans hierarchy
    /// </summary>
    public class FormationArtifact : Artifact
    {
        public List<Formation> EngravedFormations { get; }
        public Formation? ActiveFormation { get; set; }
        public double FormationEfficiency { get; set; }
        public int MaxFormations { get; set; }
        
        public FormationArtifact(ArtifactType type, string name, int maxFormations = 1) 
            : base(type, ElementType.Void, name) // Will be updated based on formations
        {
            EngravedFormations = new List<Formation>();
            FormationEfficiency = 0.85; // Formations lose some efficiency when stored
            MaxFormations = maxFormations;
            
            // Set artifact properties
            Description = $"Can store up to {maxFormations} formation(s)";
            EnergyCapacity = maxFormations * 200; // More capacity for complex formations
            CurrentEnergy = EnergyCapacity;
        }

        /// <summary>
        /// Engraves a formation onto this artifact
        /// </summary>
        public bool EngraveFormation(Formation formation)
        {
            if (EngravedFormations.Count >= MaxFormations)
                return false;

            // Clone the formation to avoid modifying the original
            var clonedFormation = formation.Clone();
            clonedFormation.Name = $"{formation.Name} (Engraved)";
            
            EngravedFormations.Add(clonedFormation);
            
            // Update artifact properties based on engraved formations
            UpdateArtifactProperties();
            
            return true;
        }

        /// <summary>
        /// Removes a formation from this artifact
        /// </summary>
        public bool RemoveFormation(string formationId)
        {
            var formation = EngravedFormations.FirstOrDefault(f => f.Id == formationId);
            if (formation == null) return false;

            EngravedFormations.Remove(formation);
            
            // Clear active formation if it was removed
            if (ActiveFormation?.Id == formationId)
                ActiveFormation = null;
                
            UpdateArtifactProperties();
            return true;
        }

        /// <summary>
        /// Sets the active formation for casting
        /// </summary>
        public bool SetActiveFormation(string formationId)
        {
            var formation = EngravedFormations.FirstOrDefault(f => f.Id == formationId);
            if (formation == null) return false;

            ActiveFormation = formation;
            return true;
        }

        /// <summary>
        /// Casts the active formation if enough energy is available
        /// </summary>
        public FormationCastResult? CastActiveFormation(double energyInput = 0)
        {
            if (ActiveFormation == null)
                return null;

            var requiredEnergy = CalculateRequiredEnergy(ActiveFormation);
            var totalEnergy = CurrentEnergy + energyInput;
            
            if (totalEnergy < requiredEnergy)
                return null;

            if (!TryUse(requiredEnergy))
                return null;

            // Execute the formation
            return ExecuteFormation(ActiveFormation);
        }

        /// <summary>
        /// Casts a specific formation by ID
        /// </summary>
        public FormationCastResult? CastFormation(string formationId, double energyInput = 0)
        {
            if (SetActiveFormation(formationId))
                return CastActiveFormation(energyInput);
            return null;
        }

        /// <summary>
        /// Gets information about all engraved formations
        /// </summary>
        public List<FormationInfo> GetFormationSummaries()
        {
            return EngravedFormations.Select(f => new FormationInfo
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                CircleCount = f.Circles.Count,
                ConnectionCount = f.Connections.Count,
                PowerOutput = f.TotalPowerOutput,
                Stability = f.OverallStability,
                Complexity = f.FormationComplexity,
                CastingTime = f.CastingTime,
                Type = f.Type,
                RequiredEnergy = CalculateRequiredEnergy(f)
            }).ToList();
        }

        /// <summary>
        /// Updates artifact properties based on engraved formations
        /// </summary>
        private void UpdateArtifactProperties()
        {
            if (EngravedFormations.Count == 0)
            {
                PowerLevel = 1.0;
                Stability = 1.0;
                Description = $"Can store up to {MaxFormations} formation(s)";
                return;
            }

            // Calculate average properties from all formations
            PowerLevel = EngravedFormations.Average(f => f.TotalPowerOutput);
            Stability = EngravedFormations.Average(f => f.OverallStability);
            
            // Update description
            var formationNames = string.Join(", ", EngravedFormations.Select(f => f.Name));
            Description = $"Contains formation(s): {formationNames}";
            
            // Update energy capacity based on most complex formation
            if (EngravedFormations.Count > 0)
            {
                var maxComplexity = EngravedFormations.Max(f => f.FormationComplexity);
                EnergyCapacity = maxComplexity * 50 + MaxFormations * 100;
            }
        }

        /// <summary>
        /// Calculates the energy required to cast a formation
        /// </summary>
        private double CalculateRequiredEnergy(Formation formation)
        {
            var baseCost = formation.TotalPowerOutput * 15;
            var complexityCost = formation.FormationComplexity * 10;
            var stabilityDiscount = formation.OverallStability * 0.2; // Better stability = less energy needed
            
            return Math.Max(10, (baseCost + complexityCost) * (1.0 - stabilityDiscount));
        }

        /// <summary>
        /// Executes a formation and returns the results
        /// </summary>
        private FormationCastResult ExecuteFormation(Formation formation)
        {
            var condition = GetCondition();
            var effectivePower = formation.TotalPowerOutput * FormationEfficiency * condition;
            var effectiveStability = formation.OverallStability * condition;
            
            // Resolve all connections in the formation
            formation.ResolveConnections();
            
            // Calculate spell effects from all circles
            var circleEffects = formation.Circles.Values
                .Select(circle => circle.CalculateSpellEffect())
                .ToList();
            
            return new FormationCastResult
            {
                Formation = formation,
                TotalPower = effectivePower,
                OverallStability = effectiveStability,
                CastingTime = formation.CastingTime,
                CircleEffects = circleEffects,
                Success = effectiveStability > 0.3, // Formations need higher stability than single circles
                Message = effectiveStability > 0.3 ? "Formation cast successfully" : "Formation casting failed due to instability"
            };
        }

        public override string ToString()
        {
            var formationCount = EngravedFormations.Count;
            var activeInfo = ActiveFormation != null ? $", Active: {ActiveFormation.Name}" : "";
            return $"{Name}: {formationCount}/{MaxFormations} formations{activeInfo}";
        }
    }

    /// <summary>
    /// Information about a formation stored in an artifact
    /// </summary>
    public class FormationInfo
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int CircleCount { get; set; }
        public int ConnectionCount { get; set; }
        public double PowerOutput { get; set; }
        public double Stability { get; set; }
        public double Complexity { get; set; }
        public double CastingTime { get; set; }
        public FormationType Type { get; set; }
        public double RequiredEnergy { get; set; }

        public override string ToString()
        {
            return $"{Name}: {CircleCount} circles, {ConnectionCount} connections, Power: {PowerOutput:F1}";
        }
    }

    /// <summary>
    /// Result of casting a formation from an artifact
    /// </summary>
    public class FormationCastResult
    {
        public required Formation Formation { get; set; }
        public double TotalPower { get; set; }
        public double OverallStability { get; set; }
        public double CastingTime { get; set; }
        public List<SpellEffect> CircleEffects { get; set; } = new();
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        public override string ToString()
        {
            var effectCount = CircleEffects.Count;
            var status = Success ? "SUCCESS" : "FAILED";
            return $"Formation Cast [{status}]: {Formation.Name}, {effectCount} effects, Power: {TotalPower:F1}";
        }
    }
}
