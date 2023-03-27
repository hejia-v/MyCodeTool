enum TokenType {
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

interface TokenMatch {
    IsMatch: boolean;
    TokenType: TokenType;
    Value: string;
    RemainingText: string;
}

class Token {
    constructor(public Type: TokenType, public Value: string = "") { }

    public Clone(): Token {
        return new Token(this.Type, this.Value);
    }

    public toString(): string {
        return `Token<Type="${this.Type}", Value="${this.Value}">`;
    }
}

class TokenDefinition {
    private _regex: RegExp;
    private _returnsToken: TokenType;

    constructor(returnsToken: TokenType, regexPattern: string) {
        this._regex = new RegExp(regexPattern, "i");
        this._returnsToken = returnsToken;
    }

    public Match(inputString: string): TokenMatch {
        const match = inputString.match(this._regex);
        if (match && match.index === 0) {
            let remainingText = "";
            if (match[0].length !== inputString.length) {
                remainingText = inputString.substring(match[0].length);
            }

            return {
                IsMatch: true,
                RemainingText: remainingText,
                TokenType: this._returnsToken,
                Value: match[0]
            };
        } else {
            return {
                IsMatch: false,
                RemainingText: "",
                TokenType: this._returnsToken,
                Value: ""
            };
        }
    }
}


class GLSLTokenizer {
    private readonly m_tokenDefinitions: TokenDefinition[];

    constructor() {
        this.m_tokenDefinitions = [
            new TokenDefinition(TokenType.BracketLeft, "\\("),
            new TokenDefinition(TokenType.BracketRight, "\\)"),
            new TokenDefinition(TokenType.MiddleBracketLeft, "\\["),
            new TokenDefinition(TokenType.MiddleBracketRight, "\\]"),
            new TokenDefinition(TokenType.CurlyBraceLeft, "\\{"),
            new TokenDefinition(TokenType.CurlyBraceRight, "\\}"),

            new TokenDefinition(TokenType.StrText, "\"([^\"]*)\""), // ????  "\"(.*?)\""
            new TokenDefinition(TokenType.WhiteSpace, "\\s+"),
            new TokenDefinition(TokenType.Define, "#define"),
            new TokenDefinition(TokenType.Include, "#include"),
            new TokenDefinition(TokenType.Typedef, "typedef"),
            new TokenDefinition(TokenType.Struct, "\\bstruct"),
            new TokenDefinition(TokenType.Colon, ":"),
            new TokenDefinition(TokenType.SemiColon, ";"),
            new TokenDefinition(TokenType.Comma, ","),
            new TokenDefinition(TokenType.Dot, "\\."),
            new TokenDefinition(TokenType.Or, "\\|\\|"),
            new TokenDefinition(TokenType.AndSymbol, "&"),
            new TokenDefinition(TokenType.Dollar, "\\$"),
            new TokenDefinition(TokenType.Percent, "%"),
            new TokenDefinition(TokenType.Minus, "-"),
            new TokenDefinition(TokenType.Plus, "\\+"),
            new TokenDefinition(TokenType.Divide, "/"),
            new TokenDefinition(TokenType.At, "@"),
            new TokenDefinition(TokenType.Greater, ">"),
            new TokenDefinition(TokenType.Smaller, "<"),
            new TokenDefinition(TokenType.Times, "\\*"),
            new TokenDefinition(TokenType.EqComp, "=="),
            new TokenDefinition(TokenType.Equal, "="),
            new TokenDefinition(TokenType.Hex, "0[xX][0-9a-fA-F]+"),
            new TokenDefinition(TokenType.Numbers, "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?[f]?"),
            new TokenDefinition(TokenType.Variable, "\\b[A-Za-z0-9_]+\\b")
        ];
    }

    public Tokenize(text: string): Token[] {
        const tokens: Token[] = [];

        let remainingText = text;

        while (remainingText.trim().length != 0) {
            const match = this.FindMatch(remainingText);
            if (match.IsMatch) {
                tokens.push(new Token(match.TokenType, match.Value));
                remainingText = match.RemainingText!;
            } else {
                throw new Error("Failed to generate invalid token");
            }
        }

        //tokens.Add(new Token(Type.SequenceTerminator, string.Empty));

        return tokens;
    }

    private FindMatch(lqlText: string): TokenMatch {
        for (const tokenDefinition of this.m_tokenDefinitions) {
            const match = tokenDefinition.Match(lqlText);
            if (match.IsMatch) return match;
        }

        return {
            IsMatch: false,
            RemainingText: "",
            TokenType: TokenType.Variable,
            Value: ""
        };
    }

    private IsWhitespace(lqlText: string): boolean {
        return /^\s+/.test(lqlText);
    }
}
