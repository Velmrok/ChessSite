import type { GameState, GameSummary } from "@/types/game";
import apiFetch from "./api";


export const fetchGame = async (gameId: string): Promise<GameState> => {
   const response = await apiFetch(`/games/${gameId}`,{ method: 'GET' }, 'application/json');
   return response as Promise<GameState>;
}
export const joinGame = async (gameType: gameType, time: number, increment: number):
 Promise<{ status: string; gameId?: string }> => {

   const response = await apiFetch(`/games/join`,{ method: 'POST', body: JSON.stringify({ gameType, time, increment }) }, 'application/json');
       
   return response as Promise<{ status: string; gameId?: string }>;
}

export const getAllGames = async (url:string): Promise<{games: Array<GameSummary>, totalPages: number}> => {
   const response = await apiFetch(`/games${url}`,{ method: 'GET' }, 'application/json');
   return response as Promise<{games: Array<GameSummary>, totalPages: number}>;
}