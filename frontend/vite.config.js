import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';
import path from 'path';
import basicSsl from '@vitejs/plugin-basic-ssl';
// https://vite.dev/config/
export default defineConfig({
    plugins: [react(), tailwindcss(),basicSsl()],
    server: {
        host: true,
        watch: {
            usePolling: true
        },
        proxy: {
            '/api': {
                target: 'http://backend:5062',
                changeOrigin: true,
                secure: false,
                rewrite: (path) => path.replace(/^\/api/, ''),
                ws: true
            }
        }
    },
    resolve: {
        extensions: ['.js', '.ts', '.jsx', '.tsx', '.json'],
        alias: {
            "@": path.resolve(__dirname, "./src")
        }
    }
});
