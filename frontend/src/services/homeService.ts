import type { Leaderboard } from "@/types/home";
import apiFetch from "./api";
import type { PublicUser } from "@/types/user";


export const getLeaderboard = async (): Promise<Leaderboard> => {
    const response = await apiFetch({ url: `/home/leaderboard`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
    return response as Promise<Leaderboard>;
}

export const getQueueList = async (gameType:gameType| 'any'): Promise<QueueList>=>{
    const response = await apiFetch({ url: `/home/queue?gameType=${gameType}`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
    return response as Promise<QueueList>;
}
