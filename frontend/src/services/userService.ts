import apiFetch from "./api";



export const fetchUserProfile = async (nickname: string): Promise<UserProfile> => {
    const response = await apiFetch(`/users/${nickname}/profile`, 'GET', false, 'application/json');
    return response.json() as Promise<UserProfile>;
};
export const fetchUserFriends = async (nickname: string, page: number, limit: number = 5): Promise<{ friendList: Array<Friend>, totalPages: number }> => {
    const response = await apiFetch(`/users/${nickname}/friends?page=${page}&limit=${limit}`, 'GET', false, 'application/json');
    return response.json() as Promise<{ friendList: Array<Friend>, totalPages: number }>;
}
export const fetchUserOnlineFriends = async (nickname: string, page: number, limit: number = 5): Promise<{ friendList: Array<Omit<Friend, "rating">>, totalPages: number }> => {
    const response = await apiFetch(`/users/${nickname}/friends/online?page=${page}&limit=${limit}`, 'GET', true, 'application/json');
    return response.json() as Promise<{ friendList: Array<Friend>, totalPages: number }>;
}
export const updateUserBio = async (nickname: string, bio: string) => {
    const response = await apiFetch(`/users/${nickname}/profile`, 'PATCH', true, 'application/json', JSON.stringify({ bio }));
  
    return response.json();
}
export const uploadUserAvatar = async (avatarFile: File) => {
    const formData = new FormData();
    formData.append("avatar", avatarFile);
    const response = await apiFetch(`/users/avatar`, 'PATCH', true, undefined, formData);
    return response.json();
}
export const fetchUserGameHistory = async (nickname: string, page: number): Promise<{ gameHistory: Array<ProfileGame>, totalPages: number }> => {
    const response = await apiFetch(`/users/${nickname}/games?page=${page}`, 'GET', false, 'application/json');
    return response.json() as Promise<{ gameHistory: Array<ProfileGame>, totalPages: number }>;
}

export const addFriend = async (friendNickname: string) => {
    const response = await apiFetch(`/users/friend`, 'POST', true, 'application/json', JSON.stringify({ friendNickname }));
    return response.json();
}
export const deleteFriend = async (friendNickname: string) => {
     const response = await apiFetch(`/users/friend`, 'DELETE', true, 'application/json', JSON.stringify({ friendNickname }));
    return response.json();
}
export const deleteAccount = async (nickname: string) => {
    const response = await apiFetch(`/users/${nickname}/account`, 'DELETE', true, 'application/json');
    return response.json();
}
export const createAccount = async (data:{login:string; nickname: string; password: string; bio: string;ratings: {rapid: number; blitz: number; bullet: number}}) => {
    const response = await apiFetch(`/users/account`, 'POST', true, 'application/json',JSON.stringify( data));
    return response.json();
}
export const getEloHistory = async (nickname: string, gameType: 'ratingRapid' | 'ratingBlitz' | 'ratingBullet'): Promise<Array<{ date: string; rating: number }>> => {
    const response = await apiFetch(`/users/${nickname}/elo?ratingType=${gameType}`, 'GET', false, 'application/json');
    return response.json() as Promise<Array<{ date: string; rating: number }>>;
}   
export const changePassword = async (nickname: string, currentPassword: string, newPassword: string) => {
    const response = await apiFetch(`/users/${nickname}/password`, 'PATCH', true, 'application/json', JSON.stringify({ currentPassword, newPassword }));
    return response.json();
}
export const editAccount = async (nickname: string, data:{ password: string; bio: string;ratings: {rapid: number; blitz: number; bullet: number}}) => {
   
    const response = await apiFetch(`/users/${nickname}/account`, 'PATCH', true, 'application/json',JSON.stringify( data));
    return response.json();
}