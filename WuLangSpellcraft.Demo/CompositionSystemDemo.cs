using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo
{
    /// <summary>
    /// Demonstrates the unified composition system for magic circles
    /// Showcases simple stacking, nested circles, connected networks, and unified compositions
    /// </summary>
    public static class CompositionSystemDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Wu Lang Spellcraft - Unified Composition System Demo ===\n");
            
            DemonstrateSimpleStacking();
            Console.WriteLine(new string('=', 80));
            
            DemonstrateNestedCircles();
            Console.WriteLine(new string('=', 80));
            
            DemonstrateConnectedNetwork();
            Console.WriteLine(new string('=', 80));
            
            DemonstrateUnifiedComposition();
            Console.WriteLine(new string('=', 80));
            
            DemonstrateCastingTimeAnalysis();
        }

        /// <summary>
        /// Demonstrates simple vertical stacking (Layer approach)
        /// </summary>
        private static void DemonstrateSimpleStacking()
        {
            Console.WriteLine("🔥 SIMPLE STACKING APPROACH 🔥");
            Console.WriteLine("Building a layered Fireball spell with power amplification\n");

            // Base layer - Core fireball
            var baseCircle = new MagicCircle("Fireball Core", 8.0) { Layer = 0.0 };
            baseCircle.AddTalisman(new Talisman(new Element(ElementType.Fire, 2.5), "Primary Fire"));
            baseCircle.AddTalisman(new Talisman(new Element(ElementType.Earth, 1.0), "Mass Component"));

            // Second layer - Power amplification
            var amplifierCircle = new MagicCircle("Power Amplifier", 6.0) { Layer = 3.0 };
            amplifierCircle.AddTalisman(new Talisman(new Element(ElementType.Fire, 1.5), "Fire Amplifier"));
            amplifierCircle.AddTalisman(new Talisman(new Element(ElementType.Metal, 1.2), "Focus Ring"));

            // Third layer - Precision targeting
            var targetingCircle = new MagicCircle("Precision Targeting", 4.0) { Layer = 6.0 };
            targetingCircle.AddTalisman(new Talisman(new Element(ElementType.Metal, 1.8), "Targeting Array"));

            Console.WriteLine($"Layer 0 (Base): {baseCircle}");
            Console.WriteLine($"  Casting Time: {baseCircle.CastingTime:F1}s");
            Console.WriteLine($"  Complexity: {baseCircle.ComplexityScore:F1}");
            Console.WriteLine();

            Console.WriteLine($"Layer 3 (Amplifier): {amplifierCircle}");
            Console.WriteLine($"  Casting Time: {amplifierCircle.CastingTime:F1}s");
            Console.WriteLine($"  Complexity: {amplifierCircle.ComplexityScore:F1}");
            Console.WriteLine();

            Console.WriteLine($"Layer 6 (Targeting): {targetingCircle}");
            Console.WriteLine($"  Casting Time: {targetingCircle.CastingTime:F1}s");
            Console.WriteLine($"  Complexity: {targetingCircle.ComplexityScore:F1}");
            Console.WriteLine();

            var totalCastingTime = baseCircle.CastingTime + amplifierCircle.CastingTime + targetingCircle.CastingTime;
            var totalComplexity = baseCircle.ComplexityScore + amplifierCircle.ComplexityScore + targetingCircle.ComplexityScore;
            
            Console.WriteLine($"📊 Stacked Composition Summary:");
            Console.WriteLine($"   Total Casting Time: {totalCastingTime:F1}s");
            Console.WriteLine($"   Total Complexity: {totalComplexity:F1}");
            Console.WriteLine($"   Layers: 3 (Simple Stacking)");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates nested circles approach
        /// </summary>
        private static void DemonstrateNestedCircles()
        {
            Console.WriteLine("⚡ NESTED CIRCLES APPROACH ⚡");
            Console.WriteLine("Creating a Lightning Storm with nested charge and direction modules\n");

            // Main circle - Lightning storm
            var mainCircle = new MagicCircle("Lightning Storm", 12.0);
            mainCircle.AddTalisman(new Talisman(new Element(ElementType.Lightning, 3.0), "Storm Core"));
            mainCircle.AddTalisman(new Talisman(new Element(ElementType.Wind, 2.0), "Air Current"));
            mainCircle.AddTalisman(new Talisman(new Element(ElementType.Water, 1.5), "Cloud Formation"));

            // Nested circle 1 - Electrical charge controller
            var chargeCircle = new MagicCircle("Charge Controller", 3.0);
            chargeCircle.AddTalisman(new Talisman(new Element(ElementType.Lightning, 1.8), "Charge Accumulator"));
            chargeCircle.AddTalisman(new Talisman(new Element(ElementType.Metal, 1.0), "Conductor"));

            // Nested circle 2 - Direction and targeting
            var directionCircle = new MagicCircle("Direction Module", 2.5);
            directionCircle.AddTalisman(new Talisman(new Element(ElementType.Wind, 1.2), "Wind Guide"));

            // Nest the circles
            var nested1 = mainCircle.NestCircle(chargeCircle, 0.5);
            var nested2 = mainCircle.NestCircle(directionCircle, 0.4);

            Console.WriteLine($"Main Circle: {mainCircle}");
            Console.WriteLine($"  Casting Time: {mainCircle.CastingTime:F1}s");
            Console.WriteLine($"  Total Power (with nested): {mainCircle.GetCompositionPowerOutput():F1}");
            Console.WriteLine();

            Console.WriteLine($"Nested Successfully: Charge={nested1}, Direction={nested2}");
            Console.WriteLine($"Nested Circles: {mainCircle.NestedCircles.Count}");
            Console.WriteLine($"All Composition Circles: {mainCircle.GetAllCompositionCircles().Count}");
            Console.WriteLine();

            foreach (var nested in mainCircle.NestedCircles)
            {
                Console.WriteLine($"  → {nested} (Scale: {nested.NestedScale:F1})");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates connected network approach
        /// </summary>
        private static void DemonstrateConnectedNetwork()
        {
            Console.WriteLine("🌊 CONNECTED NETWORK APPROACH 🌊");
            Console.WriteLine("Building an Ice-Fire steam explosion network\n");

            // Core energy distributor
            var coreCircle = new MagicCircle("Energy Core", 6.0);
            coreCircle.AddTalisman(new Talisman(new Element(ElementType.Lightning, 2.5), "Power Source"));
            coreCircle.AddTalisman(new Talisman(new Element(ElementType.Metal, 1.5), "Energy Distributor"));

            // Fire stream circle
            var fireCircle = new MagicCircle("Fire Stream", 5.0);
            fireCircle.AddTalisman(new Talisman(new Element(ElementType.Fire, 2.0), "Fire Generator"));
            fireCircle.AddTalisman(new Talisman(new Element(ElementType.Wind, 1.0), "Fire Propulsion"));

            // Ice stream circle
            var iceCircle = new MagicCircle("Ice Stream", 5.0);
            iceCircle.AddTalisman(new Talisman(new Element(ElementType.Water, 2.0), "Water Source"));
            iceCircle.AddTalisman(new Talisman(new Element(ElementType.Wind, 1.0), "Cooling Wind"));

            // Fusion reaction circle
            var fusionCircle = new MagicCircle("Steam Explosion", 4.0);
            fusionCircle.AddTalisman(new Talisman(new Element(ElementType.Fire, 1.0), "Heat Catalyst"));
            fusionCircle.AddTalisman(new Talisman(new Element(ElementType.Water, 1.0), "Steam Generator"));

            // Create the network connections
            var coreToFire = coreCircle.ConnectTo(fireCircle, ConnectionType.Flow);
            var coreToIce = coreCircle.ConnectTo(iceCircle, ConnectionType.Flow);
            var fireToFusion = fireCircle.ConnectTo(fusionCircle, ConnectionType.Direct);
            var iceToFusion = iceCircle.ConnectTo(fusionCircle, ConnectionType.Direct);
            var fusionResonance = fusionCircle.ConnectTo(coreCircle, ConnectionType.Resonance);

            Console.WriteLine($"Core Circle: {coreCircle}");
            Console.WriteLine($"  Connections: {coreCircle.Connections.Count}");
            Console.WriteLine($"  Network Power: {coreCircle.GetCompositionPowerOutput():F1}");
            Console.WriteLine();

            Console.WriteLine("Network Connections:");
            Console.WriteLine($"  {coreToFire}");
            Console.WriteLine($"  {coreToIce}");
            Console.WriteLine($"  {fireToFusion}");
            Console.WriteLine($"  {iceToFusion}");
            Console.WriteLine($"  {fusionResonance}");
            Console.WriteLine();

            Console.WriteLine("Individual Circle Analysis:");
            var allCircles = new[] { coreCircle, fireCircle, iceCircle, fusionCircle };
            foreach (var circle in allCircles)
            {
                Console.WriteLine($"  {circle.Name}: Power={circle.PowerOutput:F1}, " +
                                $"Stability={circle.Stability:F2}, CastTime={circle.CastingTime:F1}s");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates the ultimate unified composition system
        /// </summary>
        private static void DemonstrateUnifiedComposition()
        {
            Console.WriteLine("🌟 UNIFIED COMPOSITION SYSTEM 🌟");
            Console.WriteLine("Combining ALL approaches: Stacking + Nesting + Networking\n");

            // Layer 0: Base foundation circle
            var foundationCircle = new MagicCircle("Elemental Foundation", 15.0) { Layer = 0.0 };
            foundationCircle.AddTalisman(new Talisman(new Element(ElementType.Earth, 3.0), "Foundation Core"));
            foundationCircle.AddTalisman(new Talisman(new Element(ElementType.Metal, 2.0), "Structural Support"));
            foundationCircle.AddTalisman(new Talisman(new Element(ElementType.Fire, 2.5), "Energy Source"));
            foundationCircle.AddTalisman(new Talisman(new Element(ElementType.Water, 2.0), "Flow Regulator"));

            // Nested circle in foundation
            var stabilityModule = new MagicCircle("Stability Module", 4.0);
            stabilityModule.AddTalisman(new Talisman(new Element(ElementType.Earth, 1.5), "Anchor Point"));
            stabilityModule.AddTalisman(new Talisman(new Element(ElementType.Metal, 1.0), "Stability Ring"));
            foundationCircle.NestCircle(stabilityModule, 0.6);

            // Layer 1: Amplification layer
            var amplificationCircle = new MagicCircle("Power Amplifier", 12.0) { Layer = 4.0 };
            amplificationCircle.AddTalisman(new Talisman(new Element(ElementType.Lightning, 2.5), "Power Booster"));
            amplificationCircle.AddTalisman(new Talisman(new Element(ElementType.Wind, 1.5), "Energy Circulation"));

            // Nested circle in amplification
            var resonanceModule = new MagicCircle("Resonance Module", 3.0);
            resonanceModule.AddTalisman(new Talisman(new Element(ElementType.Lightning, 1.0), "Resonance Crystal"));
            amplificationCircle.NestCircle(resonanceModule, 0.5);

            // Layer 2: Control and targeting layer
            var controlCircle = new MagicCircle("Quantum Controller", 8.0) { Layer = 8.0 };
            controlCircle.AddTalisman(new Talisman(new Element(ElementType.Light, 2.0), "Targeting Array"));
            controlCircle.AddTalisman(new Talisman(new Element(ElementType.Void, 1.5), "Void Anchor"));

            // Create network connections between layers
            var foundationToAmplifier = foundationCircle.ConnectTo(amplificationCircle, ConnectionType.Flow);
            var amplifierToControl = amplificationCircle.ConnectTo(controlCircle, ConnectionType.Resonance);
            var controlToFoundation = controlCircle.ConnectTo(foundationCircle, ConnectionType.Trigger);

            // Cross-layer nested connections
            var crossResonance = stabilityModule.ConnectTo(resonanceModule, ConnectionType.Resonance);

            Console.WriteLine("🏗️ UNIFIED COMPOSITION STRUCTURE:");
            Console.WriteLine($"Foundation (L0): {foundationCircle}");
            Console.WriteLine($"  └─ Nested: {stabilityModule}");
            Console.WriteLine($"Amplifier (L1): {amplificationCircle}");
            Console.WriteLine($"  └─ Nested: {resonanceModule}");
            Console.WriteLine($"Controller (L2): {controlCircle}");
            Console.WriteLine();

            Console.WriteLine("⚡ NETWORK CONNECTIONS:");
            Console.WriteLine($"  {foundationToAmplifier}");
            Console.WriteLine($"  {amplifierToControl}");
            Console.WriteLine($"  {controlToFoundation}");
            Console.WriteLine($"  Cross-Layer: {crossResonance}");
            Console.WriteLine();

            Console.WriteLine("📊 UNIFIED METRICS:");
            Console.WriteLine($"  Total Composition Circles: {foundationCircle.GetAllCompositionCircles().Count}");
            Console.WriteLine($"  Foundation Power Output: {foundationCircle.GetCompositionPowerOutput():F1}");
            Console.WriteLine($"  Foundation Casting Time: {foundationCircle.CastingTime:F1}s");
            Console.WriteLine($"  Foundation Complexity: {foundationCircle.ComplexityScore:F1}");
            Console.WriteLine($"  Composition Type: {foundationCircle.CompositionType}");
            Console.WriteLine();

            // Analyze each layer separately
            var layers = new[] { foundationCircle, amplificationCircle, controlCircle };
            Console.WriteLine("🔍 LAYER-BY-LAYER ANALYSIS:");
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                Console.WriteLine($"  Layer {i} ({layer.Name}):");
                Console.WriteLine($"    Power: {layer.PowerOutput:F1} | Stability: {layer.Stability:F2} | " +
                                $"Efficiency: {layer.Efficiency:F2}");
                Console.WriteLine($"    Casting Time: {layer.CastingTime:F1}s | Complexity: {layer.ComplexityScore:F1}");
                Console.WriteLine($"    Composition: {layer.CompositionType} | Nested: {layer.NestedCircles.Count} | " +
                                $"Connections: {layer.Connections.Count}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Analyzes casting time scaling across different composition types
        /// </summary>
        private static void DemonstrateCastingTimeAnalysis()
        {
            Console.WriteLine("⏱️ CASTING TIME COMPLEXITY ANALYSIS ⏱️");
            Console.WriteLine("Comparing casting times across composition approaches\n");

            // Create test circles of different complexity levels
            var simpleCircle = CreateSimpleTestCircle();
            var stackedCircles = CreateStackedTestCircles();
            var nestedCircle = CreateNestedTestCircle();
            var networkCircle = CreateNetworkTestCircle();
            var unifiedCircle = CreateUnifiedTestCircle();

            Console.WriteLine("📈 CASTING TIME COMPARISON:");
            Console.WriteLine($"{"Composition Type",-20} {"Complexity",-12} {"Cast Time",-12} {"Power",-10} {"Efficiency",-12}");
            Console.WriteLine(new string('-', 70));

            // Simple
            Console.WriteLine($"{"Simple",-20} {simpleCircle.ComplexityScore,-12:F1} " +
                            $"{simpleCircle.CastingTime,-12:F1}s {simpleCircle.PowerOutput,-10:F1} " +
                            $"{simpleCircle.Efficiency,-12:F2}");

            // Stacked (calculate total)
            var totalStackedTime = stackedCircles.Sum(c => c.CastingTime);
            var totalStackedComplexity = stackedCircles.Sum(c => c.ComplexityScore);
            var totalStackedPower = stackedCircles.Sum(c => c.PowerOutput);
            Console.WriteLine($"{"Stacked (3 layers)",-20} {totalStackedComplexity,-12:F1} " +
                            $"{totalStackedTime,-12:F1}s {totalStackedPower,-10:F1} " +
                            $"{stackedCircles.Average(c => c.Efficiency),-12:F2}");

            // Nested
            Console.WriteLine($"{"Nested",-20} {nestedCircle.ComplexityScore,-12:F1} " +
                            $"{nestedCircle.CastingTime,-12:F1}s {nestedCircle.GetCompositionPowerOutput(),-10:F1} " +
                            $"{nestedCircle.Efficiency,-12:F2}");

            // Network
            Console.WriteLine($"{"Network",-20} {networkCircle.ComplexityScore,-12:F1} " +
                            $"{networkCircle.CastingTime,-12:F1}s {networkCircle.GetCompositionPowerOutput(),-10:F1} " +
                            $"{networkCircle.Efficiency,-12:F2}");

            // Unified
            Console.WriteLine($"{"Unified",-20} {unifiedCircle.ComplexityScore,-12:F1} " +
                            $"{unifiedCircle.CastingTime,-12:F1}s {unifiedCircle.GetCompositionPowerOutput(),-10:F1} " +
                            $"{unifiedCircle.Efficiency,-12:F2}");

            Console.WriteLine();
            Console.WriteLine("💡 CASTING TIME INSIGHTS:");
            Console.WriteLine("• Simple circles are fastest but least powerful");
            Console.WriteLine("• Stacked circles scale linearly in casting time");
            Console.WriteLine("• Nested circles have moderate time with good power efficiency");
            Console.WriteLine("• Network circles balance complexity with power amplification");
            Console.WriteLine("• Unified compositions offer maximum power but require significant time investment");
            Console.WriteLine();

            Console.WriteLine("🎯 STRATEGIC RECOMMENDATIONS:");
            Console.WriteLine("• Use Simple for quick combat spells");
            Console.WriteLine("• Use Stacked for gradually building spell power");
            Console.WriteLine("• Use Nested for compact, efficient spell modules");
            Console.WriteLine("• Use Network for elemental combination effects");
            Console.WriteLine("• Use Unified for ultimate ritual magic and world-changing spells");
        }

        #region Test Circle Creation Methods

        private static MagicCircle CreateSimpleTestCircle()
        {
            var circle = new MagicCircle("Simple Test", 5.0);
            circle.AddTalisman(new Talisman(new Element(ElementType.Fire, 2.0), "Test Fire"));
            return circle;
        }

        private static MagicCircle[] CreateStackedTestCircles()
        {
            var circles = new MagicCircle[3];
            
            circles[0] = new MagicCircle("Stack Base", 6.0) { Layer = 0.0 };
            circles[0].AddTalisman(new Talisman(new Element(ElementType.Fire, 2.0), "Base Fire"));
            
            circles[1] = new MagicCircle("Stack Mid", 5.0) { Layer = 3.0 };
            circles[1].AddTalisman(new Talisman(new Element(ElementType.Fire, 1.5), "Mid Fire"));
            
            circles[2] = new MagicCircle("Stack Top", 4.0) { Layer = 6.0 };
            circles[2].AddTalisman(new Talisman(new Element(ElementType.Metal, 1.0), "Top Focus"));
            
            return circles;
        }

        private static MagicCircle CreateNestedTestCircle()
        {
            var main = new MagicCircle("Nested Main", 8.0);
            main.AddTalisman(new Talisman(new Element(ElementType.Fire, 2.5), "Main Fire"));
            main.AddTalisman(new Talisman(new Element(ElementType.Earth, 1.5), "Main Earth"));
            
            var nested = new MagicCircle("Nested Sub", 3.0);
            nested.AddTalisman(new Talisman(new Element(ElementType.Metal, 1.0), "Nested Metal"));
            
            main.NestCircle(nested);
            return main;
        }

        private static MagicCircle CreateNetworkTestCircle()
        {
            var main = new MagicCircle("Network Main", 6.0);
            main.AddTalisman(new Talisman(new Element(ElementType.Lightning, 2.0), "Main Lightning"));
            
            var connected = new MagicCircle("Network Connected", 5.0);
            connected.AddTalisman(new Talisman(new Element(ElementType.Fire, 1.5), "Connected Fire"));
            
            main.ConnectTo(connected, ConnectionType.Resonance);
            return main;
        }

        private static MagicCircle CreateUnifiedTestCircle()
        {
            var main = new MagicCircle("Unified Main", 10.0) { Layer = 0.0 };
            main.AddTalisman(new Talisman(new Element(ElementType.Fire, 2.5), "Unified Fire"));
            main.AddTalisman(new Talisman(new Element(ElementType.Earth, 2.0), "Unified Earth"));
            
            var nested = new MagicCircle("Unified Nested", 3.0);
            nested.AddTalisman(new Talisman(new Element(ElementType.Metal, 1.5), "Unified Metal"));
            main.NestCircle(nested);
            
            var connected = new MagicCircle("Unified Connected", 6.0) { Layer = 3.0 };
            connected.AddTalisman(new Talisman(new Element(ElementType.Water, 1.8), "Unified Water"));
            
            main.ConnectTo(connected, ConnectionType.Flow);
            return main;
        }

        #endregion
    }
}
