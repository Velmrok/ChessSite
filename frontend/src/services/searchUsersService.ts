import type { UserSearchItem } from "@/types/user";
import apiFetch from "./api";
import type { UsersResponse } from "@/types/usersSearch";


export const fetchAllUsers = async (url: string) : Promise<{ users: UserSearchItem[], totalPages: number }> => {
    console.log("Fetching users with URL:", url);
    const response = await apiFetch({ url: `/users${url}`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
    return response as Promise<{ users: UserSearchItem[], totalPages: number }>;
}