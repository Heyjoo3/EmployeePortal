import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import VueDevTools from 'vite-plugin-vue-devtools'
import express from 'express'
import cors from 'cors'

const app = express()

app.use(cors())
// import mkcert from 'vite-plugin-mkcert/dist/mkcert/index'
// import basicSsl from '@vitejs/plugin-basic-ssl'

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    proxy: {
      '/foo': 'http://localhost:5174',
      '/api': {
        target: 'http://localhost:5174',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  },

  plugins: [
    vue(),
    VueDevTools()
    // mkcert,
    // basicSsl()
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})
