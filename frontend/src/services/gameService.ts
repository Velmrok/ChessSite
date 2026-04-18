import apiFetch from "./api";

const API_URL = import.meta.env.VITE_API_URL;


export const fetchGame = async (gameId: string): Promise<GameState> => {
   const response = await apiFetch({ url: `/games/${gameId}`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
   return response.json() as Promise<GameState>;
}
export const joinGame = async (gameType: gameType, time: number, increment: number):
 Promise<{ status: string; gameId?: string }> => {

   const response = await apiFetch({ url: `/games/join`, method: 'POST', includeCredentials: true, contentType: 'application/json', body: JSON.stringify({ gameType, time, increment }) });
       
   return response.json() as Promise<{ status: string; gameId?: string }>;
}

export const getAllGames = async (url:string): Promise<{games: Array<GameSummary>, totalPages: number}> => {
   const response = await apiFetch({ url: `/games${url}`, method: 'GET', includeCredentials: true, contentType: 'application/json' });
   return response.json() as Promise<{games: Array<GameSummary>, totalPages: number}>;
}