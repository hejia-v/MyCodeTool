using BinaryTemplate;
using System.Diagnostics;

public class FieldNode
{
}

public class SentenceNode
{
}

public class MainProgramNode
{
    private List<StructNode> m_structs = new List<StructNode>();

    public void AddStruct(StructNode node)
    {
        m_structs.Add(node);
    }
}

public class FunctionNode
{
}

public class StructNode
{
    public void Parse(TokenReader reader)
    {
        var pos = reader.Position;

        Debug.Assert(reader.Read().Type == TokenType.CurlyBraceLeft);
        var brace_depth = 1;

        while (reader.CanRead())
        {
            var tok = reader.Read();
            if (tok.Type == TokenType.CurlyBraceLeft)
            {
                brace_depth++;
            }
            else if (tok.Type == TokenType.CurlyBraceRight)
            {
                brace_depth--;
                if (brace_depth == 0)
                {
                }
            }
            else
            {
            }
        }
    }
}

public class Generator
{
    public void Parse(List<Token> tokens)
    {
        var reader = new TokenReader(tokens);
        var main = new MainProgramNode();

        while (reader.CanRead())
        {
            var tok = reader.Read();
            if (tok.Type == TokenType.Define)
            {
                reader.ForwardToSentenceEnd();
            }
            else if (tok.Type == TokenType.Typedef)
            {
                Debug.Assert(reader.Read().Type == TokenType.Struct);
                Debug.Assert(reader.Read().Type == TokenType.CurlyBraceLeft);
                reader.Rewind();
                var structNode = new StructNode();
                structNode.Parse(reader);
                main.AddStruct(structNode);
            }
            else
            {
                break;
            }
        }
    }
}

public class Program
{
    private static void Main(string[] args)
    {
        //Console.WriteLine("start:");
        var filepath = "J:/GameAnalysis/Bayonetta/bayonetta_tools/binary_templates/Bayonetta wmb.bt";
        var text = File.ReadAllText(filepath);
        var tr = new SimpleRegexTokenizer();

        foreach (var token in tr.Tokenize(text))
        {
            if (token.Type == TokenType.WhiteSpace) continue;
            Console.WriteLine(token);
        }

        var tokens = tr.Tokenize(text).Where(x => x.Type != TokenType.WhiteSpace).ToList();
        var g = new Generator();
        g.Parse(tokens);
    }
}