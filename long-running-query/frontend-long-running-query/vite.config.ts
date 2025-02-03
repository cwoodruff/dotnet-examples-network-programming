import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Proxy API requests to the backend server.
      '/api': {
        target: 'http://localhost:7256',
        changeOrigin: true,
        secure: false
      },
      // Optionally proxy SignalR requests.
      '/queryHub': {
        target: 'http://localhost:7256',
        changeOrigin: true,
        secure: false
      }
    }
  }
});
