import useUserStore from '@/stores/useUserStore';
import type { SignalRError, SignalRRequest, SignalRResponse } from '@/types/signalR';
import { HubConnectionBuilder, HubConnection, HubConnectionState, HttpTransportType } from '@microsoft/signalr';
import { v4 as uuid } from 'uuid';

let connection: HubConnection | null = null;
let isReconnecting = false;
let heartBeatInterval: ReturnType<typeof setTimeout> | null = null;
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const eventListeners = new Map<string, (response: any) => void>();

function resubscribeEvents() {
    if (!connection) return;
    for (const [event, handler] of eventListeners) {
        connection.on(event, handler);
    }
}
const setIsConnected = useUserStore.getState().setIsConnected;
export const getConnection = (): HubConnection => {
    if (!connection) throw new Error('SignalR not connected');
    return connection;
};

export const connectSignalR = async (): Promise<void> => {
    console.log('Attempting to connect to SignalR...');
    if (connection?.state === HubConnectionState.Connected || connection?.state === HubConnectionState.Connecting) {
        setIsConnected(true);
        return;
    }

    if (!connection) {
        connection = new HubConnectionBuilder()
            .withUrl('/api/mainhub', {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()

            .build();
            setIsConnected(true);

        connection.onclose(err => {
            console.error("SignalR closed:", err);
            setIsConnected(false);
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
    isReconnecting = true;
    if (connection) {
        try {
            await connection.stop();
            setIsConnected(false);
        } catch (err) {
            console.error("Error stopping SignalR connection:", err);
        }
        connection = null;
    }
    await connectSignalR();
    resubscribeEvents();
    isReconnecting = false;
}
export const startHeartBeat = () => {
    if (heartBeatInterval) return;
    heartBeatInterval = setInterval(() => {
        if (connection?.state === HubConnectionState.Connected) {
            connection.invoke('Heartbeat', { type: "Heartbeat", correlationId: uuid(), data: {} })
        }
    }, 20000);
};
export const stopHeartBeat = () => {
    if (heartBeatInterval) {
        clearInterval(heartBeatInterval);
        heartBeatInterval = null;
    }
};


const waitForConnected = async () => {
  if (!connection) {
    await connectSignalR();
  }

  while (isReconnecting || !connection || connection.state !== HubConnectionState.Connected) {
    await new Promise(res => setTimeout(res, 50));
    }
};

export const invokeSignalR = async <TData = unknown>(
    methodName: string,
    request: SignalRRequest
): Promise<SignalRResponse<TData>> => {
    await waitForConnected();
    let response: SignalRResponse<TData>;
    try {
        response = await connection!.invoke(methodName, request);
    } catch (err: any) {
        if (isReconnecting || !connection || connection.state !== HubConnectionState.Connected) {
            await waitForConnected();
            response = await connection!.invoke(methodName, request);
        } else {
            throw err;
        }
    }
    if (response.error) {
        throw Object.assign(new Error(response.error.title), {
            signalRError: response.error
        });
    }
    return response as SignalRResponse<TData>;
};
export const signUpForEvent = <TData = unknown>(eventName: string, callback: (response: SignalRResponse<TData>) => void,onError?: (error: SignalRError) => void) => {
    const handler = (Response: SignalRResponse<TData>) => {
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
    };
    eventListeners.set(eventName, handler);
    const conn = getConnection();
    conn.on(eventName, handler);
};
export const leaveEvent = (eventName: string) => {
    const handler = eventListeners.get(eventName);
    if (handler) {
        eventListeners.delete(eventName);
        if (connection) connection.off(eventName, handler);
    }
};