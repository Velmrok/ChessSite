import type { UsersSearchResponse } from "@/types/usersSearch";
import apiFetch from "./api";


export const fetchAllUsers = async (url: string) : Promise<UsersSearchResponse> => {
    const response = await apiFetch(`/users${url}`,{  method: 'GET'}, 'application/json');
    return response as Promise<UsersSearchResponse>;
}