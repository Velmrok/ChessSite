

export type Rating = {
    rapid: number;
    blitz: number;
    bullet: number;
}
export type PublicUser = {
    nickname: string;
    avatar: string;
    rating: Rating;
    onlineStatus: 'online' | 'offline';
    lastActive: string;
    friendList: Array<string>;
    isInQueue: boolean;
    role: 'user' | 'admin';
}
export type UserInfo = {
    rating: {
        rapid: number;
        blitz: number;
        bullet: number;
    };
    joinDate: string;
    gamesPlayed: number;
    wins: number;
    losses: number;
    draws: number;
}

export type UserProfile = {
        nickname: string;
        avatar: string;
        bio: string;
        onlineStatus: 'online' | 'offline';

        userInfo: UserInfo;
    };
export type User = PublicUser & {};