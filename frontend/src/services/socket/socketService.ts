
import useUserStore from '@/stores/useUserStore';
import { io, Socket } from 'socket.io-client';

const SOCKET_URL = import.meta.env.VITE_SOCKET_URL;


export const socket: Socket = io(SOCKET_URL, {
    autoConnect: false,
    reconnection: true,
    withCredentials: true,
});

export const connectSocket = () => {
    if (socket.connected) return;
    socket.connect();
   
    
};

export const disconnectSocket = () => {
    if (socket.connected) {
        
        socket.disconnect();
    }
};