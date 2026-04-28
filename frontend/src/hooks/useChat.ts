import { useEffect, useState } from 'react';
import { getConnection, invokeSignalR } from '@/services/signalR/connection';

type Props = {
    gameId: string;
    previousMessages: Array<Message>;
}
export const useChat = ({gameId, previousMessages}: Props) => {
    const [messages, setMessages] = useState<Array<Message>>(previousMessages);
    useEffect(() => {
        const conn = getConnection();
        invokeSignalR('JoinGroup', { type: "Chat", correlationId: crypto.randomUUID(), payload: { gameId } });
        conn.on('MessageReceived', handleReceiveMessage);
        return () => {
            conn.off('MessageReceived', handleReceiveMessage);
            invokeSignalR('LeaveGroup', { type: "Chat", correlationId: crypto.randomUUID(), payload: { gameId } });
        }
    }, []);
    const handleReceiveMessage = (msg: Message) => {
        console.log("Received chat message:", msg);
        setMessages((prev) => [...prev, msg]);
        };
    
     
    return { messages};
}
