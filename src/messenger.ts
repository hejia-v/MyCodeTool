import * as vscode from 'vscode'
import { GLSLRefactor } from './tool/GLSLRefactor'

export const handleMessages = (webview: vscode.Webview) => {
	receiveMessages(webview)

	sendMessages(webview)
}

const receiveMessages = (webview: vscode.Webview) => {
	webview.onDidReceiveMessage(async (message) => {
		let openPath: vscode.Uri

		switch (message.command) {
			case 'openFileExample': {
				openPath = vscode.Uri.file(message.text)

				vscode.workspace.openTextDocument(openPath).then(async (doc) => {
					vscode.window.showTextDocument(doc)
				})
				return
			}
			case 'onInlineVariable': {
				GLSLRefactor.InlineVariable();
				break
			}
			case 'onRefreshCurrentVariable': {
				GLSLRefactor.RefreshCurrentVariable();
				break
			}
			case 'onRenameVariable': {
				GLSLRefactor.RenameVariable(message.newVariable);
				break
			}
			case 'onRemoveRedundantOperatorSymbols': {
				GLSLRefactor.RemoveRedundantOperatorSymbols();
				break
			}
			case 'onNormailzeNumber': {
				GLSLRefactor.NormalizeNumber();
				break
			}
			case 'onSimplifyOperations': {
				GLSLRefactor.SimplifyOperations();
				break
			}
			case 'onFlipOperations': {
				GLSLRefactor.FlipOperations();
				break
			}
		}
	})
}

const sendMessages = (webview: vscode.Webview) => {
	vscode.window.onDidChangeActiveTextEditor(async (editor) => {
		if (!editor) return

		const currentFile = editor.document.fileName

		await webview.postMessage({
			command: 'setCurrentFileExample',
			text: currentFile
		})
	})
}
