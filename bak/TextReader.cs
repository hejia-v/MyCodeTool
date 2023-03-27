using System;
using UnityEngine;

public class TextReader
{
    private readonly string m_text;
    private int m_position;

    public int Position
    {
        get => m_position;
        private set => m_position = Math.Max(0, value);
    }

    public TextReader(string text)
    {
        m_text = text;
        Position = 0;
    }

    public char Read()
    {
        if (Position < m_text.Length)
        {
            var token = m_text[Position];
            Position++;
            return token;
        }
        return default;
    }

    public char Peak()
    {
        if (Position < m_text.Length)
        {
            var token = m_text[Position];
            return token;
        }
        return default;
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

    public void Advance(int offset)
    {
        offset = Mathf.Max(0, offset);
        Position += offset;
    }

    public bool CanRead()
    {
        return Position < m_text.Length;
    }

    public void Jump(char c)
    {
        var pos = Position;
        while (CanRead())
        {
            var tok = Read();
            if (tok == c)
            {
                return;
            }
        }
        Seek(pos);
    }

    public void Jump(string c)
    {
        if (!CanRead())
        {
            return;
        }

        var ti = m_text.IndexOf(c, Position);
        if (ti < 0)
        {
            Seek(m_text.Length + 1);
        }
        else
        {
            Seek(ti);
        }
    }

    public bool JumpBack(string c)
    {
        var ti = m_text.LastIndexOf(c, Position);
        if (ti < 0)
        {
            return false;
        }
        else
        {
            Seek(ti);
            return true;
        }
    }

    public void JumpBraceBracketEnd()
    {
        Debug.Assert(Read() == '{');
        var depth = 1;

        while (CanRead())
        {
            var tok = Read();
            if (tok == '{')
            {
                depth++;
            }
            else if (tok == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return;
                }
            }
        }

        throw new Exception("");
    }

    public string PeekString()
    {
        var str = m_text.Substring(0, Position);
        return str;
    }

    public string PeekString(int start, int end)
    {
        var str = m_text.Substring(start, end - start);
        return str;
    }

    public void ForwardToSentenceEnd()
    {
        //while (CanRead())
        //{
        //    var tok = Read();
        //    if (tok.Type == TokenType.SemiColon)
        //    {
        //        return;
        //    }
        //}
    }
}