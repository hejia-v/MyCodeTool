using BinaryTemplate;

public class TokenReader
{
    private readonly List<Token> m_tokens;
    private int m_position;

    public int Position
    {
        get => m_position;
        private set => m_position = Math.Max(0, value);
    }

    public TokenReader(List<Token> tokens)
    {
        m_tokens = tokens;
        Position = 0;
    }

    public Token Read()
    {
        if (Position < m_tokens.Count)
        {
            var token = m_tokens[Position];
            Position++;
            return token;
        }
        return null;
    }

    public Token Peak()
    {
        if (Position < m_tokens.Count)
        {
            var token = m_tokens[Position];
            return token;
        }
        return null;
    }

    public void Seek(int pos)
    {
        Position = pos;
    }

    public void Rewind(int offset = 1)
    {
        if (offset < 0) throw new Exception("offset must > 0");
        Position -= offset;
    }

    public bool CanRead()
    {
        return Position < m_tokens.Count;
    }

    public void ForwardToSentenceEnd()
    {
        while (CanRead())
        {
            var tok = Read();
            if (tok.Type == TokenType.SemiColon)
            {
                return;
            }
        }
    }
}