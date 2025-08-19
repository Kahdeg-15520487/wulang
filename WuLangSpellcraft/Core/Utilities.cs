using System;
using System.Linq;
using System.Collections.Generic;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Static utility methods used throughout the WuLangSpellcraft library
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Generates a short, human-readable ID
        /// </summary>
        public static string GenerateShortId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Validates if a string is a valid CNF identifier
        /// </summary>
        public static bool IsValidCnfId(string id)
        {
            return !string.IsNullOrWhiteSpace(id) && 
                   id.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-') &&
                   id.Length <= 16;
        }

        /// <summary>
        /// Reads a number from a string starting at the specified position
        /// </summary>
        public static (string number, int newPosition) ReadNumber(string input, int startPos)
        {
            var endPos = startPos;
            var hasDecimal = false;
            
            while (endPos < input.Length)
            {
                var c = input[endPos];
                if (char.IsDigit(c))
                {
                    endPos++;
                }
                else if (c == '.' && !hasDecimal)
                {
                    hasDecimal = true;
                    endPos++;
                }
                else
                {
                    break;
                }
            }
            
            return (input[startPos..endPos], endPos);
        }

        /// <summary>
        /// Reads an identifier from a string starting at the specified position
        /// </summary>
        public static (string identifier, int newPosition) ReadIdentifier(string input, int startPos)
        {
            var endPos = startPos;
            
            while (endPos < input.Length)
            {
                var c = input[endPos];
                if (char.IsLetter(c) || c == '_' || 
                    (endPos > startPos && (char.IsDigit(c) || c == '.'))) // Allow numbers and dots after first character
                {
                    endPos++;
                }
                else
                {
                    break;
                }
            }
            
            return (input[startPos..endPos], endPos);
        }

        /// <summary>
        /// Element type to symbol mapping for CNF serialization
        /// </summary>
        public static readonly Dictionary<ElementType, char> ElementTypeToSymbol = new()
        {
            { ElementType.Fire, 'F' },
            { ElementType.Water, 'W' },
            { ElementType.Earth, 'E' },
            { ElementType.Metal, 'M' },
            { ElementType.Wood, 'O' },  // 'O' for wOod to avoid conflict with Water
            { ElementType.Lightning, 'L' },
            { ElementType.Wind, 'N' },  // wiNd
            { ElementType.Light, 'I' }, // lIght
            { ElementType.Dark, 'D' },
            { ElementType.Forge, 'G' }, // forGe
            { ElementType.Chaos, 'C' },
            { ElementType.Void, 'V' }
        };

        /// <summary>
        /// Symbol to element type mapping for CNF deserialization
        /// </summary>
        public static readonly Dictionary<char, ElementType> SymbolToElementType = 
            ElementTypeToSymbol.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        /// <summary>
        /// Gets the CNF symbol for an element type
        /// </summary>
        public static char GetElementSymbol(ElementType type)
        {
            return ElementTypeToSymbol.TryGetValue(type, out var symbol) ? symbol : '?';
        }

        /// <summary>
        /// Gets the element type from a CNF symbol
        /// </summary>
        public static ElementType GetElementType(char symbol)
        {
            return SymbolToElementType.TryGetValue(char.ToUpperInvariant(symbol), out var type) ? type : ElementType.Fire;
        }

        /// <summary>
        /// Validates if a character is a valid element symbol
        /// </summary>
        public static bool IsValidElementSymbol(char symbol)
        {
            return SymbolToElementType.ContainsKey(char.ToUpperInvariant(symbol));
        }
    }
}
