

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

export type User = PublicUser & {};