<template>
  <div class="flex items-center flex-col mx-auto">

    
    <button @click="onInlineVariable">
      Inline variable
    </button>
    
    <hr class="border-white w-full mt-4 mb-8">
    <p>Old Variable</p>
    <input v-model="oldVariable" type="text">
    <p>New Variable</p>
    <input v-model="newVariable" type="text">
    
    <button @click="onRenameVariable">
      Rename variable
    </button>
    
    <hr class="border-white w-full mt-4 mb-8">
    <button @click="onRemoveRedundantOperatorSymbols">
      Remove redundant operator symbols
    </button>



  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import IconLighthouse from '~icons/mdi/lighthouse'
import Button from './components/Button.vue'

const { t, locale } = useI18n()

let currentFile = ref('')
let lastFile = ref('')

let oldVariable = ref('')
let newVariable = ref('')

// Example of handling messages sent from the extension to the webview
window.addEventListener('message', (event) => {
  const message = event.data // The JSON data our extension sent

  switch (message.command) {
    case 'setCurrentFileExample':
      lastFile.value = currentFile.value
      currentFile.value = message.text
      return
  }
})

// Example of sending a message from the webview to the extension
const openLastFile = () => {
  vscode.postMessage({
    command: 'openFileExample',
    text: lastFile.value
  })
}

const onInlineVariable = () => {
  vscode.postMessage({
    command: 'onInlineVariable',
  })
}

const onRenameVariable = () => {
  vscode.postMessage({
    command: 'onRenameVariable',
    oldVariable: oldVariable.value,
    newVariable: newVariable.value,
  })
}

const onRemoveRedundantOperatorSymbols = () => {
  vscode.postMessage({
    command: 'onRemoveRedundantOperatorSymbols',
  })
}

</script>
