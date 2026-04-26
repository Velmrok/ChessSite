

export type Rating = {
    rapid: number;
    blitz: number;
    bullet: number;
}
export type PublicUser = {
    nickname: string;
    profilePictureUrl: string;
    rating: Rating;
    isOnline: boolean;
    lastActive: string;
    friendNicknames: Array<string>;
    isInQueue: boolean;
    role: 'user' | 'admin';
}
export type UserInfo = {
    rating: Rating;
    createdAt: string;
    gamesPlayed: number;
    wins: Rating;
    losses: Rating;
    draws: Rating;
    totalWins: number;
    totalLosses: number;
    totalDraws: number;
}

export type UserProfile = {
        nickname: string;
        profilePictureUrl: string;
        bio: string;
        isOnline: boolean;

        userInfo: UserInfo;
    };

export type FriendsOnlineResponse = {
    friends: Array<{
        nickname: string;
        profilePictureUrl: string;
    }>,
    totalPages: number;
}

export type FriendsProfileResponse = {
    friends: Array<{
        nickname: string;
        profilePictureUrl: string;
        rating: Rating;
        isOnline: boolean;
    }>,
    totalPages: number;
}



export type UsersLeaderboardResponse = {
    users: Array<{
        nickname: string;
        profilePictureUrl: string;
        rating: number;
    }>;
    totalPages: number;
}
export type User = PublicUser & {};