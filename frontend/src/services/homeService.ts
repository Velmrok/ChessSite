import apiFetch from "./api";

export const getHomeInfo = async (): Promise<HomeInfo>=>{
    const response = await apiFetch({ url: `/home/info`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
    return response.json() as Promise<HomeInfo>;
}
export const getQueueList = async (gameType:gameType| 'any'): Promise<QueueList>=>{
    const response = await apiFetch({ url: `/home/queue?gameType=${gameType}`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
    return response.json() as Promise<QueueList>;
}
