import { useEffect, useState } from 'react';
import { socket } from '../services/socket/socketService'; 

type Props = {
    gameId: string;
    previousMessages: Array<Message>;
}
export const useChat = ({gameId, previousMessages}: Props) => {
    const [messages, setMessages] = useState<Array<Message>>(previousMessages);
    useEffect(() => {
        socket.emit('chat:join', gameId);
        socket.on('chat:receive_message', handleReceiveMessage);
        return () => {
            socket.off('chat:receive_message', handleReceiveMessage);
            socket.emit('chat:leave', gameId);
        }
    }, []);
    const handleReceiveMessage = (msg: Message) => {
        console.log("Received chat message:", msg);
        setMessages((prev) => [...prev, msg]);
        };
    
     
    return { messages};
}
