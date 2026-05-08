import type { LeaderboardResponse } from "@/types/home";
import apiFetch from "./api";
import type { PublicUser } from "@/types/user";


export const getLeaderboard = async (): Promise<LeaderboardResponse> => {
    const response = await apiFetch(`/home/leaderboard`,{  method: 'GET'}, 'application/json');
    return response as Promise<LeaderboardResponse>;
}

export const getQueueList = async (gameType:gameType| 'any'): Promise<QueueList>=>{
    const response = await apiFetch(`/home/queue?gameType=${gameType}`,{ method: 'GET' }, 'application/json');
    return response as Promise<QueueList>;
}
