import apiFetch from "./api";

const API_URL = import.meta.env.VITE_API_URL;
type AllUsers = {
  users: User[];
  totalPages: number;
}
export const fetchAllUsers = async (url: string) : Promise<AllUsers> => {
    const response = await apiFetch({ url: `/users/all${url}`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
    return response.json()as Promise<AllUsers>;
}