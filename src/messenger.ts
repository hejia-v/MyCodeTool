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
				break
			}
			case 'onRefreshCurrentVariable': {
				GLSLRefactor.RefreshCurrentVariable();
				break
			}
			case 'onRenameVariable': {
				break
			}
			case 'onRemoveRedundantOperatorSymbols': {
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
