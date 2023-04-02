<template>
  <div class="flex items-center flex-col mx-auto box1">
    <div class="textcontainer1">
      <p class="fullwidth1 text1">Curr Var:&nbsp;&nbsp;{{ currVariable }}</p>
    </div>
    <button @click="onRefreshCurrentVariable">
      Refresh variable
    </button>
  </div>

  <hr class="splitter1">

  <div class="flex items-center flex-col mx-auto box1">
    <button @click="onInlineVariable">
      Inline variable
    </button>
  </div>

  <hr class="splitter1">

  <div class="flex items-center flex-col mx-auto box1">
    <div class="textcontainer1 flex">
      <p class="text1">New Var:&nbsp;&nbsp;</p>
      <input v-model="newVariable" type="text">
    </div>

    <button @click="onRenameVariable">
      Rename variable
    </button>
  </div>

  <hr class="splitter1">

  <div class="flex items-center flex-col mx-auto box1">
    <button @click="onRemoveRedundantOperatorSymbols">
      Remove redundant operator symbols
    </button>
    <div class="space_height1">
    </div>
    <button @click="onNormailzeNumber">
      Normailze number
    </button>
    <div class="space_height1">
    </div>
    <button @click="onSimplifyOperations">
      Simplify operations
    </button>
    <div class="space_height1">
    </div>
    <button @click="onFlipOperations">
      Flip operations
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

let currVariable = ref('')
let newVariable = ref('')

// Example of handling messages sent from the extension to the webview
window.addEventListener('message', (event) => {
  const message = event.data // The JSON data our extension sent

  switch (message.command) {
    case 'setCurrentFileExample': {
      lastFile.value = currentFile.value
      currentFile.value = message.text
      return
    }
    case 'setCurrentVariable': {
      currVariable.value = message.text
      return
    }
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

const onRefreshCurrentVariable = () => {
  vscode.postMessage({
    command: 'onRefreshCurrentVariable',
  })
}

const onRenameVariable = () => {
  vscode.postMessage({
    command: 'onRenameVariable',
    currVariable: currVariable.value,
    newVariable: newVariable.value,
  })
}

const onRemoveRedundantOperatorSymbols = () => {
  vscode.postMessage({
    command: 'onRemoveRedundantOperatorSymbols',
  })
}

const onNormailzeNumber = () => {
  vscode.postMessage({
    command: 'onNormailzeNumber',
  })
}

const onSimplifyOperations = () => {
  vscode.postMessage({
    command: 'onSimplifyOperations',
  })
}

const onFlipOperations = () => {
  vscode.postMessage({
    command: 'onFlipOperations',
  })
}

</script>
