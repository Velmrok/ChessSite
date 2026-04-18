import type { LoginFormType, RegisterFormType } from "@/types/Auth";
import apiFetch from "./api";
import { disconnectSocket,connectSocket } from "./socket/socketService";
import useUserStore from "@/stores/useUserStore";


const API_URL = import.meta.env.VITE_API_URL;

export const loginUser = async (form: LoginFormType) => {
    return await apiFetch({ url: `/auth/login`, method: 'POST', includeCredentials: true, contentType: 'application/json', body: JSON.stringify(form) });
    
};

export const registerUser = async (form: RegisterFormType) => {
    return await apiFetch({ url: `/auth/register`, method: 'POST', includeCredentials: false, contentType: 'application/json', body: JSON.stringify(form) });
    
};

export const getMe = async () => {
   
    return await apiFetch({ url: `/auth/me`, method: 'GET', includeCredentials: true, contentType: 'application/json' });

  


};
export const refresh = async () => {
    return await apiFetch({ url: `/auth/refresh`, method: 'POST', includeCredentials: true, contentType: 'application/json' });
   
}


export const logoutUser = async () => {
    
    await apiFetch({ url: `/auth/logout`, method: 'POST', includeCredentials: true, contentType: 'application/json' });
}