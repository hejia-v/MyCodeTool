import assert from "assert";

class TextReader {
    private readonly m_text: string;
    private m_position: number;

    constructor(text: string) {
        this.m_text = text;
        this.m_position = 0;
    }

    get Position(): number {
        return this.m_position;
    }

    set Position(position: number) {
        this.m_position = Math.max(0, position);
    }

    public read(): string | null {
        if (this.m_position >= this.m_text.length) {
            return null;
        }

        const character = this.m_text.charAt(this.m_position);
        this.m_position++;

        return character;
    }

    public Peak(): string | null {
        if (this.m_position < this.m_text.length) {
            const character = this.m_text.charAt(this.m_position);
            return character;
        }
        return null;
    }

    public Seek(pos: number): void {
        this.Position = pos;
    }

    public Rewind(offset: number = 1): void {
        if (offset < 0) {
            throw new Error("offset must > 0");
        }
        this.Position -= offset;
    }

    public Advance(offset: number): void {
        offset = Math.max(0, offset);
        this.Position += offset;
    }

    public CanRead(): boolean {
        return this.Position < this.m_text.length;
    }

    public JumpToChar(c: string): void {
        const pos = this.Position;
        while (this.CanRead()) {
            const tok = this.read();
            if (tok === c) {
                return;
            }
        }
        this.Seek(pos);
    }

    public JumpToString(c: string): void {
        if (!this.CanRead()) { return; }
        const ti = this.m_text.indexOf(c, this.Position);
        if (ti < 0) {
            this.Seek(this.m_text.length + 1);
        } else {
            this.Seek(ti);
        }
    }

    public JumpBackToString(c: string): boolean {
        const ti = this.m_text.lastIndexOf(c, this.Position);
        if (ti < 0) {
            return false;
        } else {
            this.Seek(ti);
            return true;
        }
    }

    public JumpBraceBracketEnd(): void {
        assert(this.read() === "{");
        let depth = 1;

        while (this.CanRead()) {
            const tok = this.read();
            if (tok === "{") {
                depth++;
            } else if (tok === "}") {
                depth--;
                if (depth === 0) {
                    return;
                }
            }
        }
        throw new Error("can not find end brace bracket");
    }

    public PeekString(): string {
        const str = this.m_text.substring(0, this.Position);
        return str;

    }

    public GetString(start: number, end: number): string {
        const str = this.m_text.substring(start, end);
        return str;
    }
}
