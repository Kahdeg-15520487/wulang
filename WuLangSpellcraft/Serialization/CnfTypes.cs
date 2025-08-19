using System;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Serialization
{
    /// <summary>
    /// Options for Circle Notation Format serialization
    /// </summary>
    public class CnfOptions
    {
        // Formatting options
        public bool IncludePowerLevels { get; set; } = true;
        public bool IncludePositions { get; set; } = false;
        public bool IncludeElementStates { get; set; } = false;
        public bool IncludeTalismanIds { get; set; } = false;
        public bool UseCompactFormat { get; set; } = false;
        public bool UseReadableIds { get; set; } = true;
        
        // Layout options
        public bool MultiLine { get; set; } = false;
        public string IndentString { get; set; } = "  ";
        public int MaxLineLength { get; set; } = 80;
        
        // Content options
        public bool IncludeComments { get; set; } = false;
        public bool ValidateOnSerialize { get; set; } = true;
    }

    /// <summary>
    /// Element symbol mappings for CNF
    /// </summary>
    public static class ElementSymbols
    {
        public static char GetSymbol(ElementType type)
        {
            return Core.Utilities.GetElementSymbol(type);
        }

        public static ElementType GetElementType(char symbol)
        {
            return Core.Utilities.GetElementType(symbol);
        }

        public static bool IsValidSymbol(char symbol)
        {
            return Core.Utilities.IsValidElementSymbol(symbol);
        }
    }

    /// <summary>
    /// Exception thrown when CNF parsing fails
    /// </summary>
    public class CnfException : Exception
    {
        public int Position { get; }
        
        public CnfException(string message, int position = -1) : base(message)
        {
            Position = position;
        }
        
        public CnfException(string message, int position, Exception innerException) 
            : base(message, innerException)
        {
            Position = position;
        }
    }

    /// <summary>
    /// Token types for CNF lexical analysis
    /// </summary>
    public enum CnfTokenType
    {
        Identifier,     // Element symbols (F, W, E, etc.), talisman IDs, circle IDs
        Number,         // Numbers for positions, strengths, power levels
        Colon,          // :
        Semicolon,      // ;
        LeftParen,      // (
        RightParen,     // )
        LeftBracket,    // [
        RightBracket,   // ]
        LeftBrace,      // {
        RightBrace,     // }
        Comma,          // ,
        At,             // @
        Arrow,          // ->
        DoubleArrow,    // <->
        Equals,         // =
        Tilde,          // ~
        Plus,           // +
        Minus,          // -
        Star,           // * (for element states)
        Question,       // ? (for element states)
        Exclamation,    // ! (for element states)
        EOF
    }

    /// <summary>
    /// Token for CNF lexical analysis
    /// </summary>
    public record CnfToken(CnfTokenType Type, string Value, int Position);
}
