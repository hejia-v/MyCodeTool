import * as vscode from 'vscode'
import { TokenReader } from './TokenReader'

export class GLSLRefactor {
    static GetTokenReader(): TokenReader|null {
        const editor = vscode.window.activeTextEditor;
        if(!editor) return null;
        const text = editor?.document.getText();
        console.log(text);
        return new TokenReader([]);
    }
    static RefreshCurrentVariable(): void {
        console.log('This is a static function.');
        const editor = vscode.window.activeTextEditor;
        this.GetTokenReader();
    }
}
