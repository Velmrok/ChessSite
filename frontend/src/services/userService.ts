import apiFetch from "./api";



export const fetchUserProfile = async (nickname: string): Promise<UserProfile> => {
    const response = await apiFetch({ url: `/users/${nickname}/profile`, method: 'GET', includeCredentials: false, contentType: 'application/json' });
    return response as Promise<UserProfile>;
};
export const fetchUserFriends = async (nickname: string, page: number, limit: number = 5): Promise<{ friendList: Array<Friend>, totalPages: number }> => {
    const response = await apiFetch({ url: `/users/${nickname}/friends?page=${page}&limit=${limit}`, method: 'GET', includeCredentials: false, contentType: 'application/json' });
    return response as Promise<{ friendList: Array<Friend>, totalPages: number }>;
}
export const fetchUserOnlineFriends = async (nickname: string, page: number, limit: number = 5): Promise<{ friendList: Array<Omit<Friend, "rating">>, totalPages: number }> => {
    const response = await apiFetch({ url: `/users/${nickname}/friends/online?page=${page}&limit=${limit}`, method: 'GET', includeCredentials: false, contentType: 'application/json' });
    return response as Promise<{ friendList: Array<Friend>, totalPages: number }>;
}
export const updateUserBio = async (nickname: string, bio: string) => {
    const response = await apiFetch({ url: `/users/${nickname}/profile`, method: 'PATCH', includeCredentials: true, contentType: 'application/json', body: JSON.stringify({ bio }) });
    return response;
}
export const uploadUserAvatar = async (avatarFile: File) => {
    const formData = new FormData();
    formData.append("avatar", avatarFile);
    const response = await apiFetch({ url: `/users/avatar`, method: 'PATCH', includeCredentials: true, contentType: undefined, body: formData });
    return response;
}
export const fetchUserGameHistory = async (nickname: string, page: number): Promise<{ gameHistory: Array<ProfileGame>, totalPages: number }> => {
    const response = await apiFetch({ url: `/users/${nickname}/games?page=${page}`, method: 'GET', includeCredentials: false, contentType: 'application/json' });
    return response as Promise<{ gameHistory: Array<ProfileGame>, totalPages: number }>;
}

export const addFriend = async (friendNickname: string) => {
    const response = await apiFetch({ url: `/users/friend`, method: 'POST', includeCredentials: true, contentType: 'application/json', body: JSON.stringify({ friendNickname }) });
    return response;
}
export const deleteFriend = async (friendNickname: string) => {
     const response = await apiFetch({ url: `/users/friend`, method: 'DELETE', includeCredentials: true, contentType: 'application/json', body: JSON.stringify({ friendNickname }) });
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