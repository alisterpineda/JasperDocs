import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { TanStackRouterVite } from '@tanstack/router-plugin/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    TanStackRouterVite({
      routesDirectory: './src/routes',
      generatedRouteTree: './src/routeTree.gen.ts',
    }),
    react(),
  ],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        // Use VITE_API_URL from Aspire, or fall back to localhost:5000 for standalone dev
        target: process.env.VITE_API_URL || 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
        followRedirects: false, // Don't follow redirects, go straight to HTTPS
        rewrite: (path) => path, // Keep the path as-is
        configure: (proxy, options) => {
          console.log('[Vite Proxy] Proxying /api requests to:', options.target);
        },
      }
    }
  },
  build: {
    outDir: '../JasperDocs.WebApi/wwwroot',
    emptyOutDir: true,
  },
})
