import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr';

let connection: HubConnection | null = null;
let connectionPromise: Promise<void> | null = null;
let heartBeatInterval: ReturnType<typeof setTimeout> | null = null;
export const getConnection = (): HubConnection => {
    if (!connection) throw new Error('SignalR not connected');
    return connection;
};

export const connectSignalR = async (): Promise<void> => {
    
    if (connection?.state === HubConnectionState.Connected) return;


    if (connectionPromise) return connectionPromise;

    connection = new HubConnectionBuilder()
        .withUrl('/api/mainhub', { withCredentials: true })
        .withAutomaticReconnect()
        .build();


    connectionPromise = connection.start().finally(() => {
        connectionPromise = null;
    });

    return connectionPromise;
};
export const startHeartBeat = () => {
    if (heartBeatInterval) return; 
    heartBeatInterval = setInterval(() => {
        if (connection?.state === HubConnectionState.Connected) {
            console.log('Sending heartbeat');
            connection.invoke('Heartbeat').catch(err => console.error('Heartbeat error:', err));
        }
    }, 20000); 
};


export const invokeSignalR = async (methodName: string, ...args: any[]) => {
    if (!connection) throw new Error('SignalR not connected');
    try {
    return await connection.invoke(methodName, ...args);
    } catch (error) {
        console.error(`Error invoking SignalR method ${methodName}:`, error);
        throw error;
    }
}