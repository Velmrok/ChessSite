

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
export type UserSummary = {
    nickname: string;
    profilePictureUrl: string;
    rating: Rating;
    isOnline: boolean;

}
export type UserSearchItem = Pick<UserSummary, "nickname" | "profilePictureUrl" | "rating" | "isOnline"> &{
    lastActive: string;
};
export type UserLeaderboardItem = {
    nickname: string;
    profilePictureUrl: string;
    rating: number;
}
export type UserOnlineItem = Pick<UserSummary, "nickname" | "profilePictureUrl">

export type User = PublicUser & {};