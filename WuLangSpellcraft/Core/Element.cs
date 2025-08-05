using System;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Represents the five elements of Wu Xing system
    /// </summary>
    public enum ElementType
    {
        Water,  // 水 - Flow control, data streams
        Fire,   // 火 - Actions, transformations
        Earth,  // 土 - Storage, stability
        Metal,  // 金 - Logic, precision
        Wood    // 木 - Growth, iteration
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

        public Element(ElementType type, double energy = 1.0)
        {
            Type = type;
            Energy = energy;
            IsActive = true;
            
            (Name, ChineseName, Color) = type switch
            {
                ElementType.Water => ("Water", "水", ConsoleColor.Cyan),
                ElementType.Fire => ("Fire", "火", ConsoleColor.Red),
                ElementType.Earth => ("Earth", "土", ConsoleColor.Yellow),
                ElementType.Metal => ("Metal", "金", ConsoleColor.White),
                ElementType.Wood => ("Wood", "木", ConsoleColor.Green),
                _ => throw new ArgumentException($"Unknown element type: {type}")
            };
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
            // Generative cycle (生): Water→Wood→Fire→Earth→Metal→Water
            var generativeMap = new (ElementType source, ElementType target)[]
            {
                (ElementType.Water, ElementType.Wood),
                (ElementType.Wood, ElementType.Fire),
                (ElementType.Fire, ElementType.Earth),
                (ElementType.Earth, ElementType.Metal),
                (ElementType.Metal, ElementType.Water)
            };

            // Destructive cycle (克): Water→Fire→Metal→Wood→Earth→Water
            var destructiveMap = new (ElementType source, ElementType target)[]
            {
                (ElementType.Water, ElementType.Fire),
                (ElementType.Fire, ElementType.Metal),
                (ElementType.Metal, ElementType.Wood),
                (ElementType.Wood, ElementType.Earth),
                (ElementType.Earth, ElementType.Water)
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
