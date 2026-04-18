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
    const response = await apiFetch(`/auth/login`, 'POST', true, 'application/json', JSON.stringify(userData));
    return response.json();
};

export const registerUser = async (userData: UserData) => {
    const response = await apiFetch(`/auth/register`, 'POST', false, 'application/json', JSON.stringify(userData));
    
    return response.json();
};

export const getMe = async () => {
   
    const response = await apiFetch(`/auth/me`, 'GET', true, 'application/json');

    return response.json();


};
export const refresh = async () => {
    const response = await apiFetch(`/auth/refresh`, 'POST', true, 'application/json');
    return response.json();
}


export const logoutUser = async () => {
    
    const response = await apiFetch(`/auth/logout`, 'POST', true, 'application/json');
    return response.json();
}