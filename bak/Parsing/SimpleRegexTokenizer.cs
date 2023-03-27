using System.Text.RegularExpressions;

namespace BinaryTemplate
{
    public enum TokenType
    {
        BracketLeft,
        BracketRight,
        MiddleBracketLeft,
        MiddleBracketRight,
        CurlyBraceLeft,
        CurlyBraceRight,
        StrText,
        WhiteSpace,
        Define,
        Include,
        Typedef,
        Struct,
        Colon,
        SemiColon,
        Comma,
        Dot,
        Or,
        AndSymbol,
        Dollar,
        Percent,
        Minus,
        Plus,
        Divide,
        At,
        Greater,
        Smaller,
        // UpArrow,
        Times,
        // Exclamation,
        // Question,
        EqComp,
        Equal,
        Hex,
        Numbers,
        Variable,
    }

    public class SimpleRegexTokenizer
    {
        private readonly List<TokenDefinition> m_tokenDefinitions;

        public SimpleRegexTokenizer()
        {
            m_tokenDefinitions = new List<TokenDefinition>
            {
                new(TokenType.BracketLeft,  @"\("),
                new(TokenType.BracketRight,  @"\)"),
                new(TokenType.MiddleBracketLeft,  @"\["),
                new(TokenType.MiddleBracketRight,  @"\]"),
                new(TokenType.CurlyBraceLeft,  @"\{"),
                new(TokenType.CurlyBraceRight,  @"\}"),

                new(TokenType.StrText,  "\"([^\"]*)\""), // ????  "\"(.*?)\""
                new(TokenType.WhiteSpace,  @"\s+"),
                new(TokenType.Define,  @"\#define"),
                new(TokenType.Include,  @"\#include"),
                new(TokenType.Typedef,  "typedef"),
                new(TokenType.Struct,  @"struct\b"),
                new(TokenType.Colon,  ":"),
                new(TokenType.SemiColon,  ";"),
                new(TokenType.Comma,  ","),
                new(TokenType.Dot,  @"\."),
                new(TokenType.Or,  @"\|\|"),
                new(TokenType.AndSymbol,  "&"),
                new(TokenType.Dollar,  @"\$"),
                new(TokenType.Percent,  "%"),
                new(TokenType.Minus,  @"\-"),
                new(TokenType.Plus,  @"\+"),
                new(TokenType.Divide,  @"\/"),
                new(TokenType.At,  "@"),
                new(TokenType.Greater,  ">"),
                new(TokenType.Smaller,  "<"),
                new(TokenType.Times,  @"\*"),
                new(TokenType.EqComp,  "=="),
                new(TokenType.Equal,  "="),
                new(TokenType.Hex,  "0[xX][0-9a-fA-F]+"),
                new(TokenType.Numbers,  @"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?[f]?"),
                new(TokenType.Variable,  @"\b[A-Za-z0-9_]+\b"),
            };
        }

        public IEnumerable<Token> Tokenize(string text)
        {
            var tokens = new List<Token>();

            string remainingText = text;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);
                if (match.IsMatch)
                {
                    tokens.Add(new Token(match.TokenType, match.Value));
                    remainingText = match.RemainingText;
                }
                else
                {
                    throw new Exception("Failed to generate invalid token");
                    if (IsWhitespace(remainingText))
                    {
                        remainingText = remainingText.Substring(1);
                    }
                    else
                    {
                        var invalidTokenMatch = CreateInvalidTokenMatch(remainingText);
                        tokens.Add(new Token(invalidTokenMatch.TokenType, invalidTokenMatch.Value));
                        remainingText = invalidTokenMatch.RemainingText;
                    }
                }
            }

            //tokens.Add(new Token(Type.SequenceTerminator, string.Empty));

            return tokens;
        }

        private TokenMatch FindMatch(string lqlText)
        {
            foreach (var tokenDefinition in m_tokenDefinitions)
            {
                var match = tokenDefinition.Match(lqlText);
                if (match.IsMatch)
                    return match;
            }

            return new TokenMatch() { IsMatch = false };
        }

        private bool IsWhitespace(string lqlText)
        {
            return Regex.IsMatch(lqlText, "^\\s+");
        }

        private TokenMatch CreateInvalidTokenMatch(string lqlText)
        {
            var match = Regex.Match(lqlText, "(^\\S+\\s)|^\\S+");
            if (match.Success)
            {
                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = lqlText.Substring(match.Length),
                    //Type = Type.Invalid,
                    Value = match.Value.Trim()
                };
            }

            throw new Exception("Failed to generate invalid token");
        }
    }
}