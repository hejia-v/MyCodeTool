import * as vscode from 'vscode'
import { TokenReader } from './TokenReader';
import { TextReader } from './TextReader'
import { GLSLTokenizer, TokenType } from './Tokenizer';

export class GLSLRefactor {
    static GetTokenReader(): TokenReader | null {
        const editor = vscode.window.activeTextEditor;
        if (!editor) return null;
        const text = editor?.document.getText();
        const reader = new TextReader(text);
        reader.JumpToString('void main()');
        reader.JumpToChar('{');
        var p1 = reader.Position;
        var begin = reader.PeekString();
        reader.Rewind();
        reader.JumpBraceBracketEnd();
        var p2 = reader.Position - 1;
        var end = reader.GetStringRight(p2);
        var middle = text?.substring(p1, p2);
        // console.log(text);
        // console.log(begin);
        // console.log(middle);
        // console.log(end);

        var glslTokenizer = new GLSLTokenizer();
        var tokens = glslTokenizer.Tokenize(middle);

        var index = begin.length;
        var sentenceId = 0;
        for (var i = 0; i < tokens.length; i++) {
            var token = tokens[i];
            token.StartIndex = index;
            index += token.Length;
            token.SentenceId = sentenceId;
            if (token.Type === TokenType.SemiColon) {
                sentenceId++;
            }
            // console.log(token);
        }

        var tokenReader = new TokenReader(tokens);
        tokenReader.BeginString = begin;
        tokenReader.EndString = end;
        return tokenReader;
    }

    static RefreshCurrentVariable(): void {
        console.log('This is a static function.');
        const editor = vscode.window.activeTextEditor;
        if (!editor) return;
        var tokenReader = this.GetTokenReader();
        const document = editor.document;
        const offset = editor.selection.active.character;
        const position = document.positionAt(offset);
        var token = tokenReader?.GetTokenAtCharactorPosition(offset);
        if (!token) { return; }
        console.log(token.Value);
    }
}
