

export type Rating = {
    rapid: number;
    blitz: number;
    bullet: number;
}
export type PublicUser = {
    nickname: string;
    avatar: string;
    rating: Rating;
    isOnline: boolean;
    lastActive: string;
    friendList: Array<string>;
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
        avatar: string;
        bio: string;
        isOnline: boolean;

        userInfo: UserInfo;
    };
export type User = PublicUser & {};