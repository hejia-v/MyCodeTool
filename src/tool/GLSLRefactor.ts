import * as vscode from 'vscode'
import { TokenReader } from './TokenReader';
import { TextReader } from './TextReader'
import { GLSLTokenizer } from './Tokenizer';

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

        for (var i = 0; i < tokens.length; i++) {
            console.log(tokens[i]);
        }


        var tokenReader = new TokenReader(tokens);
        tokenReader.BeginString = begin;
        tokenReader.EndString = end;
        return tokenReader;
    }

    static RefreshCurrentVariable(): void {
        console.log('This is a static function.');
        const editor = vscode.window.activeTextEditor;
        this.GetTokenReader();
    }
}
