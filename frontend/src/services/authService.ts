import apiFetch from "./api";
import { disconnectSocket,connectSocket } from "./socket/socketService";
import useUserStore from "@/stores/useUserStore";


const API_URL = import.meta.env.VITE_API_URL;
type UserData = {
    login: string;
    email?: string;
    nickname?: string;
    password: string;
};
export const loginUser = async (userData: UserData) => {
    const response = await apiFetch({ url: `/auth/login`, method: 'POST', includeCredentials: true, contentType: 'application/json', body: JSON.stringify(userData) });
    return response.json();
};

export const registerUser = async (userData: UserData) => {
    const response = await apiFetch({ url: `/auth/register`, method: 'POST', includeCredentials: false, contentType: 'application/json', body: JSON.stringify(userData) });
    return response.json();
};

export const getMe = async () => {
   
    const response = await apiFetch({ url: `/auth/me`, method: 'GET', includeCredentials: true, contentType: 'application/json' });

    return response.json();


};
export const refresh = async () => {
    const response = await apiFetch({ url: `/auth/refresh`, method: 'POST', includeCredentials: true, contentType: 'application/json' });
    return response.json();
}


export const logoutUser = async () => {
    
    const response = await apiFetch({ url: `/auth/logout`, method: 'POST', includeCredentials: true, contentType: 'application/json' });
    return response.json();
}