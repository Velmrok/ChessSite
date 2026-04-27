import type { UsersLeaderboardResponse } from "./user";

export type Leaderboard = {

    topRapidPlayers: Array<UsersLeaderboardResponse>;
    topBlitzPlayers: Array<UsersLeaderboardResponse>;
    topBulletPlayers: Array<UsersLeaderboardResponse>;
       
        
}
export type HomeStats = {
    usersOnline: number;
    matchesInProgress: number;
    createdAccounts: number;
}