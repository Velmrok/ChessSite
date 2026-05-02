import type { SignalRError, SignalRRequest, SignalRResponse } from '@/types/signalR';
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
export const reconnectSignalR = async () => {
    if (connection) {
        try {
            await connection.stop();
        } catch (err) {
            console.error("Error stopping SignalR connection:", err);
        }
        connection = null;
    }
    await connectSignalR();
}
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

export const invokeSignalR = async <TData = unknown>(
    methodName: string,
    request: SignalRRequest
): Promise<SignalRResponse<TData>> => {
    await waitForConnected();
    const response: SignalRResponse<TData> = await connection!.invoke(methodName, request);
    if (response.error) {
        throw Object.assign(new Error(response.error.title), { 
            signalRError: response.error 
        });
    }
    return response as SignalRResponse<TData>;
};
export const signUpForEvent = <TData = unknown>(eventName: string, callback: (response: SignalRResponse<TData>) => void,onError?: (error: SignalRError) => void) => {
    const conn = getConnection();
    conn.on(eventName, (Response: SignalRResponse<TData>) => {
        if (Response == null) {
            console.error(`Received null response for event ${eventName}`);
            return;
        }
        if (Response.error ) {
            console.error(`Error in event ${eventName}:`, Response.error);
            if (onError)
                onError(Response.error);
            return;
        }
        callback(Response);
    });
};
export const leaveEvent = (eventName: string, callback?: (...args: any[]) => void) => {
    const conn = getConnection();
    conn.off(eventName, callback || (() => {}));
};