import { HubConnectionBuilder, HubConnection, HubConnectionState, LogLevel } from '@microsoft/signalr';

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
            .withUrl('/api/mainhub', { withCredentials: true })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        connection.onclose(err => {
            console.error("SignalR closed:", err);
        });
    }

    try {
        await connection.start();
        console.log("SignalR connected");
    } catch (err) {
        console.error("SignalR start failed:", err);

        setTimeout(() => connectSignalR(), 2000);
    }
};
export const startHeartBeat = () => {
    if (heartBeatInterval) return; 
    heartBeatInterval = setInterval(() => {
        if (connection?.state === HubConnectionState.Connected) {
            console.log('Sending heartbeat');
            connection.invoke('Heartbeat')
            .then(result => console.log('Heartbeat result:', result))
            .catch(err => console.error('Heartbeat error:', err));
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

export const invokeSignalR = async (methodName: string, ...args: any[]) => {
  await waitForConnected();
  return await connection!.invoke(methodName, ...args);
};