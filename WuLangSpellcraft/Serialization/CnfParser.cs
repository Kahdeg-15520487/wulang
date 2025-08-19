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
                        yield return new CnfToken(CnfTokenType.Equals, "=", position++);
                        break;
                    case '~':
                        yield return new CnfToken(CnfTokenType.Tilde, "~", position++);
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
                        // Check for arrow (->)
                        if (position + 1 < input.Length && input[position + 1] == '>')
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
            
            if (token.Type != CnfTokenType.Identifier || !token.Value.ToUpperInvariant().StartsWith("C"))
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
            var elementRegular = new Element(elementTypeRegular, powerLevel);
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

            var lexer = new CnfLexer();
            var tokens = lexer.Tokenize(cnf).ToList();
            
            // Look for formation markers: circle IDs (id:), connections (~), or multiple circle definitions
            bool hasCircleId = false;
            bool hasConnection = false;
            int circleCount = 0;
            
            foreach (var token in tokens)
            {
                if (token.Type == CnfTokenType.Identifier && token.Value.Contains(':'))
                {
                    hasCircleId = true;
                }
                else if (token.Type == CnfTokenType.Tilde || token.Type == CnfTokenType.Arrow)
                {
                    hasConnection = true;
                }
                else if (token.Type == CnfTokenType.Identifier && token.Value.StartsWith('C') && char.IsDigit(token.Value.Length > 1 ? token.Value[1] : '\0'))
                {
                    circleCount++;
                }
            }

            // Multi-circle if we have circle IDs, connections, or multiple circles
            return hasCircleId || hasConnection || circleCount > 1;
        }

        public SpellFormation ParseFormation(string cnf)
        {
            var lexer = new CnfLexer();
            _tokens = lexer.Tokenize(cnf).ToList();
            _currentToken = 0;

            var formation = new SpellFormation();
            
            // Parse circle definitions and connections
            while (Current().Type != CnfTokenType.EOF)
            {
                if (IsCircleDefinition())
                {
                    var (circleId, circle) = ParseCircleWithId();
                    formation.AddCircle(circleId, circle);
                }
                else if (IsConnectionDefinition())
                {
                    var connection = ParseConnection();
                    formation.AddConnection(connection);
                }
                else
                {
                    throw new CnfException($"Unexpected token '{Current().Value}' at position {Current().Position}", Current().Position);
                }
            }

            return formation;
        }

        private bool IsCircleDefinition()
        {
            var current = Current();
            
            // Check for "circleId: C..." pattern
            if (current.Type == CnfTokenType.Identifier && 
                _currentToken + 1 < _tokens.Count &&
                _tokens[_currentToken + 1].Type == CnfTokenType.Colon &&
                _currentToken + 2 < _tokens.Count &&
                _tokens[_currentToken + 2].Type == CnfTokenType.Identifier &&
                _tokens[_currentToken + 2].Value.ToUpperInvariant().StartsWith("C"))
            {
                return true;
            }
            
            // Check for direct "C..." pattern
            return current.Type == CnfTokenType.Identifier && current.Value.ToUpperInvariant().StartsWith("C");
        }

        private bool IsConnectionDefinition()
        {
            // Look for patterns like "id -> id" or "id = id"
            var current = Current();
            if (current.Type == CnfTokenType.Identifier)
            {
                for (int i = _currentToken + 1; i < _tokens.Count && i < _currentToken + 5; i++)
                {
                    var token = _tokens[i];
                    if (token.Type == CnfTokenType.Arrow || token.Type == CnfTokenType.Equals || 
                        token.Type == CnfTokenType.Tilde || token.Type == CnfTokenType.DoubleArrow)
                    {
                        return true;
                    }
                    if (token.Type == CnfTokenType.EOF || token.Type == CnfTokenType.Identifier && 
                        token.Value.ToUpperInvariant().StartsWith("C"))
                    {
                        break;
                    }
                }
            }
            return false;
        }

        private (string id, MagicCircle circle) ParseCircleWithId()
        {
            string circleId;
            
            // Check if we have an explicit ID
            if (Current().Type == CnfTokenType.Identifier && 
                _currentToken + 1 < _tokens.Count &&
                _tokens[_currentToken + 1].Type == CnfTokenType.Colon)
            {
                circleId = Current().Value;
                Advance(); // Skip ID
                Advance(); // Skip colon
            }
            else
            {
                circleId = Core.Utilities.GenerateShortId(); // Auto-generate ID
            }
            
            var parser = new CnfParser();
            var remainingTokens = _tokens.Skip(_currentToken).ToList();
            var cnf = string.Join(" ", remainingTokens.TakeWhile(t => !IsConnectionStart(t)).Select(t => t.Value));
            
            var circle = parser.ParseCircle(cnf);
            
            // Advance past the circle definition
            while (Current().Type != CnfTokenType.EOF && !IsConnectionStart(Current()) && !IsCircleDefinition())
            {
                Advance();
            }
            
            return (circleId, circle);
        }

        private bool IsConnectionStart(CnfToken token)
        {
            return token.Type == CnfTokenType.Arrow || token.Type == CnfTokenType.Equals || 
                   token.Type == CnfTokenType.Tilde || token.Type == CnfTokenType.DoubleArrow;
        }

        private FormationConnection ParseConnection()
        {
            var sourceId = Current().Value;
            if (Current().Type != CnfTokenType.Identifier)
            {
                throw new CnfException($"Expected source ID at position {Current().Position}", Current().Position);
            }
            Advance();

            // Parse connection type and optional strength
            var connectionType = ParseConnectionType();
            double strength = 1.0;
            
            // Check for strength specification {value}
            if (Current().Type == CnfTokenType.LeftBrace)
            {
                Advance(); // Skip {
                if (Current().Type == CnfTokenType.Number)
                {
                    double.TryParse(Current().Value, NumberStyles.Float, CultureInfo.InvariantCulture, out strength);
                    Advance(); // Skip number
                }
                if (Current().Type == CnfTokenType.RightBrace)
                {
                    Advance(); // Skip }
                }
            }

            var targetId = Current().Value;
            if (Current().Type != CnfTokenType.Identifier)
            {
                throw new CnfException($"Expected target ID at position {Current().Position}", Current().Position);
            }
            Advance();

            return new FormationConnection
            {
                SourceId = sourceId,
                TargetId = targetId,
                Type = connectionType,
                Strength = strength
            };
        }

        private ConnectionType ParseConnectionType()
        {
            var token = Current();
            Advance();
            
            return token.Type switch
            {
                CnfTokenType.Arrow => ConnectionType.Direct,
                CnfTokenType.Equals => ConnectionType.Resonance,
                CnfTokenType.Tilde => ConnectionType.Flow,
                CnfTokenType.DoubleArrow => ConnectionType.Flow, // Bidirectional flow
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
