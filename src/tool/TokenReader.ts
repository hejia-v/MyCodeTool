import { Token, TokenType } from "./Tokenizer";

export class TokenReader {
    private readonly m_tokens: Token[];
    private m_position: number;
    public BeginString: string = "";
    public EndString: string = "";

    public get Position(): number {
        return this.m_position;
    }

    public set Position(value: number) {
        this.m_position = Math.max(0, value);
    }

    constructor(tokens: Token[]) {
        this.m_tokens = tokens;
        this.m_position = 0;
    }

    public Read(): Token | null {
        if (this.Position < this.m_tokens.length) {
            const token = this.m_tokens[this.Position];
            this.Position++;
            return token;
        }
        return null;
    }

    public Peak(): Token | null {
        if (this.Position < this.m_tokens.length) {
            const token = this.m_tokens[this.Position];
            return token;
        }
        return null;
    }

    public Seek(pos: number): void {
        this.Position = pos;
    }

    public Rewind(offset: number = 1): void {
        if (offset < 0) throw new Error("offset must > 0");
        this.Position -= offset;
    }

    public CanRead(): boolean {
        return this.Position < this.m_tokens.length;
    }

    public GetTokenAtCharactorPosition(pos: number): Token | null {
        for (let i = 0; i < this.m_tokens.length; i++) {
            const token = this.m_tokens[i];
            if(token.StartIndex <= pos && pos < token.EndIndex) {
                return token;
            }
        }
        return null;
    }

    public ForwardToSentenceEnd(): void {
        while (this.CanRead()) {
            const tok = this.Read()!;
            if (tok.Type === TokenType.SemiColon) {
                return;
            }
        }
    }
}
