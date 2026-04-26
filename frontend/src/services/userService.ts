import type { FriendsOnlineResponse, FriendsProfileResponse, UserProfile } from "@/types/user";
import apiFetch from "./api";



export const fetchUserProfile = async (nickname: string): Promise<UserProfile> => {
    const response = await apiFetch({ url: `/users/${nickname}/profile`, method: 'GET', includeCredentials: false, contentType: 'application/json' });
    return response as Promise<UserProfile>;
};
export const fetchUserFriends = async (nickname: string, page: number, limit: number = 5): Promise<FriendsProfileResponse> => {
    const response = await apiFetch({ url: `/users/${nickname}/friends?page=${page}&limit=${limit}`, method: 'GET', includeCredentials: false, contentType: 'application/json' });
    
    return response as Promise<FriendsProfileResponse>;
}
export const fetchFriendsOnline = async (page: number, limit: number): Promise<FriendsOnlineResponse> => {
    const response = await apiFetch({ url: `/users/me/friends-online?page=${page}&limit=${limit}`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
    return response as Promise<FriendsOnlineResponse>;
}   
export const updateUserBio = async (bio: string) : Promise<{ bio: string }> => {
    const response = await apiFetch({ url: `/users/me/profile/bio`, method: 'PATCH', includeCredentials: true, contentType: 'application/json', body: JSON.stringify({ bio }) });
    return response as Promise<{ bio: string }>;
}
export const uploadUserAvatar = async (avatarFile: File) : Promise<{ profilePictureUrl: string }> => {
    const formData = new FormData();
    formData.append("ProfilePictureFile", avatarFile);
    const response = await apiFetch({ url: `/users/me/profile/picture`, method: 'PATCH', includeCredentials: true, contentType: undefined, body: formData });
    return response as Promise<{ profilePictureUrl: string }>;
}
export const fetchUserGameHistory = async (nickname: string, page: number): Promise<{ gameHistory: Array<ProfileGame>, totalPages: number }> => {
    const response = await apiFetch({ url: `/users/${nickname}/games?page=${page}`, method: 'GET', includeCredentials: false, contentType: 'application/json' });
    return response as Promise<{ gameHistory: Array<ProfileGame>, totalPages: number }>;
}

export const addFriend = async (nickname: string) => {
    const response = await apiFetch({ url: `/users/${nickname}/friend`, method: 'POST', includeCredentials: true, contentType: 'application/json' });
    return response;
}
export const deleteFriend = async (friendNickname: string) => {
     const response = await apiFetch({ url: `/users/${friendNickname}/friend`, method: 'DELETE', includeCredentials: true, contentType: 'application/json' });
    return response;
}
export const deleteAccount = async (nickname: string) => {
    const response = await apiFetch({ url: `/users/${nickname}/account`, method: 'DELETE', includeCredentials: true, contentType: 'application/json' });
    return response;
}
export const createAccount = async (data:{login:string; nickname: string; password: string; bio: string;ratings: {rapid: number; blitz: number; bullet: number}}) => {
    const response = await apiFetch({ url: `/users/account`, method: 'POST', includeCredentials: true, contentType: 'application/json', body: JSON.stringify( data) });
    return response;
}
export const getEloHistory = async (nickname: string, gameType: 'ratingRapid' | 'ratingBlitz' | 'ratingBullet'): Promise<Array<{ date: string; rating: number }>> => {
    const response = await apiFetch({ url: `/users/${nickname}/elo?ratingType=${gameType}`, method: 'GET', includeCredentials: false, contentType: 'application/json' });
    return response as Promise<Array<{ date: string; rating: number }>>;
}   
export const changePassword = async (nickname: string, currentPassword: string, newPassword: string) => {
    const response = await apiFetch({ url: `/users/${nickname}/password`, method: 'PATCH', includeCredentials: true, contentType: 'application/json', body: JSON.stringify({ currentPassword, newPassword }) });
    return response;
}
export const editAccount = async (nickname: string, data:{ password: string; bio: string;ratings: {rapid: number; blitz: number; bullet: number}}) => {
   
    const response = await apiFetch({ url: `/users/${nickname}/account`, method: 'PATCH', includeCredentials: true, contentType: 'application/json', body: JSON.stringify( data) });
    return response;
}