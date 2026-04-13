import apiFetch from "./api";

export const getHomeInfo = async (): Promise<HomeInfo>=>{
    const response = await apiFetch(`/home/info`, 'GET', true, 'application/json');
    return response.json() as Promise<HomeInfo>;
}
export const getQueueList = async (gameType:gameType| 'any'): Promise<QueueList>=>{
    const response = await apiFetch(`/home/queue?gameType=${gameType}`, 'GET', true, 'application/json');
    return response.json() as Promise<QueueList>;
}
