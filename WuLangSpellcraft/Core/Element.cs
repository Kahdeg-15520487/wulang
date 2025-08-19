using System;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Represents the expanded elemental system with base Wu Xing elements and derived interactions
    /// </summary>
    public enum ElementType
    {
        // Base Elements (Wu Xing)
        Water,  // 水 - Flow control, data streams
        Fire,   // 火 - Actions, transformations
        Earth,  // 土 - Storage, stability
        Metal,  // 金 - Logic, precision
        Wood,   // 木 - Growth, iteration
        
        // Derived Elements (Interactions)
        Lightning, // 雷 - Fire + Metal - Sudden directed energy
        Wind,      // 風 - Wood + Water - Flowing adaptive movement
        Light,     // 光 - Fire + Wood - Life-giving illumination
        Dark,      // 闇 - Earth + Water - Shadows and hidden knowledge
        Forge,     // 鍛 - Metal + Wood - Transformative craft
        Chaos,     // 混沌 - All elements - Unpredictable raw potential
        Void       // 虛無 - No elements - Infinite potential and balance
    }

    /// <summary>
    /// Represents the relationship between two elements
    /// </summary>
    public enum ElementRelation
    {
        Generates,  // Generative cycle (生)
        Destroys,   // Destructive cycle (克)
        Neutral     // No strong relationship
    }

    /// <summary>
    /// Represents special states of elements in CNF notation
    /// </summary>
    public enum ElementState
    {
        Normal,     // Default state (no symbol)
        Active,     // * = active/charged element
        Unstable,   // ? = unstable element  
        Damaged,    // ! = damaged element
        Resonating  // ~ = resonating element
    }

    /// <summary>
    /// Core element class representing a single elemental force
    /// </summary>
    public class Element
    {
        public ElementType Type { get; }
        public string Name { get; }
        public string ChineseName { get; }
        public ConsoleColor Color { get; }
        public double Energy { get; set; }
        public bool IsActive { get; set; }
        public ElementState State { get; set; }

        public Element(ElementType type, double energy = 1.0, ElementState state = ElementState.Normal)
        {
            Type = type;
            Energy = energy;
            IsActive = true;
            State = state;
            
            (Name, ChineseName, Color) = type switch
            {
                // Base Elements
                ElementType.Water => ("Water", "水", ConsoleColor.Cyan),
                ElementType.Fire => ("Fire", "火", ConsoleColor.Red),
                ElementType.Earth => ("Earth", "土", ConsoleColor.Yellow),
                ElementType.Metal => ("Metal", "金", ConsoleColor.White),
                ElementType.Wood => ("Wood", "木", ConsoleColor.Green),
                
                // Derived Elements
                ElementType.Lightning => ("Lightning", "雷", ConsoleColor.Magenta),
                ElementType.Wind => ("Wind", "風", ConsoleColor.Gray),
                ElementType.Light => ("Light", "光", ConsoleColor.DarkYellow),
                ElementType.Dark => ("Dark", "闇", ConsoleColor.DarkBlue),
                ElementType.Forge => ("Forge", "鍛", ConsoleColor.DarkGray),
                ElementType.Chaos => ("Chaos", "混沌", ConsoleColor.DarkRed),
                ElementType.Void => ("Void", "虛無", ConsoleColor.Black),
                _ => throw new ArgumentException($"Unknown element type: {type}")
            };
        }

        /// <summary>
        /// Checks if this element is a base Wu Xing element
        /// </summary>
        public bool IsBaseElement()
        {
            return Type is ElementType.Water or ElementType.Fire or ElementType.Earth 
                        or ElementType.Metal or ElementType.Wood;
        }

        /// <summary>
        /// Checks if this element is a derived element
        /// </summary>
        public bool IsDerivedElement()
        {
            return !IsBaseElement();
        }

        /// <summary>
        /// Gets the source elements for a derived element
        /// </summary>
        public static (ElementType?, ElementType?) GetSourceElements(ElementType derivedElement)
        {
            return derivedElement switch
            {
                ElementType.Lightning => (ElementType.Fire, ElementType.Metal),
                ElementType.Wind => (ElementType.Wood, ElementType.Water),
                ElementType.Light => (ElementType.Fire, ElementType.Wood),
                ElementType.Dark => (ElementType.Earth, ElementType.Water),
                ElementType.Forge => (ElementType.Metal, ElementType.Wood),
                ElementType.Chaos => (null, null), // Special case - all elements
                ElementType.Void => (null, null),  // Special case - no elements
                _ => (null, null) // Base elements have no sources
            };
        }

        /// <summary>
        /// Attempts to create a derived element from two base elements
        /// </summary>
        public static ElementType? TryCreateDerivedElement(ElementType element1, ElementType element2)
        {
            // Sort elements for consistent checking
            var (first, second) = element1 < element2 ? (element1, element2) : (element2, element1);
            
            return (first, second) switch
            {
                (ElementType.Fire, ElementType.Metal) => ElementType.Lightning,
                (ElementType.Water, ElementType.Wood) => ElementType.Wind,
                (ElementType.Fire, ElementType.Wood) => ElementType.Light,
                (ElementType.Earth, ElementType.Water) => ElementType.Dark,
                (ElementType.Metal, ElementType.Wood) => ElementType.Forge,
                _ => null
            };
        }

        /// <summary>
        /// Checks if this element can combine with another to create a derived element
        /// </summary>
        public ElementType? CanCombineWith(Element other)
        {
            return TryCreateDerivedElement(this.Type, other.Type);
        }

        /// <summary>
        /// Determines the relationship between this element and another
        /// </summary>
        public ElementRelation GetRelationTo(Element other)
        {
            return GetElementRelation(this.Type, other.Type);
        }

        /// <summary>
        /// Static method to get relationship between element types
        /// </summary>
        public static ElementRelation GetElementRelation(ElementType source, ElementType target)
        {
            // Handle special cases for Chaos and Void
            if (source == ElementType.Chaos || target == ElementType.Chaos)
            {
                // Chaos disrupts everything except Void
                return target == ElementType.Void ? ElementRelation.Destroys : ElementRelation.Destroys;
            }
            
            if (source == ElementType.Void || target == ElementType.Void)
            {
                // Void neutralizes everything except Chaos
                return source == ElementType.Chaos || target == ElementType.Chaos ? ElementRelation.Destroys : ElementRelation.Neutral;
            }

            // Traditional Wu Xing cycles for base elements
            var generativeMap = new (ElementType source, ElementType target)[]
            {
                // Base Wu Xing generative cycle
                (ElementType.Water, ElementType.Wood),
                (ElementType.Wood, ElementType.Fire),
                (ElementType.Fire, ElementType.Earth),
                (ElementType.Earth, ElementType.Metal),
                (ElementType.Metal, ElementType.Water),
                
                // Derived element interactions
                (ElementType.Lightning, ElementType.Fire), // Lightning ignites
                (ElementType.Wind, ElementType.Fire),      // Wind spreads fire
                (ElementType.Light, ElementType.Earth),    // Light warms earth
                (ElementType.Fire, ElementType.Lightning), // Fire creates lightning
                (ElementType.Metal, ElementType.Lightning), // Metal conducts lightning
                (ElementType.Wood, ElementType.Wind),      // Wood creates wind movement
                (ElementType.Water, ElementType.Wind),     // Water creates wind flow
                (ElementType.Forge, ElementType.Metal),    // Forge shapes metal
                (ElementType.Forge, ElementType.Earth),    // Forge hardens earth
            };

            var destructiveMap = new (ElementType source, ElementType target)[]
            {
                // Base Wu Xing destructive cycle
                (ElementType.Water, ElementType.Fire),
                (ElementType.Fire, ElementType.Metal),
                (ElementType.Metal, ElementType.Wood),
                (ElementType.Wood, ElementType.Earth),
                (ElementType.Earth, ElementType.Water),
                
                // Derived element conflicts
                (ElementType.Lightning, ElementType.Dark), // Lightning illuminates dark
                (ElementType.Light, ElementType.Dark),     // Light banishes dark
                (ElementType.Dark, ElementType.Light),     // Dark conceals light
                (ElementType.Wind, ElementType.Earth),     // Wind erodes earth
                (ElementType.Lightning, ElementType.Water), // Lightning splits water
                (ElementType.Wind, ElementType.Light),     // Wind scatters light
            };

            foreach (var (src, tgt) in generativeMap)
            {
                if (source == src && target == tgt)
                    return ElementRelation.Generates;
            }

            foreach (var (src, tgt) in destructiveMap)
            {
                if (source == src && target == tgt)
                    return ElementRelation.Destroys;
            }

            return ElementRelation.Neutral;
        }

        public override string ToString()
        {
            return $"{ChineseName} {Name} (Energy: {Energy:F1})";
        }
    }
}
