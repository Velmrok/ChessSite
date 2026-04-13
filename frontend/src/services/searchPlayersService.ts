import apiFetch from "./api";

const API_URL = import.meta.env.VITE_API_URL;
type AllUsers = {
  users: User[];
  totalPages: number;
}
export const fetchAllUsers = async (url: string) : Promise<AllUsers> => {
    const response = await apiFetch(`/users/all${url}`, 'GET', true, 'application/json');
    return response.json()as Promise<AllUsers>;
}