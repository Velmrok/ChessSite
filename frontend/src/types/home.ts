

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
    activeGames: number;
    totalUsers: number;
}
export type QmViewMode = 'queue' | 'leaderboard' | 'friends';