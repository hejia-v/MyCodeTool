namespace BinaryTemplate
{
    public class Token
    {
        public Token(TokenType type)
        {
            Type = type;
            Value = string.Empty;
        }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token Clone()
        {
            return new Token(Type, Value);
        }

        public override string ToString()
        {
            return $"Token<Type=\"{Type}\", Value=\"{Value}\">";
        }
    }
}