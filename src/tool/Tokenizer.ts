export enum TokenType {
    BracketLeft = "BracketLeft",
    BracketRight = "BracketRight",
    MiddleBracketLeft = "MiddleBracketLeft",
    MiddleBracketRight = "MiddleBracketRight",
    CurlyBraceLeft = "CurlyBraceLeft",
    CurlyBraceRight = "CurlyBraceRight",
    StrText = "StrText",
    WhiteSpace = "WhiteSpace",
    Define = "Define",
    Include = "Include",
    Typedef = "Typedef",
    Struct = "Struct",
    Colon = "Colon",
    SemiColon = "SemiColon",
    Comma = "Comma",
    Dot = "Dot",
    Or = "Or",
    AndSymbol = "AndSymbol",
    Dollar = "Dollar",
    Percent = "Percent",
    Minus = "Minus",
    Plus = "Plus",
    Divide = "Divide",
    At = "At",
    Greater = "Greater",
    Smaller = "Smaller",
    // = "//" UpArrow,
    Times = "Times",
    // = "//" Exclamation,
    Question = "Question",
    EqComp = "EqComp",
    Equal = "Equal",
    Hex = "Hex",
    Number = "Number",
    Variable = "Variable",
    Comment = "Comment",
}

interface TokenMatch {
    IsMatch: boolean;
    TokenType: TokenType;
    Value: string;
    RemainingText: string;
}

export class Token {
    constructor(public Type: TokenType, public Value: string = "") { }

    public StartIndex: number = 0;

    public get EndIndex(): number {
        return this.StartIndex + this.Value.length;
    }

    public get Length(): number {
        return this.Value.length;
    }

    public SentenceId: number = 0;

    public Clone(): Token {
        return new Token(this.Type, this.Value);
    }

    public toString(): string {
        // var type = TokenType[this.Type];
        var type = this.Type.toString();
        return `Token<Type="${type}", Value="${this.Value}">`;
    }
}

class TokenDefinition {
    private _regex: RegExp;
    private _returnsToken: TokenType;

    constructor(returnsToken: TokenType, regexPattern: RegExp) {
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


export class GLSLTokenizer {
    private readonly m_tokenDefinitions: TokenDefinition[];

    constructor() {
        const pattern = /\?/g;
        this.m_tokenDefinitions = [
            new TokenDefinition(TokenType.Comment, /\/\/.*\r?\n/),
            new TokenDefinition(TokenType.BracketLeft, /\(/),
            new TokenDefinition(TokenType.BracketRight, /\)/),
            new TokenDefinition(TokenType.MiddleBracketLeft, /\[/),
            new TokenDefinition(TokenType.MiddleBracketRight, /\]/),
            new TokenDefinition(TokenType.CurlyBraceLeft, /\{/),
            new TokenDefinition(TokenType.CurlyBraceRight, /\}/),

            new TokenDefinition(TokenType.StrText, /"([^"])"/), // ????  "\"(.*?)\""
            new TokenDefinition(TokenType.WhiteSpace, /\s+/),
            new TokenDefinition(TokenType.Define, /\#define/),
            new TokenDefinition(TokenType.Include, /\#include/),
            new TokenDefinition(TokenType.Colon, /\:/),
            new TokenDefinition(TokenType.SemiColon, /\;/),
            new TokenDefinition(TokenType.Comma, /\,/),
            new TokenDefinition(TokenType.Dot, /\./),
            new TokenDefinition(TokenType.Or, /\|\|/),
            new TokenDefinition(TokenType.AndSymbol, /\&/),
            new TokenDefinition(TokenType.Question, /\?/),
            new TokenDefinition(TokenType.Dollar, /\$/),
            new TokenDefinition(TokenType.Percent, /\%/),
            new TokenDefinition(TokenType.Minus, /\-/),
            new TokenDefinition(TokenType.Plus, /\+/),
            new TokenDefinition(TokenType.Divide, /\//),
            new TokenDefinition(TokenType.At, /\@/),
            new TokenDefinition(TokenType.Greater, /\>/),
            new TokenDefinition(TokenType.Smaller, /\</),
            new TokenDefinition(TokenType.Times, /\*/),
            new TokenDefinition(TokenType.EqComp, /==/),
            new TokenDefinition(TokenType.Equal, /\=/),
            new TokenDefinition(TokenType.Hex, /0[xX][0-9a-fA-F]+/),
            new TokenDefinition(TokenType.Number, /[-+]?[0-9].?[0-9]+([eE][-+]?[0-9]+)?[f]?/),
            new TokenDefinition(TokenType.Variable, /\b[A-Za-z0-9_]+\b/)
        ];
    }

    public Tokenize(text: string): Token[] {
        const tokens: Token[] = [];

        let remainingText = text;

        while (remainingText.trim().length != 0) {
            const match = this.FindMatch(remainingText);
            if (match.IsMatch) {
                const token = new Token(match.TokenType, match.Value);
                // console.log(token);
                tokens.push(token);
                remainingText = match.RemainingText!;
            } else {
                var text = "";
                for (var i = 0; i < tokens.length; i++) {
                    text += tokens[i].Value;
                }
                console.log(text);
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
