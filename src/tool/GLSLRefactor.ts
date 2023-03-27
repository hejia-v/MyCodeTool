import * as vscode from 'vscode'

export class GLSLRefactor {
    static RefreshCurrentVariable(): void {
        console.log('This is a static function.');
        const editor = vscode.window.activeTextEditor;
    }
}
