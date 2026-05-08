import type { FriendsOnlineResponse, FriendsProfileResponse, ProfileGame, UserProfile } from "@/types/user";
import apiFetch from "./api";



export const fetchUserProfile = async (nickname: string): Promise<UserProfile> => {
    const response = await apiFetch(`/users/${nickname}/profile`,{  method: 'GET' }, 'application/json');
    return response as Promise<UserProfile>;
};
export const fetchUserFriends = async (nickname: string, page: number, limit: number = 5): Promise<FriendsProfileResponse> => {
    const response = await apiFetch(`/users/${nickname}/friends?page=${page}&limit=${limit}`,{  method: 'GET'}, 'application/json');
    
    return response as Promise<FriendsProfileResponse>;
}
export const fetchFriendsOnline = async (page: number, limit: number): Promise<FriendsOnlineResponse> => {
    const response = await apiFetch(`/users/me/friends-online?page=${page}&limit=${limit}`, { method: 'GET' }, 'application/json');
    return response as Promise<FriendsOnlineResponse>;
}   
export const updateUserBio = async (bio: string) : Promise<{ bio: string }> => {
    const response = await apiFetch(`/users/me/profile/bio` ,{  method: 'PATCH', body: JSON.stringify({ bio }) }, 'application/json');
    return response as Promise<{ bio: string }>;
}
export const uploadUserAvatar = async (avatarFile: File) : Promise<{ profilePictureUrl: string }> => {
    const formData = new FormData();
    formData.append("ProfilePictureFile", avatarFile);
    const response = await apiFetch(`/users/me/profile/picture`,{ method: 'PATCH',  body: formData });
    return response as Promise<{ profilePictureUrl: string }>;
}
export const fetchUserGameHistory = async (nickname: string, page: number): Promise<{ gameHistory: Array<ProfileGame>, totalPages: number }> => {
    const response = await apiFetch(`/users/${nickname}/games?page=${page}&limit=8`,{ method: 'GET'}, 'application/json');
    return response as Promise<{ gameHistory: Array<ProfileGame>, totalPages: number }>;
}

export const addFriend = async (nickname: string) => {
    const response = await apiFetch(`/users/${nickname}/friend`,{method: 'POST'}, 'application/json');
    return response;
}
export const deleteFriend = async (friendNickname: string) => {
     const response = await apiFetch(`/users/${friendNickname}/friend`,{ method: 'DELETE' }, 'application/json');
    return response;
}
export const deleteAccount = async (nickname: string) => {
    const response = await apiFetch(`/users/${nickname}/account`,{ method: 'DELETE' }, 'application/json');
    return response;
}
export const createAccount = async (data:{login:string; nickname: string; password: string; bio: string;ratings: {rapid: number; blitz: number; bullet: number}}) => {
    const response = await apiFetch(`/users/account`,{  method: 'POST', body: JSON.stringify( data) }, 'application/json');
    return response;
}
export const getEloHistory = async (nickname: string, gameType: 'ratingRapid' | 'ratingBlitz' | 'ratingBullet'): Promise<Array<{ date: string; rating: number }>> => {
    const response = await apiFetch(`/users/${nickname}/elo?ratingType=${gameType}`,{ method: 'GET' }, 'application/json');
    return response as Promise<Array<{ date: string; rating: number }>>;
}   
export const changePassword = async (nickname: string, currentPassword: string, newPassword: string) => {
    const response = await apiFetch(`/users/${nickname}/password`,{ method: 'PATCH', body: JSON.stringify({ currentPassword, newPassword }) }, 'application/json');
    return response;
}
export const editAccount = async (nickname: string, data:{ password: string; bio: string;ratings: {rapid: number; blitz: number; bullet: number}}) => {
   
    const response = await apiFetch(`/users/${nickname}/account`,{ method: 'PATCH', body: JSON.stringify( data) }, 'application/json');
    return response;
}