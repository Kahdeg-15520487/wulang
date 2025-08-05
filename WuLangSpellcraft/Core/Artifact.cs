using System;
using System.Collections.Generic;

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
        SpellWand,          // Contains a complete spell pattern
        SpellOrb,           // Alternative spell storage form
        SpellScroll,        // Temporary spell storage
        
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
}
