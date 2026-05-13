import { useEffect, useState } from 'react';
import { getConnection, invokeSignalR } from '@/services/signalR/connection';
import type { MessageInfo } from '@/types/game';

type Props = {
    gameId: string;
    previousMessages: Array<MessageInfo>;
}
export const useChat = ({gameId, previousMessages}: Props) => {
    const [messages, setMessages] = useState<Array<MessageInfo>>(previousMessages);
    useEffect(() => {
        const conn = getConnection();
        invokeSignalR('JoinGroup', { type: "Chat", correlationId: crypto.randomUUID(), payload: { gameId } });
        conn.on('MessageReceived', handleReceiveMessage);
        return () => {
            conn.off('MessageReceived', handleReceiveMessage);
            invokeSignalR('LeaveGroup', { type: "Chat", correlationId: crypto.randomUUID(), payload: { gameId } });
        }
    }, []);
    const handleReceiveMessage = (msg: MessageInfo) => {
        console.log("Received chat message:", msg);
        setMessages((prev) => [...prev, msg]);
        };
    
     
    return { messages};
}
