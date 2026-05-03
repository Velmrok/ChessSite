

export type UsersLeaderboardSummary= {
    nickname: string;
    profilePictureUrl: string;
    rating: number;
}
export type LeaderboardResponse = {

    topRapidPlayers: Array<UsersLeaderboardSummary>;
    topBlitzPlayers: Array<UsersLeaderboardSummary>;
    topBulletPlayers: Array<UsersLeaderboardSummary>; 
       
        
}
export type HomeStats = {
    usersOnline: number;
    usersInQueue: number;
    matchesInProgress: number;
    createdAccounts: number;
}
export type QmViewMode = 'queue' | 'leaderboard' | 'friends';