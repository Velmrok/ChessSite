import mqtt from 'mqtt';
import apiFetch from '../api';
const MQTT_BROKER_URL = import.meta.env.VITE_MQTT_BROKER_URL 

let client: mqtt.MqttClient | null = null;
export const connectToBroker = () => {
    if (client && client.connected) {
        return client
    }
    client = mqtt.connect(MQTT_BROKER_URL);
    return client;
};
export const disconnectFromBroker = () => {
    if (client) {
        client.end();
        client = null; 
    }
};
export const publishSystemAlert = (message: string) => {
    const response = apiFetch('/mqtt/alert', 'POST', true, 'application/json', JSON.stringify({message}));
    return response;
   
};
export const publishLogoutEveryone = () => {
    const response = apiFetch('/mqtt/all/logout', 'POST', true, 'application/json', null);
    return response;
}