import type { SignalRError, SignalRRequest } from '@/types/signalR';
import { HubConnectionBuilder, HubConnection, HubConnectionState, HttpTransportType } from '@microsoft/signalr';

let connection: HubConnection | null = null;
let heartBeatInterval: ReturnType<typeof setTimeout> | null = null;
export const getConnection = (): HubConnection => {
    if (!connection) throw new Error('SignalR not connected');
    return connection;
};

export const connectSignalR = async (): Promise<void> => {
    console.log('Attempting to connect to SignalR...');
    if (connection?.state === HubConnectionState.Connected || connection?.state === HubConnectionState.Connecting) return;

    if (!connection) {
        connection = new HubConnectionBuilder()
            .withUrl('/api/mainhub', {
                withCredentials: true,
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()

            .build();

        connection.onclose(err => {
            console.error("SignalR closed:", err);
        });
    }

    try {
        await connection.start();
    } catch (err) {
        console.error("SignalR start failed:", err);

        setTimeout(() => connectSignalR(), 2000);
    }
};
export const startHeartBeat = () => {
    if (heartBeatInterval) return; 
    heartBeatInterval = setInterval(() => {
        if (connection?.state === HubConnectionState.Connected) {
            connection.invoke('Heartbeat')
        }
    }, 20000); 
};


const waitForConnected = async () => {
  if (!connection) {
    await connectSignalR();
  }

  while (!connection || connection.state !== HubConnectionState.Connected) {
    await new Promise(res => setTimeout(res, 50));
    }
};

export const invokeSignalR = async (methodName: string, request: SignalRRequest,
    options?: {
        onError?: (error: SignalRError) => void;
    }) => {
    await waitForConnected();

  
    const response = await connection!.invoke(methodName, request);
    if (response.error) {
        if (options?.onError) {
            options.onError(response.error);
        } else {
            console.error("SignalR Error:", response.error);
        }
    }
    return response.data;
};