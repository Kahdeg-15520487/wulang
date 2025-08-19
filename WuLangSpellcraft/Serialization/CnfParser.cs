using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Serialization
{
    /// <summary>
    /// Lexical analyzer for CNF
    /// </summary>
    public class CnfLexer
    {
        public IEnumerable<CnfToken> Tokenize(string input)
        {
            var position = 0;
            
            while (position < input.Length)
            {
                var current = input[position];
                
                if (char.IsWhiteSpace(current))
                {
                    position++;
                    continue;
                }
                
                switch (current)
                {
                    case ':':
                        yield return new CnfToken(CnfTokenType.Colon, ":", position++);
                        break;
                    case ';':
                        yield return new CnfToken(CnfTokenType.Semicolon, ";", position++);
                        break;
                    case '(':
                        yield return new CnfToken(CnfTokenType.LeftParen, "(", position++);
                        break;
                    case ')':
                        yield return new CnfToken(CnfTokenType.RightParen, ")", position++);
                        break;
                    case '[':
                        yield return new CnfToken(CnfTokenType.LeftBracket, "[", position++);
                        break;
                    case ']':
                        yield return new CnfToken(CnfTokenType.RightBracket, "]", position++);
                        break;
                    case '{':
                        yield return new CnfToken(CnfTokenType.LeftBrace, "{", position++);
                        break;
                    case '}':
                        yield return new CnfToken(CnfTokenType.RightBrace, "}", position++);
                        break;
                    case ',':
                        yield return new CnfToken(CnfTokenType.Comma, ",", position++);
                        break;
                    case '@':
                        yield return new CnfToken(CnfTokenType.At, "@", position++);
                        break;
                    case '=':
                        // Check for double equals (==)
                        if (position + 1 < input.Length && input[position + 1] == '=')
                        {
                            yield return new CnfToken(CnfTokenType.DoubleEquals, "==", position);
                            position += 2;
                        }
                        else
                        {
                            yield return new CnfToken(CnfTokenType.Equals, "=", position++);
                        }
                        break;
                    case '~':
                        // Check for double tilde (~~) or unstable connection (~=)
                        if (position + 1 < input.Length && input[position + 1] == '~')
                        {
                            yield return new CnfToken(CnfTokenType.DoubleTilde, "~~", position);
                            position += 2;
                        }
                        else if (position + 1 < input.Length && input[position + 1] == '=')
                        {
                            yield return new CnfToken(CnfTokenType.TildeEquals, "~=", position);
                            position += 2;
                        }
                        else
                        {
                            yield return new CnfToken(CnfTokenType.Tilde, "~", position++);
                        }
                        break;
                    case '+':
                        yield return new CnfToken(CnfTokenType.Plus, "+", position++);
                        break;
                    case '*':
                        yield return new CnfToken(CnfTokenType.Star, "*", position++);
                        break;
                    case '?':
                        yield return new CnfToken(CnfTokenType.Question, "?", position++);
                        break;
                    case '!':
                        yield return new CnfToken(CnfTokenType.Exclamation, "!", position++);
                        break;
                    case '-':
                        // Check for double minus (--) or arrow (->)
                        if (position + 1 < input.Length && input[position + 1] == '-')
                        {
                            yield return new CnfToken(CnfTokenType.DoubleMinus, "--", position);
                            position += 2;
                        }
                        else if (position + 1 < input.Length && input[position + 1] == '>')
                        {
                            yield return new CnfToken(CnfTokenType.Arrow, "->", position);
                            position += 2;
                        }
                        else
                        {
                            yield return new CnfToken(CnfTokenType.Minus, "-", position++);
                        }
                        break;
                    case '<':
                        // Check for double arrow (<->)
                        if (position + 2 < input.Length && input[position + 1] == '-' && input[position + 2] == '>')
                        {
                            yield return new CnfToken(CnfTokenType.DoubleArrow, "<->", position);
                            position += 3;
                        }
                        else
                        {
                            throw new CnfException($"Unexpected character '{current}' at position {position}", position);
                        }
                        break;
                    default:
                        if (char.IsDigit(current) || current == '.')
                        {
                            var (number, newPos) = Core.Utilities.ReadNumber(input, position);
                            yield return new CnfToken(CnfTokenType.Number, number, position);
                            position = newPos;
                        }
                        else if (char.IsLetter(current) || current == '_')
                        {
                            var (identifier, newPos) = Core.Utilities.ReadIdentifier(input, position);
                            yield return new CnfToken(CnfTokenType.Identifier, identifier, position);
                            position = newPos;
                        }
                        else
                        {
                            throw new CnfException($"Unexpected character '{current}' at position {position}", position);
                        }
                        break;
                }
            }
            
            yield return new CnfToken(CnfTokenType.EOF, "", position);
        }
    }

    /// <summary>
    /// Parser for Circle Notation Format
    /// </summary>
    public class CnfParser
    {
        private List<CnfToken> _tokens = new();
        private int _currentToken = 0;

        public MagicCircle ParseCircle(string cnf)
        {
            var lexer = new CnfLexer();
            _tokens = lexer.Tokenize(cnf).ToList();
            _currentToken = 0;

            return ParseCircleDefinition();
        }

        private MagicCircle ParseCircleDefinition()
        {
            // Expect: C<radius> <elements>[@<center>]
            var token = Current();
            
            if (token.Type != CnfTokenType.Identifier || !token.Value.StartsWith("C"))
            {
                throw new CnfException($"Expected circle definition 'C<radius>' at position {token.Position}", token.Position);
            }

            // Extract radius
            var radiusStr = token.Value[1..]; // Remove 'C' prefix
            if (!double.TryParse(radiusStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var radius))
            {
                throw new CnfException($"Invalid radius '{radiusStr}' at position {token.Position}", token.Position);
            }

            Advance(); // Move past circle definition

            // Create circle
            var circle = new MagicCircle($"Circle R{radius}", radius);

            // Parse elements
            while (Current().Type != CnfTokenType.EOF && Current().Type != CnfTokenType.At)
            {
                var talisman = ParseTalisman();
                if (talisman != null)
                {
                    // Calculate position angle based on talisman index
                    var angle = (2 * Math.PI * circle.Talismans.Count) / 8; // Assume 8 positions for now
                    circle.AddTalisman(talisman, angle);
                }
            }

            // Check for center element (@symbol)
            if (Current().Type == CnfTokenType.At)
            {
                Advance(); // Skip @
                var centerTalisman = ParseTalisman();
                if (centerTalisman != null)
                {
                    circle.SetCenterTalisman(centerTalisman);
                }
            }

            return circle;
        }

        private Talisman? ParseTalisman()
        {
            var token = Current();
            
            if (token.Type != CnfTokenType.Identifier)
            {
                return null;
            }

            // Check if this is a compact format like "F2.5W1.2L0.8"
            if (token.Value.Length > 1)
            {
                // Look for element letters in the token
                var elementPositions = new List<int>();
                for (int i = 0; i < token.Value.Length; i++)
                {
                    if (ElementSymbols.IsValidSymbol(token.Value[i]))
                    {
                        elementPositions.Add(i);
                    }
                }

                // If we have multiple elements in one token, split and requeue
                if (elementPositions.Count > 1)
                {
                    var remainingTokens = new List<CnfToken>();
                    
                    for (int i = 0; i < elementPositions.Count; i++)
                    {
                        var start = elementPositions[i];
                        var end = i + 1 < elementPositions.Count ? elementPositions[i + 1] : token.Value.Length;
                        var elementPart = token.Value[start..end];
                        
                        remainingTokens.Add(new CnfToken(CnfTokenType.Identifier, elementPart, token.Position + start));
                    }
                    
                    // Replace current token and insert remaining ones
                    _tokens[_currentToken] = remainingTokens[0];
                    for (int i = 1; i < remainingTokens.Count; i++)
                    {
                        _tokens.Insert(_currentToken + i, remainingTokens[i]);
                    }
                    
                    // Re-parse with the first split token
                    token = Current();
                }
            }

            // Handle simple compact format (all element symbols, like "FWEMO")
            if (token.Value.Length > 1 && token.Value.All(ElementSymbols.IsValidSymbol))
            {
                // This is simple compact format - parse first element and modify token
                var firstSymbol = token.Value[0];
                var elementType = ElementSymbols.GetElementType(firstSymbol);
                
                // Update the token to contain the remaining symbols
                if (token.Value.Length > 1)
                {
                    _tokens[_currentToken] = token with { Value = token.Value[1..] };
                }
                else
                {
                    Advance();
                }
                
                // Create element and talisman for first symbol
                var element = new Element(elementType, 1.0);
                var talisman = new Talisman(element, $"Talisman {elementType}");
                
                return talisman;
            }

            // Regular parsing for single elements with potential power levels and IDs
            var elementSymbol = token.Value[0];
            if (!ElementSymbols.IsValidSymbol(elementSymbol))
            {
                throw new CnfException($"Invalid element symbol '{elementSymbol}' at position {token.Position}", token.Position);
            }

            var elementTypeRegular = ElementSymbols.GetElementType(elementSymbol);
            var powerLevel = 1.0;
            var elementState = ElementState.Normal;
            string? talismanId = null;

            // Check if there's a power level in the same token (e.g., "F2.5")
            if (token.Value.Length > 1)
            {
                var remaining = token.Value[1..];
                
                // Try to parse the entire remaining string as a number
                if (double.TryParse(remaining, NumberStyles.Float, CultureInfo.InvariantCulture, out var power))
                {
                    powerLevel = power;
                }
            }

            Advance(); // Move past element token

            // Check for element state symbols (?, !, ~)
            if (Current().Type == CnfTokenType.Question)
            {
                elementState = ElementState.Unstable;
                Advance();
            }
            else if (Current().Type == CnfTokenType.Exclamation)
            {
                elementState = ElementState.Damaged;
                Advance();
            }
            else if (Current().Type == CnfTokenType.Tilde)
            {
                // In single circle parsing, ~ immediately after an element is always an element state
                elementState = ElementState.Resonating;
                Advance();
            }

            // Check for power level as separate number token
            if (Current().Type == CnfTokenType.Number)
            {
                if (double.TryParse(Current().Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var separatePower))
                {
                    powerLevel = separatePower;
                }
                Advance();
            }

            // Check for talisman ID
            if (Current().Type == CnfTokenType.Colon)
            {
                Advance(); // Skip colon
                
                if (Current().Type == CnfTokenType.Identifier)
                {
                    talismanId = Current().Value;
                    Advance();
                }
                else
                {
                    throw new CnfException($"Expected talisman ID after ':' at position {Current().Position}", Current().Position);
                }
            }

            // Create element and talisman
            var elementRegular = new Element(elementTypeRegular, powerLevel, elementState);
            var talismanName = talismanId ?? $"Talisman {elementTypeRegular}";
            var talismanRegular = new Talisman(elementRegular, talismanName);

            return talismanRegular;
        }

        private CnfToken Current()
        {
            return _currentToken < _tokens.Count ? _tokens[_currentToken] : new CnfToken(CnfTokenType.EOF, "", -1);
        }

        private void Advance()
        {
            if (_currentToken < _tokens.Count)
            {
                _currentToken++;
            }
        }
    }

    /// <summary>
    /// Enhanced parser for multi-circle CNF with ID-based connections
    /// </summary>
    public class MultiCircleCnfParser
    {
        private List<CnfToken> _tokens = new();
        private int _currentToken = 0;

        /// <summary>
        /// Detects if CNF string contains multi-circle formation markers
        /// </summary>
        public bool IsMultiCircleFormat(string cnf)
        {
            if (string.IsNullOrWhiteSpace(cnf))
                return false;

            // Look for bracket syntax [id:circle] or 2-character connection symbols
            bool hasBrackets = cnf.Contains('[') && cnf.Contains(']');
            bool hasConnections = cnf.Contains("--") || cnf.Contains("==") || cnf.Contains("~~") || 
                                 cnf.Contains("~=") || cnf.Contains("->") || cnf.Contains("<->");
            
            return hasBrackets || hasConnections;
        }

        public Formation ParseFormation(string cnf)
        {
            var lexer = new CnfLexer();
            _tokens = lexer.Tokenize(cnf).ToList();
            _currentToken = 0;

            // Debug: Print all tokens
            Console.WriteLine($"Debug: Parsed {_tokens.Count} tokens:");
            for (int i = 0; i < _tokens.Count; i++)
            {
                Console.WriteLine($"  {i}: {_tokens[i].Type} = '{_tokens[i].Value}'");
            }

            var formation = new Formation("Parsed Formation", "Formation parsed from CNF");
            var circles = new Dictionary<string, MagicCircle>();

            // Phase 1: Parse all circle definitions
            while (Current().Type != CnfTokenType.EOF)
            {
                Console.WriteLine($"Debug: Phase 1 - Current token: {Current().Type} = '{Current().Value}'");
                if (Current().Type == CnfTokenType.LeftBracket)
                {
                    var (id, circle) = ParseBracketedCircle();
                    circles[id] = circle;
                    formation.AddCircle(id, circle);
                }
                else if (IsConnectionStart())
                {
                    Console.WriteLine("Debug: Found connection start, breaking from Phase 1");
                    // We've reached connections, break out of circle parsing
                    break;
                }
                else if (IsSimpleCircle())
                {
                    // Handle single circle without brackets
                    var circle = ParseSingleCircle();
                    var id = Core.Utilities.GenerateShortId();
                    circles[id] = circle;
                    formation.AddCircle(id, circle);
                    break; // Single circle, no connections expected
                }
                else
                {
                    Console.WriteLine($"Debug: Skipping unexpected token: {Current().Type} = '{Current().Value}'");
                    Advance(); // Skip unexpected tokens
                }
            }

            // Phase 2: Parse connections
            Console.WriteLine("Debug: Starting Phase 2 - connections");
            while (Current().Type != CnfTokenType.EOF)
            {
                Console.WriteLine($"Debug: Phase 2 - Current token: {Current().Type} = '{Current().Value}'");
                if (IsConnectionStart())
                {
                    Console.WriteLine("Debug: Parsing connection");
                    var connection = ParseConnection();
                    formation.AddConnection(connection);
                }
                else
                {
                    Console.WriteLine($"Debug: Skipping non-connection token: {Current().Type} = '{Current().Value}'");
                    Advance(); // Skip unexpected tokens
                }
            }

            return formation;
        }

        private (string id, MagicCircle circle) ParseBracketedCircle()
        {
            // Expect: [id:circle] format
            if (Current().Type != CnfTokenType.LeftBracket)
            {
                throw new CnfException($"Expected '[' at position {Current().Position}", Current().Position);
            }
            Advance(); // Skip [

            // Parse circle ID
            if (Current().Type != CnfTokenType.Identifier)
            {
                throw new CnfException($"Expected circle ID at position {Current().Position}", Current().Position);
            }
            string circleId = Current().Value;
            Advance();

            // Expect colon
            if (Current().Type != CnfTokenType.Colon)
            {
                throw new CnfException($"Expected ':' after circle ID at position {Current().Position}", Current().Position);
            }
            Advance(); // Skip :

            // Parse circle definition - collect tokens until we hit ]
            var circleTokens = new List<CnfToken>();
            while (Current().Type != CnfTokenType.RightBracket && Current().Type != CnfTokenType.EOF)
            {
                circleTokens.Add(Current());
                Advance();
            }

            if (Current().Type != CnfTokenType.RightBracket)
            {
                throw new CnfException($"Expected ']' to close circle definition at position {Current().Position}", Current().Position);
            }
            Advance(); // Skip ]

            // Parse the circle from collected tokens
            var circleText = ReconstructCircleText(circleTokens);
            var parser = new CnfParser();
            var circle = parser.ParseCircle(circleText);

            return (circleId, circle);
        }

        private string ReconstructCircleText(List<CnfToken> tokens)
        {
            var result = new List<string>();
            
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                
                // Check if this is an element symbol followed by a state symbol
                if (token.Type == CnfTokenType.Identifier && 
                    token.Value.Length == 1 && 
                    ElementSymbols.IsValidSymbol(token.Value[0]) &&
                    i + 1 < tokens.Count)
                {
                    var nextToken = tokens[i + 1];
                    if (nextToken.Type == CnfTokenType.Star || 
                        nextToken.Type == CnfTokenType.Question || 
                        nextToken.Type == CnfTokenType.Exclamation || 
                        nextToken.Type == CnfTokenType.Tilde)
                    {
                        // Combine element and state
                        result.Add(token.Value + nextToken.Value);
                        i++; // Skip the next token since we've consumed it
                        continue;
                    }
                }
                
                result.Add(token.Value);
            }
            
            return string.Join(" ", result);
        }

        private bool IsSimpleCircle()
        {
            var current = Current();
            return current.Type == CnfTokenType.Identifier && 
                   current.Value.StartsWith("C");
        }

        private MagicCircle ParseSingleCircle()
        {
            // Collect all remaining tokens for single circle parsing
            var tokens = new List<CnfToken>();
            while (Current().Type != CnfTokenType.EOF && !IsConnectionToken(Current()))
            {
                tokens.Add(Current());
                Advance();
            }

            var circleText = string.Join(" ", tokens.Select(t => t.Value));
            var parser = new CnfParser();
            return parser.ParseCircle(circleText);
        }

        private bool IsConnectionToken(CnfToken token)
        {
            return token.Type == CnfTokenType.DoubleMinus ||    // --
                   token.Type == CnfTokenType.DoubleEquals ||   // ==
                   token.Type == CnfTokenType.DoubleTilde ||    // ~~
                   token.Type == CnfTokenType.TildeEquals ||    // ~=
                   token.Type == CnfTokenType.Arrow ||          // ->
                   token.Type == CnfTokenType.DoubleArrow;      // <->
        }

        private bool IsConnectionStart()
        {
            // Look for pattern: identifier connection_symbol identifier
            if (Current().Type != CnfTokenType.Identifier)
                return false;
            
            if (_currentToken + 1 >= _tokens.Count)
                return false;
                
            var nextToken = _tokens[_currentToken + 1];
            return IsConnectionToken(nextToken);
        }

        private FormationConnection ParseConnection()
        {
            // Parse: sourceId connectionType targetId
            if (Current().Type != CnfTokenType.Identifier)
            {
                throw new CnfException($"Expected source ID at position {Current().Position}", Current().Position);
            }
            var sourceId = Current().Value;
            Advance();

            // Parse connection type
            var connectionType = ParseConnectionType();

            // Parse target ID
            if (Current().Type != CnfTokenType.Identifier)
            {
                throw new CnfException($"Expected target ID at position {Current().Position}", Current().Position);
            }
            var targetId = Current().Value;
            Advance();

            return new FormationConnection
            {
                SourceId = sourceId,
                TargetId = targetId,
                Type = connectionType,
                Strength = 1.0
            };
        }

        private ConnectionType ParseConnectionType()
        {
            var token = Current();
            Advance();
            
            return token.Type switch
            {
                CnfTokenType.DoubleMinus => ConnectionType.Basic,          // -- = basic connection (simple energy flow)
                CnfTokenType.DoubleEquals => ConnectionType.Strong,        // == = strong connection (amplified energy flow)
                CnfTokenType.DoubleTilde => ConnectionType.Harmonic,       // ~~ = harmonic connection (resonant frequency)
                CnfTokenType.TildeEquals => ConnectionType.Unstable,       // ~= = unstable connection (fluctuating energy)
                CnfTokenType.Arrow => ConnectionType.Directional,          // -> = directional flow (one-way)
                CnfTokenType.DoubleArrow => ConnectionType.Bidirectional,  // <-> = bidirectional flow (two-way)
                _ => throw new CnfException($"Invalid connection type '{token.Value}' at position {token.Position}", token.Position)
            };
        }

        private CnfToken Current()
        {
            return _currentToken < _tokens.Count ? _tokens[_currentToken] : new CnfToken(CnfTokenType.EOF, "", -1);
        }

        private void Advance()
        {
            if (_currentToken < _tokens.Count)
            {
                _currentToken++;
            }
        }
    }
}
